import time
from Main.Model import Agent
from Main.SharedMemory import SharedMemory

import threading


class Trainer:
    def __init__(self,agentNo):
        self.observe = []
        self.action = []
        self.reward = []
        self.n_epochs = 10
        self.alpha = 0.0003
        self.gamma = 0.99
        self.lmbda = 0.95
        self.temperature = 1 + ((agentNo%5)/2)*0.1*(-1**agentNo)
        self.agentNo = agentNo
        self.entropy_eps = 1e-4
        
        self.agent = Agent(alpha=self.alpha, n_epochs=self.n_epochs, 
                    input_dims=7, temperature=self.temperature)
        self.timer = 10
        self.waitingForReward = False

    
    def Observe(self,message):
        #If obs/rew unsync

        if self.waitingForReward: 
            self.Reset()
        #Load Model in enough time has called
        if self.timer == 0 : 
            self.agent.load_models()
            self.timer = 10

        #Unpack Observation
        out = []
        for i in range(0,len(message)):
            if (i>=2 and i!=9) : 
                out.append(float(message[i]))
        self.observe = out

        #Waiting for learning to finish
        while SharedMemory.training: time.sleep(0.05)
        
        #Choose action
        self.action, self.prob, self.val = self.agent.choose_action(self.observe)

        self.waitingForReward = True
        self.timer -= 1
        return self.action
        
    def Reward(self,message):
        if not self.waitingForReward: return
        self.reward = [int(message[3])+int(message[4]),bool(message[6])]
        print(self.observe)
        self.agent.remember(self.observe, self.action, self.prob, self.val, self.reward[0], self.reward[1])
        if self.agentNo == 0: threading.Thread(target=Train).start()
        self.waitingForReward = False

    
    def End(self):
        self.Reset()
        SharedMemory.clear_memory()

    def Reset(self):
        self.observe = []
        self.action = []
        self.reward = []
        self.timer = 10
        self.waitingForReward = False
        

def Train():
        agent = Agent(alpha=0.0003, n_epochs=4, 
                    input_dims=7, temperature=1)
        if not SharedMemory.training: SharedMemory.train_if_ready(agent)