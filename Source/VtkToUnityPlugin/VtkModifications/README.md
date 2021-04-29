# VtkToUnity Plugin VTK Modifications

This is required to enable Volume Rendering in Unity, other forms of rendering, for instance surface rendering of polygon based objects from an stl file, will work without this modification.

Unfortunately this is not a one button process, requiring a modified version of VTK to work. However, none of the steps are too complicated and we have tried to build as much detail into these instructions as possible.

These instructions work for Windows 10 and Visual Studio as a development environment. In theory the plugin will work on other platforms, but we have not tested it - if you would like to help us with this please get in touch.


## Software Requirements

* **git**
	* This requires git for windows (https://gitforwindows.org/) which has a bash shell
	    * You can use TortoiseGit (https://tortoisegit.org/) for a more Windows integrated experience, but this requires git for windows anyway
	* Follow the instructions on these tools to install them, or use your preferred git tools
* **glew** (for GLUT in CMake below)
	* http://glew.sourceforge.net/
	* Download glew (we have tested with 2.1.0) and unzip it to a convenient folder
	* For example C:\thirdparty\glew\
* **CMake**
	* https://cmake.org/
	* Download and install the latest stable version
* **Visual Studio**
	* https://visualstudio.microsoft.com/
	* Download and install (We are currently using Visual Studio Community 2019)


## Build Instructions

#### Folder structure

To build VTK for the plugin you can use the following folder structure:

```
C:\thirdparty\vtk-git\build         - The build folder I set in CMake
C:\thirdparty\vtk-git\install       - The folder I set as CMAKE_INSTALL_PREFIX in CMake
C:\thirdparty\vtk-git\src\vtk       - This has all of the vtk sources files in it
```

The steps to build the modified version of VTK are... (git commands are provided to be typed in a git bash session. You can alternatively use a GUI version of git to the same end).


#### Clone and Modify the VTK Source Code

* Create the above folders
* `git clone` of the VTK mainline source:
	```
	$ git clone https://gitlab.kitware.com/vtk/vtk.git C:\thirdparty\vtk-git\src\vtk
	```
	* see https://gitlab.kitware.com/vtk/vtk/blob/master/Documentation/dev/git/README.md
* Update to the tag `v8.1.0` 
```
$ cd C:\thirdparty\vtk-git\src\vtk
$ git checkout v8.1.0
$ git submodule update --init
```
* Put the patch file `VtkVolumeRenderingInUnity.patch` in the `/src/vtk` folder
* Apply the patch: 
```
$ git apply VtkVolumeRenderingInUnity.patch
```
* This may cause some warnings about tabs in indents, this is not ideal, but they can be ignored


#### Generate the Visual Studio Project and Solution files

* Run `CMake` and set... 
	* the `source` folder (e.g. `C:\thirdparty\vtk-git\src\vtk\`)
	* the `build` folders (e.g. `C:\thirdparty\vtk-git\build\`)
* Press `Configure` and set...
	* the `compiler` (e.g. `VC15 64bit`) - make sure it is the 64 bit version!
* Wait...
* In `CMake` set...
	* `enable` the Module `Module_vtkRenderingExternal`
	* `CMAKE-INSTALL_PREFIX` (e.g. `C:\thirdparty\vtk-git\install\`)
* Press `Configure` again and wait...
* In `CMake` set...
	* `GLUT_INCLUDE_DIR` (e.g `C:\thirdparty\glew-2.1.0\include\`)
	* `GLUT_glut_LIBRARY` (e.g. `C:\thirdparty\glew-2.1.0\lib\Release\x64\glew32.dll`)
* Press `Configure` a last time and wait...
* Press `Generate`
	* This will create all of the project and solution files in the `build` folder


#### Build VTK in Visual Studio

* Build `VTK` in `Visual Studio`
	* `Open` up the `solution` file (e.g. `C:\thirdparty\vtk-git\build\VTK.sln`)
	* Select the `Solution Configuration` (e.g. Debug or Release - just `Release`)
	* `Build ALL_BUILD` (e.g. by `right click` on `ALL-BUILD` project and select `Build`)
* Wait...
	* `Install` the `VTK` libraries (e.g. by `right click` on `INSTALL` project and select `Build`)


#### Final Words

That's great - VTK has now been build, that is the difficult part over.

Some notes for advanced users at this point...

* The `INSTALL` only needs to be done for one configuration as it is only needed to collect together the header files, the dlls and libs are all pulled from the regular `ALL_BUILD` output, but while this puts the dlls and libs in the bin and libs folder it leaves the h files all over the place
* You can also build the tests in VTK - currently the modification causes tests 1088, 1259-1265, 1268 and 1271 to fail - unsurprisingly they are all associated with volume rendering. When we have time we will work to fix the code so that these tests pass

## License

The license for the the modifications to VTK is MIT.

VTK is distributed under the OSI-approved BSD 3-clause License.
See [Copyright.txt][] for details.

[Copyright.txt]: Copyright.txt
