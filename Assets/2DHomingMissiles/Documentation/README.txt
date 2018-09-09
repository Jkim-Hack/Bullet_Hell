2D Homing Missiles Asset for Unity3D README
===========================================

Asset contents
==============
1. Proportional constant (PID controller) homing missile script + supporting scripts.
2. MissileLaunchScript script with custom inspector editor components for single / multiple missile launcher node configuration and pattern randomness configuration
3. ObjectPoolManager script to aid in the usage of optional object pooling for missiles
4. Missile sprite and prefab
5. Smoke trails for missiles
6. Demo scene to demonstrate dynamic instantiation of new missiles, as well as some custom launch patterns / multiple missile launches. Includes some extra helper / demo scripts too.
7. Free explosion audio effect and basic explosion particle effect
8. This readme file

Web demo: http://hobbyistcoder.com/demos/2DHomingMissilesDemo/2dhomingmissiles.html

Introduction
============

Website: http://hobbyistcoder.com

The 2D Homing Missiles asset for Unity gives you realistically moving missiles to use in your games and prototypes along with smoke trails. Missiles should never move directly towards their targets, this is not how they work in real life. Too many times we see simple missile follow scripts being used to allow missiles to track targets.

With the 2D Homing Missiles asset, a special proportional controller algorithm (PID controller) is used to move missiles intelligently, instructing missiles to reach their target, rather than just move directly to their targets. A desired state is constantly fed to the missile, as it's target moves, steering the missile in the right direction in order for it to reach it's target. This results in super realistic (and entertaining to watch) missile patterns.

This asset includes a custom MissileLaunch script that displays a customised inspector editor in the Unity3D editor, which gives you options to create launch 'sites' or 'nodes' on your GameObjects, from which missiles will fire. In addition to this, you can also configure other missile launch parameters, such as randomness, initial node 'swarm or arc' modes, and change the firing order or set up staged firing for multiple missile launches.

The code allows you to fire missiles in different ways, from multiple missiles to multiple targets, to single missiles to single targets, even allowing you to random target, or target entities in order.

Usage
=====

Simple
------
In order to setup these missiles, simply drop a missile, or many missiles into your scene, and drag a gameobject target onto the "target" property of the root missile gameobject. As soon as it has a target it will begin to intercept it.

Ideally you will want to instantiate or activate new missiles in your games whenever they are needed, at instantiation, you will simply give the MissileController.cs script a target reference to get them moving. A default maximum speed and propotional constant value are assigned to missiles to get them going. Tweak these to adjust missile performance - i.e. how 'swingy' or precise and fast they are.

Advanced (more customisation)
-----------------------------

Add a MissileLaunchScript.cs script component to your GameObject you wish to fire missiles from. Using the custom editor, select options to use.
You can tell it to fire missiles from an object pool (if you are going to be firing many) or choose not to (missiles are instantiated as needed). 
You can also setup all the parameters of missiles that will be fired from this GameObject in the editor. Once your settings have been chosen, click one of the four "Add node" buttons. 
(Left, Right, Top, Bottom). 
When one of these is clicked, and you have already selected a sprite to use for the missile launcher node, a node will be created
on your GameObject, with the layer order of the sprite set to 1 higher than your base GameObject's sprite. Missiles will be fired from this node, and if you set the options to use launcher nodes, 
and also set a suitable offset distance, missiles will initially 'arc' out, targeting a fake / invisible node at the offset. 
Note that if you used the "Create launcher node (up)" button, the fake node will be created at the offset distance "above (up)" from your parent GameObject. 
If you used the "down" button, then missiles will of course target a fake node below your parent GameObject. This gives a nice effect to missiles when they are fired out. 
A timer that you can also configure is used to specify how long the missiles should target this 'fake' node for, before switching to their 'real' targets.

Your targets are designated to the missiles by using the FireMissileUniformTargeting and FireMissileRandomTargeting methods in the MissileLaunch.cs script (public methods).
You need to pass different objects to the method. Here are the descriptions of what you should send in:

