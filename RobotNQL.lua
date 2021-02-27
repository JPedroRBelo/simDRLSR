require 'optim'

local nql = torch.class('RobotNQL')

function filename()
   local str = debug.getinfo(2, "S").source:sub(2)
   return str:match("^.*/(.*).lua$") or str
 end

function nql:__init(args)
    self.state_dim  = 84 -- State dimensionality 84x84.
    self.actions    = {'1','2','3','4'}
    self.n_actions  = #self.actions
    self.win=nil
    --- epsilon annealing
    self.ep_start   = 1.0
    self.ep         = self.ep_start -- Exploration probability.
    self.ep_end     = 0.1
    self.ep_endt    = 28000
	 self.learn_start= 0
     
    --self.bufferSize =  2000
	 self.episode=args.epi-1
	 self.iter=1
    self.seq=""	

    --orignal file names
	--local modelA='results/ep'..self.episode..'/model_cpu.net'
    --local modelB='results/ep'..self.episode..'/tmodel_cpu.net'

     local modelA='results/ep'..self.episode..'/modelA_cpu.net'
     local modelB='results/ep'..self.episode..'/modelB_cpu.net'
	 
     print(modelA)
     print(modelB)

    if paths.filep(modelA) and paths.filep(modelB) then
        print('File founded...')
		self.networkA=torch.load(modelA)
		self.networkB=torch.load(modelB)
        self.networkA:float()
        self.networkB:float() 
	 end


    -- JP: adicionado
    --if not isempty(self.networkA) then
    --    self.networkA:float()
    --end
    --if not isempty(self.networkB) then  
    --    self.networkB:float()
    --end
    -- JP: fim adicionado

    self.networkA:float()
    self.networkB:float()  

    
    self.numSteps = 0 -- Number of perceived states.
    self.lastState = nil
	 self.lastDepth = nil
    self.lastAction = nil
    self.lastTerminal=nil
    
	  
end


function nql:perceive(reward, state, depth, terminal, testing, numSteps, steps, testing_ep) 
  	
    
    local curState = state
	 local curDepth = depth  
    
    local actionIndex = 1
    if not terminal then
        actionIndex = self:eGreedy(curState,curDepth, numSteps, steps, testing_ep)
    end

    if not terminal then
        return actionIndex
    else
        return 0
    end
end


function nql:rng()


	local myString =""
	local myString2 ="1234"
	repeat
		local Choice = math.random(4)
		if string.find(myString,Choice)==nil then
			myString= myString..Choice
		end
	until string.len(myString)==4 
	return myString
	
end


function nql:eGreedy(state,depth, numSteps , steps, testing_ep) 
    self.ep = testing_ep or (self.ep_end +
                math.max(0, (self.ep_start - self.ep_end) * (self.ep_endt -
                math.max(0, numSteps - self.learn_start))/self.ep_endt))



     print('Exploration probability ',self.ep)

     if steps%4==1 or self.seq=="" then
                self.seq=self:rng()
                self.iter=1
     end
    -- Epsilon greedy
	 if torch.uniform() < self.ep then
		  
				
		  local action= tonumber(string.sub(self.seq,self.iter,self.iter))
          print('Action ',action)
          if(action==nil) then
            print('!!!!!!!!!!!!!!!!!!!!')
            print('Seq: ',self.seq)
            print('Iter',self.iter)
            print('!!!!!!!!!!!!!!!!!!!!')
          end
		  self.iter=self.iter+1
		  return action
    else
		  self.iter=self.iter+1
        return self:greedy(state,depth)
    end
end


function nql:greedy(state,depth) 
	 print("greedy")
    self.networkA:evaluate()
	 self.networkB:evaluate()
    state = state:float()
	 depth=depth:float()
	 local win=nil
    local q1 = self.networkA:forward(state)
	 local q2 = self.networkB:forward(depth)
	 local ts=q1[1]+q1[2]+q1[3]+q1[4]
	 local td=q2[1]+q2[2]+q2[3]+q2[4]

	 q_fus=(q1/ts)*0.5+(q2/td)*0.5
    local maxq = q_fus[1]
    local besta = {1}
    for a = 2, self.n_actions do
        if q_fus[a] > maxq then
            besta = { a }
            maxq = q_fus[a]
        elseif q_fus[a] == maxq then
            besta[#besta+1] = a
        end
    end
    self.bestq = maxq

    local r = torch.random(1, #besta)

    self.lastAction = besta[r]

    return besta[r]
end


