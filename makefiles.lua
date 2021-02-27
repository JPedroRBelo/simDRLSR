local e={}
episode=1
--datagenaration phase = 0
--train phase = 1
torch.save('files/phase.txt',0,'ascii')

torch.save('recent_rewards.dat',e)
torch.save('recent_actions.dat',e)
torch.save('files/reward_history.dat',e)
torch.save('files/action_history.dat',e)
torch.save('files/ep_rewards.dat',e)  
torch.save('files/episode.dat',episode)
torch.save('files/episode.txt',episode,'ascii')

torch.save('files/q1_max_s_ep.dat',e)
torch.save('files/q1_max_d_ep.dat',e)
torch.save('files/q_max_s_ep.dat',e)
torch.save('files/q_max_d_ep.dat',e)
torch.save('files/td_err_s_ep.dat',e)
torch.save('files/td_err_d_ep.dat',e)
