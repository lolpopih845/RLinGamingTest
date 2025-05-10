import os
import time
import numpy as np
import torch
from torch import nn
import torch.nn.functional as F
from torch.distributions.categorical import Categorical
from Parameter.SharedMemory import SharedMemory

class ActorNetwork(nn.Module):
    def __init__(self, input_dims, alpha, fc1_dims=256, fc2_dims=256,temperature=1.0, chkpt_dir=os.getcwd()+"/Parameter/model"):
        super(ActorNetwork, self).__init__()

        self.checkpoint_file = os.path.join(chkpt_dir, 'ActorBoi')
        self.actor = nn.Sequential(
            nn.Linear(input_dims, fc1_dims),
            nn.ReLU(),
            nn.Linear(fc1_dims, fc2_dims),
            nn.ReLU(),
            nn.Linear(fc2_dims, 2*11)
        )

        self.optimizer = torch.optim.Adam(self.parameters(), lr=alpha)
        self.device = torch.device('cuda:0' if torch.cuda.is_available() else 'cpu')
        self.to(self.device)

    def forward(self, state):
        rest_output_logits = self.actor(state) / self.temperature
        rest_output_probs = F.log_softmax(rest_output_logits.view(-1, 2, 11), dim=-1)

        rest_action_dists = [Categorical(rest_output_probs[:, i, :]) for i in range(2)]

        return rest_action_dists

    def sample_action(self, state):
        dists = self.forward(state)

        # Sample from the categorical distributions
        rest_actions = [dist.sample() for dist in dists]

        return rest_actions

    def save_checkpoint(self):
        torch.save(self.state_dict(), self.checkpoint_file)

    def load_checkpoint(self):
        try:
            self.load_state_dict(torch.load(self.checkpoint_file, map_location=self.device))
            self.to(self.device)
        except:
            return

class CriticNetwork(nn.Module):
    def __init__(self, input_dims, alpha, fc1_dims=256, fc2_dims=256,
            chkpt_dir=os.getcwd()+"/Parameter/model"):
        super(CriticNetwork, self).__init__()

        self.checkpoint_file = os.path.join(chkpt_dir, 'CriticGrl')
        self.critic = nn.Sequential(
                nn.Linear(input_dims, fc1_dims),
                nn.ReLU(),
                nn.Linear(fc1_dims, fc2_dims),
                nn.ReLU(),
                nn.Linear(fc2_dims, 1)
        )

        self.optimizer = torch.optim.Adam(self.parameters(), lr=alpha)
        self.device = torch.device('cuda:0' if torch.cuda.is_available() else 'cpu')
        self.to(self.device)

    def forward(self, state):
        value = self.critic(state)

        return value

    def save_checkpoint(self):
        torch.save(self.state_dict(), self.checkpoint_file)

    def load_checkpoint(self):
        try:
            self.load_state_dict(torch.load(self.checkpoint_file))
        except:
            return

class Agent:
    def __init__(self, input_dims, gamma=0.99, alpha=0.0003, gae_lambda=0.95,
            policy_clip=0.2, n_epochs=10, temperature=1.0):
        self.gamma = gamma
        self.policy_clip = policy_clip
        self.n_epochs = n_epochs
        self.gae_lambda = gae_lambda

        self.actor = ActorNetwork(input_dims=input_dims, alpha=alpha,temperature=temperature)
        self.critic = CriticNetwork(input_dims, alpha)
    def remember(self, state, action, probs, vals, reward, done):
        while SharedMemory.training:
            time.sleep(0.1)
        SharedMemory.store_memory(state, action, probs, vals, reward, done)
        

    def save_models(self):
        #print('... saving models ...')
        self.actor.save_checkpoint()
        self.critic.save_checkpoint()

    def load_models(self):
        #print('... loading models ...')
        self.actor.load_checkpoint()
        self.critic.load_checkpoint()

    def choose_action(self, observation):
        with torch.no_grad():
            state = torch.tensor(observation, dtype=torch.float).to(self.actor.device)

            dists = self.actor(state)
            value = self.critic(state)
                
            actions = [dist.sample() for dist in dists]
            action_tensor = torch.stack(actions).T
            probs = [dist.log_prob(action_tensor[:, i]) for i, dist in enumerate(dists)]
            probs = torch.stack(probs, dim=-1).sum(dim=-1)  
            action = action_tensor.cpu().tolist() 
            value = torch.squeeze(value).item()

            return action[0], probs, value

    def learn(self, state_arr, action_arr, old_prob_arr, vals_arr, reward_arr, dones_arr, batches):
        #print('Start Learning')
        for _ in range(self.n_epochs):
            values = vals_arr.clone() 
            advantage = np.zeros(len(reward_arr), dtype=np.float32)

            for t in range(len(reward_arr) - 1):
                discount = 1
                a_t = 0
                for k in range(t, len(reward_arr) - 1):
                    delta = reward_arr[k] + self.gamma * values[k + 1] * (1 - int(dones_arr[k])) - values[k]
                    a_t += discount * delta
                    discount *= self.gamma * self.gae_lambda
                advantage[t] = a_t
            advantage = torch.tensor(advantage, dtype=torch.float32).to(self.actor.device)

            values = values.clone().detach().to(self.actor.device)

            for batch in batches:
                states = state_arr[batch].clone().detach().to(self.actor.device)
                old_probs = old_prob_arr[batch].clone().detach().to(self.actor.device)
                valid_batch = [i for i in batch if i < len(action_arr)]
                actions = torch.tensor(np.array(action_arr[valid_batch]), dtype=torch.long, device=self.actor.device)

                dists = self.actor(states)  
                log_probs = [dists[i].log_prob(actions[:, i]) for i in range(2)] 
                log_probs = torch.stack(log_probs, dim=-1).sum(dim=-1)  

                prob_ratio = torch.exp(log_probs - old_probs).clamp(0.01, 10)
                weighted_probs = advantage[batch] * prob_ratio
                weighted_clipped_probs = torch.clamp(prob_ratio, 1 - self.policy_clip, 1 + self.policy_clip) * advantage[batch]
                actor_loss = -torch.min(weighted_probs, weighted_clipped_probs).mean()

                critic_value = self.critic(states).squeeze()
                returns = advantage[batch] + values[batch]
                critic_loss = (returns - critic_value).pow(2).mean()

                total_loss = actor_loss + 0.5 * critic_loss

                self.actor.optimizer.zero_grad()
                self.critic.optimizer.zero_grad()
                total_loss.backward()
                self.actor.optimizer.step()
                self.critic.optimizer.step()

        #print('Finish Learning')
