require 'torch'
require 'os'
require 'robot_environment'
require 'socket'
require 'io'


function sleep(sec)
	socket = require("socket")
    socket.select(nil, nil, sec)
end

local t_episodes=14
local generate_phase = "datageneration_ql.lua"
local train_phase = "train_ql.lua"
local file_phase = 'files/phase.txt'

local episode=torch.load('files/episode.dat')



local phase = torch.load(file_phase,'ascii')
local i = episode
for i  = episode,t_episodes do
	collectgarbage();
	local handle = io.popen('sync')
	handle:close()
	local phase = torch.load(file_phase,'ascii')
	if(phase == 0) then
		print("Episode: "..i.." collection data.")

		local recent_rewards=torch.load('recent_rewards.dat')
		local reward_history=torch.load('files/reward_history.dat')
		print(#recent_rewards)
		
		local init_step = 1
		if(#reward_history~=episode) then
			if((#recent_rewards>0) and (#recent_rewards<=2050)) then
				init_step = #recent_rewards
			end
		end

		local env=Environment:new()

		env:send_data_to_pepper("start")
		sleep(1)
		env:close_connection()
		sleep(1)
		--Execute data generation phase script
		dofile(generate_phase)
		
		torch.save(file_phase,1,'ascii')

		collectgarbage();
		local env=Environment:new()
		env:send_data_to_pepper("stop")
		env:close_connection()
	end
	collectgarbage();
	local handle = io.popen('sync')
	handle:close()
	local phase = torch.load(file_phase,'ascii')
	if(phase == 1) then
		print("Episode: "..i.." training model.")
		
		print("Sending signal to kill simulator")
		torch.save('flag_simulator.txt',9,'ascii')
		sleep(10)		
		--Execute train phase script
		dofile(train_phase)
		torch.save(file_phase,0,'ascii')
		collectgarbage();
		sleep(10)
	end
	handle = io.popen('sync')
	handle:close()

end

print("Model trained...")

