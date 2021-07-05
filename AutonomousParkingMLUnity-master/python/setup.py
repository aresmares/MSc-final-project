from mlagents_envs.environment import UnityEnvironment

########## STEP 1 ########
import mlagents
from mlagents_envs.environment import UnityEnvironment as UE

env = UE(file_name='../build/Autopark.exe', seed=1, side_channels=[])

env.reset()

behavior_name = list(env.behavior_specs)[0]

print(f"Name of the behavior : {behavior_name}")
spec = env.behavior_specs[behavior_name]


print("Number of observations : ", len(spec.observation_specs))

decision_steps, terminal_steps = env.get_steps(behavior_name)
# print(decision_steps.obs)

for episode in range(3):
  env.reset()
  decision_steps, terminal_steps = env.get_steps(behavior_name)
  tracked_agent = -1 # -1 indicates not yet tracking
  episode_rewards = 0 # For the tracked_agent

  done = False # For the tracked_agent
  while not done:
      
    action = spec.action_spec.random_action(len(decision_steps))
    print("randact")

    # Set the actions
    env.set_actions(behavior_name, action)
    print(action)

    # Move the simulation forward
    env.step()

    # # Get the new simulation results
    # decision_steps, terminal_steps = env.get_steps(behavior_name)
    # if tracked_agent in decision_steps: # The agent requested a decision
    #   episode_rewards += decision_steps[tracked_agent].reward
    # if tracked_agent in terminal_steps: # The agent terminated its episode
    #   episode_rewards += terminal_steps[tracked_agent].reward
    #   print("TERMINAL")
    #   done = True
    print("goin")
  print(f"Total rewards for episode {episode} is {episode_rewards}")



env.close()
print("Closed environment")
###################################
"""
STEP 7: Everything is Finished -> Close the Environment.
"""

# env.close()