FireMissileUniformTargeting method
----------------------------------
GameObject[] targetGameObjects - an array of GameObjects you wish to target with missile(s) fired
List<GameObject> launcherNodes - a List of GameObjects to fire missiles from (there is a public field on the MissileLaunchScript called "launcherNodes" that should always hold a reference to all missile launcher nodes that you can use)
float delay - A delay to be used between missiles fired from launcher nodes (for example if you were doing a staged missile launch, you would give a short delay here - e.g. 0.1f)
bool swarmMissilesOutward - true or false as to whether you would like missiles to initially swarm outward or not (initially track fake nodes offset from your missile launcher nodes or not)

FireMissileRandomTargeting method
---------------------------------
GameObject[] targetGameObjects - an array of GameObjects you wish to target with missile(s) fired
List<GameObject> launcherNodes - a List of GameObjects to fire missiles from (there is a public field on the MissileLaunchScript called "launcherNodes" that should always hold a reference to all missile 
launcher nodes that you can use)
int launcherNodeCount - The count of launcher nodes on your GameObject. You must pass in the correct number here. It is used especially for cases where you are firing more missiles than you have targets. The excess missiles re-target extra targets, so the code needs to know the correct number of launcher nodes in order to re-target correctly. You can simply get this number by using the public launcherNodes field on the MissileLaunchScript - e.g. var launcherNodeCount = launcherNodes.Count; and pass in launcherNodeCount to this method parameter.
float delay - A delay to be used between missiles fired from launcher nodes (for example if you were doing a staged missile launch, you would give a short delay here - e.g. 0.1f)
bool swarmMissilesOutward - true or false as to whether you would like missiles to initially swarm outward or not (initially track fake nodes offset from your missile launcher nodes or not)


There is a demo scene which allows you to click to launch missiles at the three targets that fly around to the right in the scene. Missiles will target these objects and launch based on the settings you can define using the UI controls in the demo scene. Two sliders allow you to adjust missile performance to see how they react in flight. There are more controls to adjust launch options and other characteristics. After adjusting the sliders, any new missiles will inherit the new properties.

Note: If a missile loses it's target reference, it will detonate immediately. There is also a boolean property on the MissileController.cs script that allows you to specify if a missile should destroy it's target's gameobject upon collision. By default this is disabled.

Explosions: A basic explosion particle effect that uses Unity's built in particle systems and particle assets is included. A GameObject property is setup on the MissileController.cs script to drag and drop an explosion prefab of your own to. Once this is done, if the missile is destroyed, it will automatically instantiate an explosion at it's detonation point, instead of the default effect included. I recommend you try out the Detonator framework (free explosion asset for Unity) - simply download the asset, import into your game, and drag and drop a detonator prefab onto the missile "explosion" property point. You may need to adjust the detonator explosion prefab size and properties to get an explosion of the right size and look for your missiles.

Missile fuel: A missile fuel property is included. To use this, your missiles should enable the "usesFuel" boolean flag. This is a public field which will be visible on your missile under the MissileController script. When this is enabled, every tick of the update loop, fuel will run out on each missile that has this enabled.
Upon reaching 0, the missile will detonate. The fuel settings are not enabled by default, so just check the flag on, or set the boolean flag to true in your launcher script to use this feature. Don't forget to set an amount of fuel the missile has to start (default is 4.0f).

Tweaking performance when using lots of missiles
================================================

To tweak performance of your final game, you should consider how many missiles you use on screen at any point in time. If the number is high, consider using the built in object pool. You can choose to use pooling by ticking the option on in the MissileLaunchScript.cs component. The component will check to ensure you have an instance of the ObjectPoolManager.cs script in your scene (included with this asset). If not, you will be shown a red warning message. You can create the object pooler if you don't already have one in your scene by creating an empty GameObject, and adding the ObjectPoolManager.cs script to this. Supply the script with the included missile prefab in the "Prefabs" folder and specify how many to initially spawn. You can also tweak the smoke trail renderer's maximum number of particles and rate to gain more performance at the cost of visual impact.
