2D Laser Pack Asset for Unity3D README
======================================

Asset contents
==============

1. Two different laser type component scripts (LineBased and SpriteBased)
2. 9 x different line based materials/textures
3. 2 x different animated spritesheet variants for sprite based laser components (x3)
4. Pre-configured laser prefabs (one for each laser type)
5. Lots of extra demo related scripts
6. 3 x demo scenes with fully customisable controls / examples
7. Well commented and documented code
8. Custom inspector editors for each laser script component type
9. This readme file

Web player demo: http://hobbyistcoder.com/demos/2DLaserPack1.0/2DLaserPack.html
Web GL demo: http://hobbyistcoder.com/demos/2DLaserPackWebGL1.0/index.html
Youtube: https://www.youtube.com/watch?v=6Dvvm-rjI7s

Introduction
============

Website: http://hobbyistcoder.com

This asset provides you with highly configurable 2D lasers. You can change all aspects of the lasers including looks, special effects (like optional lightning arc, hit spark effects), collision handling, target tracking, rotation speeds, and more. There are 3 x demo scenes that demonstrate different usages of the lasers, and provide on-screen controls to allow real-time manipulation of the lasers.

Demo scenes:
============

1. "Laser Gauntlet". Two line based laser are setup and track you, the player. Use WASD to avoid lasers, taking cover behind crates. Your health is depleted when lasers touch you, and you die when your health reaches 0.

2. "Laser types configuration" This demo gives you two laser types - the sprite based, and the line based. It provides on-screen controls to allow you to manipulate all aspects of the laser systems.

3. "Laser Grid" A rotating box spins around, with various lasers setup around the edges, aiming inward. Click each laser 'node' to enable/disable it. A barrel rolls around inside the box, giving the lasers a target to collide with.

Usage
=====

Usage instructions
------------------

1. Drop a copy of the LineLaser.prefab, BlueSpriteLaser.prefab or RedSpriteLaser.prefab prefabs into your scene, and assign an optional "Target GameObject"
(set this if you would like the laser to track a target).
2. Configure your desired laser settings/behaviours using the custom laser editor inspector options.
3. If you wish to change the LineBased Laser looks, just assign different materials (included in the Materials folder)
to the Laser and LaserArc GameObjects nested under the main laser GameObject. For sprite based lasers, you will just need to assign the start, middle and end piece sprites.
4. Lasers use raycasts to detect objects that will collide with the beam, you can use the Ignore Raycast layer, or your own custom layer matrix setup to control
which objects will be hit by lasers and which will not. You can alternatively disable all collisions with the laser by checking the "Ignore Collisions" option on the laser component.

More about all the features:
============================

You can subscribe to the OnLaserHitTriggered event on a laser script component to listen for event triggers when the laser collides with another GameObject.
A RaycastHit2D hitinfo object is passed through to the subscribing method, and here you can use the hitinfo object. For example with an enemy laser, 
you might have on your player a method that subscribes to any enemy lasers in the scene. In this method, you could check the hitInfo.collider.gameObject and'
see if this is the same gameObject as your player's gameObject. If it is, then this means that the laser is hitting you, and you could subtract health from the
player. See the 1st demo scene (gauntlet) for an example of this scenario in action.

More help:
----------

Check the tooltips by hovering over the various options in the custom WeaponSystem script inspector - all properties are decorated with detailed information in the tooltips to help explain what they do if you are unsure!

If you have any further questions or need help, please feel free to get in touch with me! My contact e-mail details can be found on the Unity Asset Store page for this asset, or grab me on twitter @shogan85.