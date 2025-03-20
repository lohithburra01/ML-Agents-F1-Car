# ML-Agents: F1 Race Simulation in Unity

https://github.com/user-attachments/assets/b54315e7-c8a8-4969-a543-ebcc6cffd7db


## Project Overview
This project is an **F1 race simulation using Unity ML-Agents**, where an AI-controlled **Ferrari F1 car** learns to drive on a simplified version of the **Bahrain International Circuit**. The AI is trained using **reinforcement learning (RL)** with **Proximal Policy Optimization (PPO)**, gradually improving its driving skills through trial and error.  

The project is structured in three major phases:  
1. **Training Phase** – The AI learns to drive through reinforcement learning.  
2. **Inference Phase** – The AI applies its learned behavior and completes the circuit.  
3. **Multi-Agent Expansion** *(Upcoming)* – AI agents will compete against each other in a race environment.  

This project was trained using **Unity ML-Agents Toolkit**, leveraging **CUDA acceleration (CUDA 12.1)** for faster model training.  

---

# Features & Implementation 

## F1 Track & Environment  
- **Circuit:** The track is a simplified **Bahrain GP Circuit** modeled in Unity.  
- **Track Barriers:** The car is kept within bounds using **walls**.  
- **Checkpoint System:** Checkpoints are placed around the track to guide the AI and assign rewards.  

## AI-Controlled F1 Car
- **Low-poly Ferrari F1 model** serves as the agent.  
- Uses **wheel colliders** and **Unity’s physics system** for realistic driving behavior.  
- **Sensor System:** The car detects walls, track boundaries, and checkpoints via **Ray Perception Sensors**.  

---

# Training Process & ML-Agents Configuration

At the start of training, the AI has **no knowledge of how to drive**. It explores the environment and gradually improves through trial and error, receiving rewards for **staying on track, passing checkpoints, and completing laps efficiently**.  

**Comparison: Before vs. After Training**  

| Phase       | Behavior |
|------------|----------|
| **Training**  | Car struggles, crashes frequently, slow movements. |
| **Inference** | Car drives smoothly, follows track, minimal crashes. |

---

## Training Video: AI Learning to Drive  


https://github.com/user-attachments/assets/811e4610-4dc5-4a55-b495-123e486985ce


 
*(Replace this with the training video link – this is the jerky, untrained AI struggling to drive.)*  

### Training Objectives
Drive forward and complete laps.  
Stay within the track boundaries.  
Optimize speed and braking for better lap times.  
Avoid crashes with walls or unnecessary slowdowns.  

### PPO Configuration 
The **PPO algorithm** is used for training, configured with the following hyperparameters:  

```yaml
behaviors:
  CarAgent:
    trainer_type: ppo
    hyperparameters:
      batch_size: 2048
      buffer_size: 20480
      learning_rate: 3.0e-4
      beta: 0.02
      epsilon: 0.2
    network_settings:
      normalize: true
      hidden_units: 128
      num_layers: 2
    reward_signals:
      extrinsic:
        gamma: 0.99
        strength: 1.0
    max_steps: 10000000
    time_horizon: 1000
    summary_freq: 10000
```

## Training Process & ML-Agents Configuration 

At the start of training, the AI has **no knowledge of how to drive**. It explores the environment and gradually improves through trial and error, receiving rewards and penalties based on its behavior.  


## Reward & Penalty System

The AI is trained using **reinforcement learning**, meaning it gets **rewards for good behavior** and **penalties for mistakes**.  

### Reward System:
- **+1.0** → Passing a checkpoint.  
- **+25.0** → Completing the track successfully.  

### Penalty System: 
- **-1.0** → Crashing into walls.  
- **-0.001** → Time penalty for slow progress.  
- **-5.0** → Timeout penalty if too slow.  
- **-25.0** → Hitting the wrong checkpoint.  
- **30-second timeout** per checkpoint; failure to reach it results in a penalty.  

These parameters **encourage the AI to drive efficiently, avoid mistakes, and complete laps as quickly as possible**.  


 **The AI was trained for over 10 million steps**, resulting in **stable, smooth driving behavior.**  

---

# **Future Plans: Multi-Agent Racing  vs **  

### Upcoming Features: 
🔹 **Waypoint Training** – The AI will be trained to follow the **ideal racing line** for maximum efficiency.  
🔹 **Multi-Agent Mode** – Instead of a single AI car, **two AI agents will compete against each other**, simulating an F1 race.  
🔹 **Race Strategy Training** – AI agents will learn to optimize **overtaking, defensive driving, and braking techniques**.  

---

# How to Run the Project 

## Clone the Repository  
```sh
git clone https://github.com/yourusername/f1-ml-agents.git
cd f1-ml-agents
```

## **Open Unity & Set Up ML-Agents**  
- Install **ML-Agents Toolkit** in Unity.  
- Ensure **Python & TensorFlow** are installed.  

## **Train the AI**  
```sh
mlagents-learn config.yaml --run-id=CarAgentTraining
```
- The AI will start training and improve over time.  
- Training progress is saved in the **results folder**.  

## **Run Inference (Test Trained Model)**  
- Load the trained model in Unity.  
- Press **Play** to see the AI complete the track.  

---

# **📂 Project Structure**  
```
📁 F1-ML-Agents
│── 📂 Assets
│   ├── 📁 ML-Agents
│   ├── 📁 Models (Trained models stored here)
│   ├── 📁 Prefabs (Car & Track elements)
│── 📂 Scripts
│   ├── CarAgent.cs
│   ├── CheckpointManager.cs
│   ├── TrainingSetup.cs
│── 📂 Config
│   ├── config.yaml (ML-Agents training config)
│── README.md
```

---

# **License & Credits**  
**License**: Open-source (MIT License). Feel free to modify and improve!  
**Acknowledgments**:  
- **Unity ML-Agents** for providing the framework.  
- **Formula 1** inspiration for track design.  
