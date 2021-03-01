require 'image'
require	'torch'

Environment = {}

function sleep(sec)
    socket.select(nil, nil, sec)
end

function Environment:new()
    env = {}
    setmetatable(env, self)
    self.__index = self
   	self.r_len=8 --recording time in sec
	self.raw_frame_height= 320   -- 640 --- height and width of captured frame  -- 320
	self.raw_frame_width=  240    -- 480 --- height and width of captured frame   -- 240
	self.proc_frame_size=84 --
	self.state_size=8
	self.frame_per_sec=1
	self.socket = require("socket")
	local port = 12375         --- The port address on which 'robot_listen.py' is listenting
	--local host='192.168.0.11' --- The IP address on which 'robot_listen.py' is listenting
	local host='10.62.6.208' --- The IP address on which 'robot_listen.py' is listenting
	self.client =self.socket.connect(host, port)
	while(self.client==nil) do 
		print("Can't connect with robot! Trying again...")
		self.client =self.socket.connect(host, port)	
		torch.save('flag_simulator.txt',1,'ascii')
		sleep(1)
	end
	torch.save('flag_simulator.txt',0,'ascii')
	return env
end




function Environment:pre_process(step)
	
	print('Preprocessing images')
	local images=torch.Tensor(self.r_len,1,self.raw_frame_width,self.raw_frame_height)
	local depths=torch.Tensor(self.r_len,1,self.raw_frame_width,self.raw_frame_height)
	episode=torch.load('files/episode.dat')
	dirname_rgb='dataset/RGB/ep'..episode
	dirname_dep='dataset/Depth/ep'..episode
	
	for i=1,self.r_len do
			local filename=dirname_rgb..'/image_'..step..'_'..i..'.png'
			local filename2=dirname_dep..'/depth_'..step..'_'..i..'.png'
    		images[i] =image.load(filename)--:select(1, 1)
			depths[i] =image.load(filename2)--:select(1, 1)
	end
     
	local proc_image=torch.Tensor(self.state_size,self.proc_frame_size,self.proc_frame_size)
	local proc_depth=torch.Tensor(self.state_size,self.proc_frame_size,self.proc_frame_size)
	for i=1, self.state_size do
		local x =images[i]
		local d=depths[i]
		local y=image.scale(x,self.proc_frame_size,self.proc_frame_size,'bilinear')
		local d2=image.scale(d,self.proc_frame_size,self.proc_frame_size,'bilinear')
		proc_image[i]=y[1]
		proc_depth[i]=d2[1]
	end
	
	return proc_image,proc_depth	

end 

--Conecta com robot_listen.py
--Envia ação {1..4}
--Retorna recompensa
function Environment:send_data_to_pepper(data)
	--local socket = require("socket")
	--local port = 12375         --- The port address on which 'robot_listen.py' is listenting
	--local host='127.0.0.1' --- The IP address on which 'robot_listen.py' is listenting
	-- client =socket.connect(host, port)
	print('Send data connected to Pepper')
	self.client:send(data)
	print('Sending data to Pepper')
	flag=true
	while flag do
		local data2, emsg, partial=self.client:receive()
		print('Received: ',data2)
		if data2 then
			data2 = string.gsub(data2,',','.' )
			--self.client:close()
			return tonumber(data2)
		end
		break
	end  
	print("Connected with the server")
	--client:close()
	return 0

end 

function Environment:perform_action(action,step)
   
	---out to pepper and get new state,reward, terminal
	--PERFORM action
	
	local r,term
	r=self:send_data_to_pepper(action)
	local s,d=self:pre_process(step)
	
	term=false
   return s,d,r,term
end 

function Environment:close_connection()
	self.client:close()
end


