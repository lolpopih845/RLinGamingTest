from Trap.Trainer import Trainer

trainers = []

def Init():
    for i in range(0,10):
        try:
            trainers.append(Trainer(i))
        except Exception as e:
            raise Exception(f"Trap Init Failed : {e}")

def Observe(message):#ignore atkCount
    index = int(message[1])
    try:
        return trainers[index].RecieveInput(message)
        
    except Exception as e:
        raise Exception(f"Trap Observe failed : {e}")
    
def Reward(message): #[reward,done,info]
    index = int(message[1])
    try:
        trainers[index].RecieveInput(message)
        
    except Exception as e:
        raise Exception(f"Trap Reward failed : {e}")
    
def End():
    try:
        if(len(trainers)>0):
            for i in range(0,10):
                trainers[i].End()
    except Exception as e:
        print(f'Trap End Failed : {e}')
    finally:
        trainers.clear()

def Reset(message):
    if len(trainers)==0 : return
    trainers[int(message[1])].Reset()