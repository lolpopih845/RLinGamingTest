import torch
import numpy as np


class SharedMemory:
    max_size = 100  # Limit total memory size
    states = []
    probs = []
    vals = []
    actions = []
    rewards = []
    dones = []
    training = False
    trainingNo = 0
    

    batch_size = 48
    def generate_batches():
        n_states = len(SharedMemory.states)
        batch_start = np.arange(0, n_states, SharedMemory.batch_size)
        indices = np.arange(n_states, dtype=np.int64)
        np.random.shuffle(indices)
        batches = [indices[i:i+SharedMemory.batch_size] for i in batch_start if i+SharedMemory.batch_size <= n_states]

        try:
            max_len = max(len(s) for s in SharedMemory.states)  # Find longest state
            padded_states = np.array([s + [0] * (max_len - len(s)) for s in SharedMemory.states], dtype=np.float32)
            
            return padded_states, \
                np.array(SharedMemory.actions, dtype=np.float32), \
                np.array(SharedMemory.probs, dtype=np.float32), \
                np.array(SharedMemory.vals, dtype=np.float32), \
                np.array(SharedMemory.rewards, dtype=np.float32), \
                np.array(SharedMemory.dones, dtype=np.float32), \
                batches
        except Exception as e:
            print(f"Error in generate_batches: {e}, States: {SharedMemory.states[:5]}")  # Print first 5 for debugging
            return 
    def clear_memory():
        SharedMemory.states = []
        SharedMemory.probs = []
        SharedMemory.actions = []
        SharedMemory.rewards = []
        SharedMemory.dones = []
        SharedMemory.vals = []
    @staticmethod
    def store_memory(state, action, probs, vals, reward, done):
        SharedMemory.states.append(state)
        SharedMemory.actions.append(action)
        SharedMemory.probs.append(probs)
        SharedMemory.vals.append(vals)
        SharedMemory.rewards.append(reward)
        SharedMemory.dones.append(done)

    @staticmethod
    def train_if_ready(agent):
        agent.load_models()
        training = True
        if len(SharedMemory.actions) >= SharedMemory.batch_size:
            states, actions, probs, values, rewards, dones, batches = SharedMemory.generate_batches()

            # Convert to PyTorch tensors
            states = torch.tensor(np.array(states), dtype=torch.float).to(agent.actor.device)
            actions = torch.tensor(np.array(actions)).to(agent.actor.device)
            probs = torch.tensor(np.array(probs)).to(agent.actor.device)
            values = torch.tensor(np.array(values)).to(agent.actor.device)
            rewards = torch.tensor(np.array(rewards)).to(agent.actor.device)
            dones = torch.tensor(np.array(dones)).to(agent.actor.device)
            batches = batches

            # Train PPO agent            
            agent.learn(states, actions, probs, values, rewards, dones, batches)
            agent.save_models()
            
                # Clear the memory after training
            SharedMemory.clear_memory()
            SharedMemory.trainingNo +=1
            print("Para training done :",SharedMemory.trainingNo)
            training = False
            
            