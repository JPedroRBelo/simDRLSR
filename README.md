# simDRLSR: Deep Reinforcement Learning Simulator for Social Robotics

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![GNU License][license-shield]][license-url]
[![Scholar][scholar-shield]][scholar-url]


<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/JPedroRBelo/simDRLSR">
    <img src="doc/logo2.png" alt="Logo" width="228" height="80">
  </a>

  <!-- <h3 align="center">simDRLSR</h3> -->

  <p align="center">
    Simulator for Deep Reinforcement Learning and Social Robotics   
     <!-- <br /><a href="https://github.com/othneildrew/Best-README-Template"><strong>Explore the docs »</strong></a> -->
    <br />
    <br />
    <a href="https://youtu.be/e4C8hK4q8Ug" target="_blank">View Video</a>
    ·
    <a href="https://github.com/JPedroRBelo/simDRLSR/issues">Report Bug</a>
    ·
    <a href="https://github.com/JPedroRBelo/simDRLSR/issues">Request Feature</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#simconfigure">SimDRLSR Configuration</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>

## About the Project

SimDRLSR is a simulator for social robotics that offers resources for training and validation of reinforcement and deep learning methods.

The first version of the simulator is based on the MDQN algorithm as a reinforcement learning module, available at:

https://github.com/ahq1993/Multimodal-Deep-Q-Network-for-Social-Human-Robot-Interaction

The simDRLSR simulator offers the Pepper robot structure as an agent, which interacts with the environment using four actions:

 - Wait: wait, while looking at a random direction of the environment;
 - Look: looks at the nearest human, if he is in the robot's field of view;
 - Wave: makes the gesture of waving while looking at the nearest human;
 - Handshake: performs the handshake gesture to greet the human.


[![Watch the video](doc/preview.png)](https://youtu.be/e4C8hK4q8Ug)



## Getting Started

### Prerequisites

Simulator requirements:
- ![Unity 2020.2.6](https://unity3d.com/pt/unity/whats-new/2020.2.6)
- Unity Linux Build Support (IL2CPP)

MDQN requirements. It is recommended to install the following packages/frameworks using ![nvidia-docker](https://github.com/NVIDIA/nvidia-docker):
- Ubuntu  16.04  LTS  
- LUA  5.2  
- Torch7
- CUDA  10.1.

Alternatively, you can use pyMDQN, a Python 3 version of MDQN:
- Python 3.8 
- Pytorch (version 1.7.1 is recommended)
- (Optionally) Conda 3 

### Installation

1. Cloning repository
 
   Clone the repo without pyMDQN (MDQN with lua, only):
   ```sh
   git clone git@github.com:JPedroRBelo/simDRLSR.git simDRLSR
   ```
   Alternatively, you can clone the repo with python implementation (pyMDQN) of MDQN (also clone Lua version):
   
   ```sh
   git clone --recursive git@github.com:JPedroRBelo/simDRLSR.git simDRLSR
   ```
2. Building simDRLSR:

  - Open the simDRLSR Unity folder with Unity Editor
  - In Editor go to `File>Open Scene>`
    - Choose "Scenes/Library.unity"
  - Verify if NavMesh is configured:
    
    - Go to `Window>AI>Navigation`
    - Select "Libray" on Hierarchy
    - In `Navigation/Bake` tab, click on "Bake" button   
  
     ![Bake Image](doc/bake.png)
    
  - In Editor go to `File>Build Settings...`:
    - Scenes in Build, check:
      - [x] "Scenes/Libray"
      - [x] "Scenes/Empty"
    - Platform: PC, Mac & Linux Standalone
      - Target Platform: Linux
      - Architecture: x86_64 
    - Click Build button:
      - set executable name to simDRLSR.x86_64
      - save inside simDRLSR repo folder

    ![Build Image](doc/build.png)

     

## SimDRLSR Configuration

The `config.xml` file stores the settings for the simulation. The configuration options are:

- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Simulation Quality:
  - Very Low
  - Low
  - Medium
  - High
  - Very High
  
  The default value is "Medium".
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Frames Per Second (FPS): 60
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Screen Width: 1024
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Screen height: 768
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Fullscreen: True or False (default)
- ![#f03c15](https://via.placeholder.com/15/f03c15/000000?text=+) IP Address: stores the IP of MDQN module. Default is 172.17.0.3. If are running the MDQN on a docker container, you need to set the container's IP.If you are running MDQN on the same system as the simulator, localhost IP 127.0.0.1 will probably solve the problem).
- ![#1589F0](https://via.placeholder.com/15/1589F0/000000?text=+) Port: port that the MDQN module uses for socket communication. Default is 12375.
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Path Prob Folder: folder with human avatar probabilities files.
- ![#1589F0](https://via.placeholder.com/15/c5f015/000000?text=+) Path WorkDir: directory with MDQN files. Default is `simDRLSR/simMDQN/`. Change to `simDRLSR/pyMDQN`if you need to use pyMDQN.
- ![#c5f015](https://via.placeholder.com/15/c5f015/000000?text=+) Total Steps: defines the number of interactions that robot performs at each data collection.

The IP Adress,most likely, will be the only value you should change.



  
  


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/JPedroRBelo/simDRLSR.svg?style=for-the-badge
[contributors-url]: https://github.com/JPedroRBelo/simDRLSR/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/JPedroRBelo/simDRLSR.svg?style=for-the-badge
[forks-url]: https://github.com/JPedroRBelo/simDRLSR/network/members
[stars-shield]: https://img.shields.io/github/stars/JPedroRBelo/simDRLSR.svg?style=for-the-badge
[stars-url]: https://github.com/JPedroRBelo/simDRLSR/stargazers
[issues-shield]: https://img.shields.io/github/issues/JPedroRBelo/simDRLSR.svg?style=for-the-badge
[issues-url]: https://github.com/JPedroRBelo/simDRLSR/issues
[license-shield]: https://img.shields.io/badge/license-GNU%20GPU%203.0-brightgreen?style=for-the-badge
[license-url]: https://github.com/JPedroRBelo/simDRLSR/blob/development/LICENSE
[scholar-shield]: https://img.shields.io/badge/-Google%20Scholar-black.svg?style=for-the-badge&logo=google-scholar&colorB=555
[scholar-url]: https://scholar.google.com.br/citations?user=0nh0sDMAAAAJ&hl

