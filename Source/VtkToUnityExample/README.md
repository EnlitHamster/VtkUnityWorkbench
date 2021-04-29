# VtkToUnity Example

## Setting VR or not VR

To set the rendering to use the computer screen or the VR headset:

1. Open `Unity->Edit->Project Settings...`
2. Select `Player` settings
3. Select `XR Settings`
4. Then for...
	* `VR` make `OpenVR` the first option
	* `Computer Screen` make `None` the first option

The first time you run the VR example Unity may not have the OpenVR API. This can be installed by:

1. Open `Unity->Window->Packages`
2. Select `OpenVR (Desktop)`
3. Install this package


## Minimal Volume Render Scene

This is a simple example of volume rendering using VTK in a Unity scene. It mixes surface rendered spheres (in both Unity and VTK) mixed with a volume render.


## Example VR Volume Render Scene

This has both a volume render and MPR the user can interact with. 

* To 'pick up' the volume render, MPR or cropping plane put the blue cross on/in the virtual object and use the trigger on the controller to grab it. 
* To place the volume in front of you press the menu button on the left hand controller. 
* Contrast and gain can be adjusted on the track ad of the right hand controller.
* Data, animation (only for mhd data) transfer function can be altered on the menu panel.

The best thing to do is experiment - enjoy!


# Sources

This is an example of using the VtkToUnity plugin based on the UNity native plugin example. For developing our HTC Vive interaction, we found the following tutorial a good place to start:

https://www.raywenderlich.com/9189-htc-vive-tutorial-for-unity

The GenericSingletonClass.cs was published on this blog 

http://www.unitygeek.com/unity_c_singleton/
