require 'nn'
require 'torch'
require 'robot_environment'
require 'image'
require 'RobotNQL'
require 'os'
local t_steps=2050

local gpu=-1

torch.manualSeed(torch.initialSeed())  
local win=nil

local episode=torch.load('files/episode.dat')
args={epi=episode}
local agent=RobotNQL(args)
local env=Environment:new()

local win=nil
local dirname_rgb='dataset/RGB/ep'..episode
local dirname_dep='dataset/Depth/ep'..episode
local dirname_model='results/ep'..episode
paths.mkdir(dirname_rgb)
paths.mkdir(dirname_dep)
paths.mkdir(dirname_model)



function filename()
   local str = debug.getinfo(2, "S").source:sub(2)
   return str:match("^.*/(.*).lua$") or str
 end






function generate_data(episode)
	--make new directory for new episode	

	local recent_rewards=torch.load('recent_rewards.dat')
	local recent_actions=torch.load('recent_actions.dat')
	local reward_history=torch.load('files/reward_history.dat')
	local action_history=torch.load('files/action_history.dat')
	local ep_rewards=torch.load('files/ep_rewards.dat')

	local aset = {'1','2','3','4'}
	local testing = 0.00001

	local arg_test = arg[1]	
	if arg_test ~= nil then
		if(arg_test=='true') then
			testing = 1
		end
	
	end	
	
	

    local init_step = 0
	
	if(#reward_history~=episode) then
		if((#recent_rewards>0) and (#recent_rewards<=t_steps)) then
			init_step = #recent_rewards
		end
	end

	if testing then
		--aset = {'1','1','1','1'}
		--aset = {'4','4','4','4'}
		init_step = 0
		print(testing)

	end


	local aux_total_rewards = 0
	for i=1,init_step do
		aux_total_rewards = aux_total_rewards+recent_rewards[i]
	end

	--Modified
	local actions= {}
	local rewards= {}

	if(init_step~=0) then
		actions= recent_actions
		rewards= recent_rewards
	end

	local total_reward = aux_total_rewards

	--init_step = 0

	env:send_data_to_pepper("step"..init_step)
	env:close_connection()
	env = Environment:new()

	local screen, depth, reward, terminal = env:perform_action('-',init_step+1)

	--local actions={}
	--local rewards={}
	--local total_reward=0

	--init_step = 0
	
	
	
	local step=init_step+1
			while step <=t_steps do
				print("Step="..step)
				local action_index=0
				numSteps=(episode-1)*t_steps+step
				if reward>15 then
					action_index = agent:perceive(1, screen,depth, terminal, false, numSteps,step,testing)
				else
					action_index = agent:perceive(0, screen,depth, terminal, false, numSteps,step,testing)
            end
				step=step+1		
				if action_index == nil then
						action_index=2
				end
				if not terminal then 
					screen,depth,reward,terminal=env:perform_action(aset[action_index],step)
				else  
					screen,depth, reward, terminal = env:perform_action('-',step)
				end

				if step >= t_steps then
					terminal=1
				end
				table.insert(rewards,reward)
				table.insert(actions,action_index)
				total_reward=total_reward+reward
				print("Total Reward: ",total_reward)
				print('================>')
				torch.save('recent_rewards.dat',rewards)
				torch.save('recent_actions.dat',actions)

				
			end
	table.insert(reward_history,rewards)
	table.insert(action_history,actions)
	table.insert(ep_rewards,total_reward)
	print("Table: ",table)
	print('+++++++++++++++++++++++++++++++++')
	
	 
	torch.save('files/validation_ep_rewards.dat',ep_rewards)
	torch.save('files/validation_reward_history.dat',reward_history)
	torch.save('files/validation_action_history.dat',action_history)

	--Added
	torch.save('recent_rewards.dat',{})
	torch.save('recent_actions.dat',{})

end 



function main()
 generate_data(episode)
 --- training 
 --episode=episode+1
 --print("Episode: ",episode)
 --torch.save('files/episode.txt',episode,'ascii')
 --torch.save('files/episode.dat',episode)
 env:close_connection()

end
main()
