import time
import sys
import os
import threading
# insert root directory into python module search path
sys.path.insert(1, os.getcwd())

import Main.Commander as GeneralMandane

def RunCommand(pipe,message):
    messageArr = message.split('&')
    for message in messageArr:
        #print(message)
        command = message.split(',')[0]
        try:
            #print(message)
            if command == "Init": threading.Thread(target=Init).start()
            elif command == "Observe": threading.Thread(target=Observe,args=[pipe,message]).start()
            elif command == "Reward": threading.Thread(target=Reward,args=[message]).start()
            elif command == "End": threading.Thread(target=End,args=[message]).start()
            elif command == "Exit": threading.Thread(target=Exit,args=[message]).start()
            elif command == "Reset": threading.Thread(target=Reset,args=[message]).start()
        except Exception as e:
            print(f"Command Fail : {e}")
            return

def Init():
    GeneralMandane.Init()
def Observe(pipe,message): #[Command,No,PlX,PlY,BX,BY,PlVcX,PlVcY,atkCount,moveset]
    #print(message)
    message = message.split(',')
    
    action = GeneralMandane.Observe(message)#[Command,No,moveset,tele,rot,X,Y]
    #print(action)
    TrySending(pipe,action)
    

def Reward(message): #[Command,No,moveset,MSrwd,realrwd,traprwd,done]
    message = message.split(',')
    GeneralMandane.Reward(message)

def End(message):
    GeneralMandane.End()

def Exit(message):
    pass

def Reset(message): #[Command,No,isMS,isPara,isTrap]
    message = message.split(',')
    GeneralMandane.Reset(message)

def TrySending(pipe,action):
    try:
        #print(action)
        pipe.sendall(action.encode("UTF-8"))

    except Exception as e:
        print(f"Pipe error: {e}")
        return False