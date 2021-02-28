import socket
import uuid
import time


def move(s,pos):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["Move"])+DLIMT
	cmd += str(dictParType["WithPos"])+DLIMT
	cmd += str(pos[0])+DLIMT
	cmd += str(pos[1])+DLIMT
	cmd += str(pos[2])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def moveid(s,idd):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["Move"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += str(idd)
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def openLeft(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["ActivateLeft"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def openRight(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["ActivateRight"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def closeLeft(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["DeactivateLeft"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def closeRight(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["DeactivateRight"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def LookFor(s,lookfor):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["LookFor"])+DLIMT
	cmd += str(dictParType["WithString"])+DLIMT
	cmd += lookfor
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def HeadReset(s):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["HeadReset"])+DLIMT
	cmd += str(dictParType["WithoutParameter"])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def LeaveLeft(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["LeaveLeft"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def LeaveRight(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["LeaveRight"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def GetSenses(s):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["GetSenses"])+DLIMT
	cmd += str(dictParType["WithoutParameter"])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def LookAt(s,pos):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["LookAt"])+DLIMT
	cmd += str(dictParType["WithPos"])+DLIMT
	cmd += str(pos[0])+DLIMT
	cmd += str(pos[1])+DLIMT
	cmd += str(pos[2])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def LookAtId(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["LookAt"])+DLIMT
	cmd += str(dictParType["WithId"])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def Turn(s,pos):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["Turn"])+DLIMT
	cmd += str(dictParType["WithPos"])+DLIMT
	cmd += str(pos[0])+DLIMT
	cmd += str(pos[1])+DLIMT
	cmd += str(pos[2])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd


def Rotate(s,angle):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["Rotate"])+DLIMT
	cmd += str(dictParType["WithAngle"])+DLIMT
	cmd += str(angle)
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd


def TakeLeft(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["TakeLeft"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def TakeRight(s,id):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["TakeRight"])+DLIMT
	cmd += str(dictParType["WithId"])+DLIMT
	cmd += id
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def TasteLeft(s):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["TasteLeft"])+DLIMT
	cmd += str(dictParType["WithoutParameter"])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def TasteRight(s):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["TasteRight"])+DLIMT
	cmd += str(dictParType["WithoutParameter"])
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd

def Speech(s,say):
	uid = str(uuid.uuid4())
	cmd = uid+DLIMT
	cmd += str(dictCmds["Speech"])+DLIMT
	cmd += str(dictParType["WithString"])+DLIMT
	cmd += str(say)
	s.send(cmd.encode('ascii'))
	msg = s.recv(1024) 
	print (msg.decode('ascii'))
	return cmd


KITCHEN_POS = [-3.623,0.418,1.1]
LIVING_POS = [-4.7,0,-3.75]
DLIMT = "<|>";
ID_DOORKL = "DoorKitchenLiving"
ID_CRACKER = "Crackers"
ID_SALMON = "Salmon"
ID_DOORFRIDGE = "Fridge_Door"
ID_FRIDGE = "Fridge"
ID_MARIANA = "Mariana"
ID_TAP = "SinkTap"
POS_TAP = [-7.3,1.1,1.4]
POS_FRIDGE = [-5.8,0.5,5.5]
ID1_POS = [18.9,0,2.4]



POS_FRAG_SMELL = [-7.0,1.1,1.9]

dictCmds = {
    "ActivateLeft" : 1,
    "ActivateRight" : 2,
    "DeactivateLeft" : 3,
    "DeactivateRight" : 4,
    "HeadReset" : 5,
    "LeaveLeft" : 6,
    "LeaveRight" : 7,
    "LookAt" : 8,
    "LookFor" : 9,
    "Move" : 10,
    "Rotate" : 11,
    "SmellLeft" : 12,
    "SmellRight" : 13,
    "Speech" : 14,
    "TakeLeft" : 15,
    "TakeRight" : 16,
    "TasteLeft" : 17,
    "TasteRight" : 18,
    "Turn" : 19,
    "CancelCommands" : 20,
    "GetSenses" : 21
}

dictParType = {
    "WithId" : 1,
    "WithPos" : 2,
    "WithAngle" : 3,
    "WithString" : 4,
    "WithoutParameter" : 5
}

# create a socket object
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 

# get local machine name
#host = socket.gethostname()                           
host = "127.0.0.1"

port = 6321

# connection to hostname on the port.
s.connect((host, port))                               

# Receive no more than 1024 bytes
#msg = "1E009820-008C-2E00-756C-E0CB4EE729FE"+DLIMT+"10"+DLIMT+"2"+DLIMT+str(KITCHEN_POS[0]) +DLIMT+ str(KITCHEN_POS[1]) +DLIMT+str(KITCHEN_POS[2])
#msg = "1E009820-008C-2E00-756C-E0CB4EE729FE"+DELIMITER+"21"+DELIMITER+"5"
#GetSenses(s)

''''
Speech(s,"Hello, how can I help you?")
time.sleep(5)
msg = GetSenses(s)
msg = move(s,KITCHEN_POS)
msg = GetSenses(s)
msg = openRight(s,ID_DOORKL)
msg = GetSenses(s)
msg = move(s,KITCHEN_POS)
msg = GetSenses(s)
msg = LookAt(s,POS_TAP)
msg = GetSenses(s)
msg = moveid(s,ID_TAP)
msg = GetSenses(s)
msg = closeLeft(s,ID_TAP)
msg = GetSenses(s)
msg = LookAt(s,POS_FRAG_SMELL)
msg = GetSenses(s)
msg = moveid(s,ID_SALMON)
msg = GetSenses(s)
TakeRight(s,ID_SALMON)
msg = GetSenses(s)
TasteRight(s)
msg = GetSenses(s)
HeadReset(s)
GetSenses(s)
Turn(s,POS_FRIDGE)
GetSenses(s)
moveid(s,ID_DOORFRIDGE)
GetSenses(s)
LookAt(s,POS_FRIDGE)
GetSenses(s)
moveid(s,ID_DOORFRIDGE)
GetSenses(s)
openLeft(s,ID_DOORFRIDGE)
GetSenses(s)
LeaveRight(s,ID_FRIDGE)
GetSenses(s)
Rotate(s,-90)
HeadReset(s)
HeadReset(s)
closeRight(s,ID_DOORFRIDGE)
LookFor(s,"Crackers")
GetSenses(s)
moveid(s,ID_CRACKER)
GetSenses(s)
TakeRight(s,ID_CRACKER)
GetSenses(s)
HeadReset(s)
GetSenses(s)
move(s,LIVING_POS)
GetSenses(s)
LookFor(s,"Mariana")
GetSenses(s)
moveid(s,ID_MARIANA)
GetSenses(s)
Speech(s,'Hello Mariana, here your Crackers')
#Robot please get crackers for me
'''

'''
GetSenses(s)
moveid(s,ID_TAP)
GetSenses(s)
LookAt(s,POS_TAP)
GetSenses(s)
closeLeft(s,ID_TAP)
GetSenses(s)
LookAt(s,POS_FRAG_SMELL)
GetSenses(s)
moveid(s,ID_SALMON)
GetSenses(s)
TakeRight(s,ID_SALMON)
GetSenses(s)
HeadReset(s)
GetSenses(s)
Turn(s,POS_FRIDGE)
GetSenses(s)
moveid(s,ID_DOORFRIDGE)
GetSenses(s)
LookAt(s,POS_FRIDGE)
GetSenses(s)
#moveid(s,ID_DOORFRIDGE)
GetSenses(s)
openLeft(s,ID_DOORFRIDGE)
GetSenses(s)
LeaveRight(s,ID_FRIDGE)
GetSenses(s)
Rotate(s,-90)
GetSenses(s)
HeadReset(s)
GetSenses(s)
closeRight(s,ID_DOORFRIDGE)
GetSenses(s)
move(s,LIVING_POS)
GetSenses(s)
Rotate(s,60)
GetSenses(s)
'''

move(s,ID1_POS)
move(s,[7.6, 0.2, -13.2])
Rotate(s,-90)

s.close()