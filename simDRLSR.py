#!/usr/bin/env python
import signal
import sys
import os
import time
import subprocess
from subprocess import Popen



def signal_handler(sig, frame):
    process.terminate()
    sys.exit(0)

def getValue(filename):
	line = subprocess.check_output(['tail', '-1', filename])
	return line.decode('utf-8').replace('\n', '')

def setValue(filename,value):
	f = open(filename, "w")
	f.write(value)
	f.close()





command = './simDRLSR.x86_64'
filename = 'simMDQN/flag_simulator.txt'


process = Popen('false') # something long running
signal.signal(signal.SIGINT, signal_handler)
print('Running...')

while(True):
	flag = getValue(filename)
	if flag == "1":
		process.terminate()
		time.sleep(5)
		process = Popen(command)
		time.sleep(5)
		setValue(filename,"0")
	elif flag == "9":
		process.terminate()
		setValue(filename,"0")
	time.sleep(10)

	

