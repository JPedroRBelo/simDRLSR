require 'cunn'
require 'gmodel'
require 'paths'
local model    = create_network()
local tmodel = model:clone()
local save_modelA_gpu='results/ep0/modelA_cuda.net'
local save_modelB_gpu='results/ep0/modelB_cuda.net'
local save_tmodelA_gpu='results/ep0/tmodelA_cuda.net'
local save_tmodelB_gpu='results/ep0/tmodelB_cuda.net'
local save_modelA_cpu='results/ep0/modelA_cpu.net'
local save_modelB_cpu='results/ep0/modelB_cpu.net'
local save_tmodelA_cpu='results/ep0/tmodelA_cpu.net'
local save_tmodelB_cpu='results/ep0/tmodelB_cpu.net'
paths.mkdir('results/ep0')
torch.save(save_modelA_gpu,model)
torch.save(save_modelB_gpu,model)
torch.save(save_tmodelA_gpu,tmodel)
torch.save(save_tmodelB_gpu,tmodel)

model=model:float()
tmodel=tmodel:float()

torch.save(save_modelA_cpu,model)
torch.save(save_modelB_cpu,model)
torch.save(save_tmodelA_cpu,tmodel)
torch.save(save_tmodelB_cpu,tmodel)
