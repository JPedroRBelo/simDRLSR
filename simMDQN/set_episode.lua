episode= arg[1]

if episode ~= nil then
	print('Configuring files with episode: ',episode)
	torch.save('files/episode.dat',episode)
	torch.save('files/episode.txt',episode,'ascii')

else 
	print('nil')
end


