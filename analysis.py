import torch
import numpy as np
import pickle


folder = 'simMDQN/files'



t_steps=2050
datContent = []

rewards_file = folder+'/reward_history.dat'

#rewards = pickle.load(rewards_file,encoding='latin-1')

'''
table = []
with open(rewards_file, "rb") as fin:
  	buf = fin.readlines()
  	for i in buf:
  		bytess = map(int, i)
  		l = list(bytess)
  		table.append(l)
'''

filename = rewards_file

with open(filename, 'rb') as f:
    nx, ny = ...  # parse; advance file-pointer to data segment
    data = np.fromfile(f, dtype='>f8', count=nx*ny)
    array = np.reshape(data, [nx, ny], order='F')

'''
objects = []
with (open(rewards_file,encoding='latin-1')) as openfile:
    while True:
        try:
            objects.append(pickle.load(openfile))
        except EOFError:
            break
'''




#rewards=torch.load(folder+'/reward_history.dat')
#actions=torch.load(folder+'/action_history.dat')
#datContent = open(rewards_file,encoding='latin-1')
#datContent=torch.load(rewards_file,encoding='bytes')
#datContent =byte_array.decode(datContent)
'''
with open(rewards_file,encoding='latin-1') as f:
	datContent = list(f)
	#datContent = list(f.decode('utf8'))

print(datContent)
'''
#results = {{}}

'''
for i=1,#actions do
	local hspos = 0
	local hsneg = 0
	local wave = 0
	local wait = 0
	local look = 0

	for step=1,t_steps do		

		if actions[i][step] == 4 then
			if rewards[i][step]>0 then
				hspos = hspos+1
			elseif rewards[i][step]==-0.1 then 
				hsneg = hsneg+1
			end
		elseif actions[i][step] == 1 then
			wait = wait+1
		elseif actions[i][step] == 2 then
			look = look+1
		elseif actions[i][step] == 3 then
			wave = wave+1
		end
	end
	table.insert(results,{hspos,hsneg,wave,wait,look})
	print('###################')
	print('Epoch\t',i)
	print('HS Suc.\t',hspos)
	print('HS Fail\t',hsneg)
	print('Acuracy\t',((hspos)/(hspos+hsneg)))
	print('Wait\t',wait)
	print('Look\t',look)
	print('Wave\t',wave)	
end

print(results)

require 'math'

-- we create a 'path' for the function f(x) = sin(x)*exp(-0.1*x)
-- and plot it

p = graph.plot('Example')
line = graph.fxline(function(x) return sin(x)*exp(-0.1*x) end, 0, 10*pi)
p:addline(line)
p:show()'''