from Parameter.Trainer import Trainer

trainers = []
def Init():
    for i in range(0,10):
        try:
            trainers.append(Trainer(i))
        except Exception as e:
            raise Exception(f"Para Init Failed : {e}")

def Observe(message):#ignore atkCount
    index = int(message[1])
    try:
        return trainers[index].Observe(message)
    except Exception as e:
        raise Exception(f"Para Observe failed : {e}")
    
def Reward(message): #[reward,done,info]
    index = int(message[1])
    try:
        trainers[index].Reward(message)
    except Exception as e:
        raise Exception(f"Para Reward failed : {e}")
    
def End():
    try:
        if(len(trainers)>0):
            for i in range(0,10):
                trainers[i].End()
    except Exception as e:
        print(f'Para End Failed : {e}')
    finally:
        trainers.clear()

def Reset(message):
    if len(trainers)==0 : return
    trainers[int(message[1])].Reset()