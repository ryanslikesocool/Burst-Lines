# Shapes
![Shapes banner image](images/ShapesBanner.png) 
###### A small line rendering tool built around Unity's Job system

## Features
- Immediate mode and retained mode mesh rendering (retained need improvements)
- Super speedy job-based mesh creation
- Store shapes data in scriptable objects for easy reuse
- Pixel width or quad-based shapes
- URP compatible (not tested on HDRP but it might work)
- Arcs
- Miter angles for non-arc shapes

## Dependencies
#### This package is based around Unity's Jobs and Burst compiler systems.  Because of this, there are a few dependencies that are required.
All of the dependencies can be found in the Unity Package Manager
- Jobs
- Burst
- Mathematics
- Collections

Installing the Jobs package should immediately install the other packages as dependencies

## Usage
- Drop the Shapes folder (root/Assets/_Scripts/Shapes) into your Unity project's Plugins folder
- Add a Shape Renderer component to an object
- Create a shape Scriptable Object (Project panel > right click > Create > ifelse > Shapes)
- Set Scriptable Object Properties
- Attach Scriptable Object to Shape Renderer
- :sparkles: Magic :sparkles:

There is also a sample scene (root/Assets/_Scenes/Testing) with a basic setup to help explain

![Shapes sample image](images/ShapesSample.png) 
