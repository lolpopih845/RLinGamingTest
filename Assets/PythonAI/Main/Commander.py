import Parameter.Commander as GeneralPitcher
import Trap.Commander as GeneralThomas
from Main.Trainer import Trainer

trainers = []
def Init():
    for i in range(0,10):
        try:
            trainers.append(Trainer(i))
            GeneralPitcher.Init()
            GeneralThomas.Init()
        except Exception as e:
            print(e)

def Observe(message):#ignore moveset
    index = int(message[1])
    try:
        action = int(message[9])
        if index>=5 or index==0 : 
            action = trainers[index].Observe(message)
        message[9] = str(action)
        para = GeneralPitcher.Observe(message)
        trap = "0,0"
        if action==6 or action==12:
            trap = GeneralThomas.Observe(message)
            
        return f"Action,{str(index)},{str(action)},{para},{trap}&"
    except Exception as e:
        print(e)
        return "Actions,"+ str(index) + ",0,0,0,0,0&"
    
def Reward(message): #[reward,done,info]
    index = int(message[1])
    try:
        trainers[index].Reward(message)
        GeneralPitcher.Reward(message)
        GeneralThomas.Reward(message)
    except:
        return

def Reset(message):
    if bool(message[2]):
        if len(trainers)==0 : return
        trainers[int(message[1])].Reset()
    if bool(message[3]):
        GeneralPitcher.Reset(message)
    if bool(message[4]):
        GeneralThomas.Reset(message)

def End():
    try:
        if(len(trainers)>0):
            for i in range(0,10):
                trainers[i].End()
    except Exception as e:
        print(f'Main End Failed : {e}')
    finally:
        trainers.clear()
        GeneralPitcher.End()
        GeneralThomas.End()