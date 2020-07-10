# Unity3D Game Developer — Case Study

## 1 — Select one of two games

Given the choice between Split Balls 3D and Tower Color, I have selected the Tower Color for this case study.
Both games were based in physics but the Split Balls 3D requires a bigger investment in level design, whereas the Tower Color levels can be procedurally generated.
Due to the tight time frame, I chose Tower Color in order to focus on other aspects and still achieve a great gameplay experience.

## 2 — Playable and solid version of the game

The game was tested on a Google Pixel 3a and runs smoothly at 60 FPS and without any bugs (as far as I am aware).

It is possible that the level loading might cause a slowdown on low end devices due to the amount of stacked blocks that need to adjust their position in the very first frames — although this should not be a problem during the gameplay, and only when the level is loaded. There are several ways that this can be addressed, but without a low end device I chose not to spend too much time on something that I cannot actually confirm if it is a problem.

## 3 — Improve the game with an extra gameplay feature

I've implemented a "powers" feature that gives the player the possibility to add extra effects to the ball.
Due to the time constraints I could only implement one type of power, although the implemented system is prepared to be extended for new powers.
The implemented power was a "rainbow" ball, which can destroy blocks of any color.

Currently each level starts with 3 "rainbow" balls, but the idea is to have powers as consumables, which could be won as the player progresses through the game, and also obtained for example by watching a rewarded video ad.

### Parts of the code could be implemented as independent modules

There are already a few scripts that are independent from the rest of the code and could be extracted to separate modules. All of them were developed during the course of this case study. These are:

- _TaskUtils_ — Maps the usual coroutines functionalities provided by Unity to the C# more modern task based asynchronous programing paradigm;
- _UITransitions_ — Provide simple fade and scale transitions that can be applied to UI elements;
- _FloatingBehaviour_ — As the name suggests, implements a floating behaviour on the game object that it is attached to.

Besides these, the _InputHandler_ sole purpose is to receive touches that are not caught from the rest of the UI, so it could be modified in order to not depend on anything else and be decoupled from the rest of the game. This could be done by providing event listeners that others could subscribe to.

## 4 — Deliverables

- Source code — https://github.com/gapspt/Tower-Color
- Android build — https://github.com/gapspt/Tower-Color/blob/master/deliverables/android-build-v1.apk?raw=true
- Gameplay video — https://github.com/gapspt/Tower-Color/blob/master/deliverables/gameplay-v1.mp4?raw=true

## Additional notes

I used a vanilla approach to Unity in this project and did not to use any plugins, with the exception of the skybox from BOXOPHOBIC.
Besides the skybox and a font file, all the assets were created by me for this project. This includes all the images, scripts, shaders, materials, and prefabs. The water shader was created in Shader Graph through a tutorial so it might be similar to others available online.
Although some plugins could be helpful to perform tasks such as _tweening_, I didn't find a strong need to use any of such plugins for the simple behaviours that were implemented.

Towards the end of the project, both the _Level_ and the _UIManager_ scripts started to get a bit messy, so perhaps they could benefit from some restructuring — although since they are still relatively small files, this should not be a big issue.

Features that were not implemented due to the time constraints:

- Fire ball power that upon impact would cause all the nearby blocks to explode;
- Better visual feedback when a level is won, such as throwing confetti or fireworks;
- Vibration when the blocks are destroyed and in other gameplay events;
- Allow customizing the blocks size — currently it is assumed that they are 1 unity high and 1 unity of diameter.
