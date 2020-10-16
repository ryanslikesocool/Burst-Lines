# Burst Lines
![Burst Lines banner image](images/ShapesBanner.png) 
###### A small line rendering tool built around Unity's Job system

## Features
- Immediate mode and retained mode mesh rendering
- Super speedy job-based mesh creation (only when data changes, so no unnecessary calculation)
- Stored in scriptable objects for easy reuse
- Pixel width or quad-based shapes
- URP compatible (not tested on HDRP or classic but it should work)
- Miter angles (nice corners)
- Arcs
- Color modes (fill, per point, per vertex, gradient)
- Color blending modes (step, mix)

## Dependencies
#### This package was created around Unity's Jobs and Burst compiler systems.  Because of this, there are a few dependencies that are required.
All of the dependencies can be found in the Unity Package Manager
- Jobs
- Burst
- Mathematics
- Collections

Installing the Jobs package should immediately install the other packages as dependencies

## Usage
- Drop the Burst Lines folder into your Unity project's Plugins folder
- Add a Shape Renderer component to an object
- Create a shape Scriptable Object (`Project panel`>`right click`>`Create`>`Burst Lines`)
- Set the Shape Renderer and Scriptable Object properties
- Attach the Scriptable Object to the Shape Renderer
- :sparkles: Magic :sparkles:

There is also a sample scene (`Burst Lines/Samples/Sample.unity`) with a basic setup to help explain

![Shapes sample image](images/ShapesSample.png) 
