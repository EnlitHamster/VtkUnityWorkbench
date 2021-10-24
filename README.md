# All-In-One VR visualization workbench

This is the main repository for the code from the my Master Thesis. Other than me, a number of BSc students have made their theses based on this and I will try to keep the list updated with the different updates, new features and toolkit components that are added.

This Unity plugin is composed of a underlying architecture that connects VTK and Unity using a shared OpenGL context (see Wheeler et al.'s plugin on which this is based [here](https://gitlab.com/3dheart_public/vtktounity)) and a toolbox of UI components that are called for interactive manipulation through introspection on the VTK classes. In particular, these components are to be compounded in order to create a modular UI where the data for the VTK pipelines can be inserted.

If you want to contribute, there a number of parts where you could do so. If you are more interested in the UI part of the plugin, you can develop or refine the UI components in the *Toolbox*. If you want to enhance or expand the architecture you are free to do so. You can fork this repository but we ask you to open a pull request when you are at a stable stage in your development so we can maintain this tool updated and running rather than fragmenting it in numerous repositories.

## Installation

### Requirements

- CMake, preferably 3.14.X (https://cmake.org/files/v3.14/)
- Python, required 3.7.X, recommended 3.7.8 (https://www.python.org/downloads/release/python-378/)
- Visual Studio 15 2017 and Windows SDK
- Windows 64bit, recommended Windows 10
- Unity Hub 2 at least

### Setup

- Create a folder as you please that will be you workspace (e.g. `C:/Users/<your-username>/Documents/VtkUnity/`) referred to from here on as `%WORKSPACE%`
- Inside `%WORKSPACE%` create two folders, `Vtk` and `VtkUnityWorkbench`

### Building VTK

- Inside `%WORKSPACE%/Vtk/` create the folders `Source`, `Build` and `Install`
- Open a console or git bash inside `%WORKSPACE%/Vtk/` (you can also use a git UI client for these steps)
- Clone VTK's repository as follows: `git clone https://github.com/Kitware/VTK.git source`
- Checkout VTK version 8.1.2 (we suggest this version as the software was tested with it): `git checkout v8.1.2`
- Initialize the repository's submodules: `git submodule update --init`
- Open CMake and set the source directory to `%WORKSPACE%/Vtk/source` and the build directory to `%WORKSPACE%/Vtk/Build`
- Click on `Configure` and select *Visual Studio 15 2017* generator, with platform *x64*, click `Finish` and wait till the configuration ends
- Check `Advanced` in the top bar
- Set `CMAKE_INSTALL_PREFIX` to `%WORKSPACE%/Vtk/Install`
- Set the `INSTALL_*_DIR` to `%WORKSPACE%/Vtk/Install/*` for each of the entries
- Enable the following modules
  - `Module_VtkPython`
  - `Module_VtkRenderingExternal`
  - `Module_VtkWrappingPythonCore`
  - `Module_VtkWrappingTools`
- Make sure `PYTHON_EXECUTABLE` points to the right `python.exe` file (i.e. Python 3.7.X)
- Set `VTK_PYTHON_VERSION` to `3.7`
- Check `VTK_WRAP_PYTHON`
- Click `Configure` and wait for CMake to finish
- Make sure the `PYTHON_*` point to the right Python 3.7.X files and directories
- Click `Configure` and wait for CMake to finish
- Check `VTK_ENABLE_VTKPYTHON`
- Click `Configure` and wait for CMake to finish
- Click `Generate` and wait for CMake to finish
- Open `%WORKSPACE%/Vtk/Build/VTK.sln` with Visual Studio 2017
- Make sure the build configuration is `Release x64`
- In the solution explorer
  - Open the *CMakePredefinedTargets* filter
  - Right click on *ALL_BUILD* and run *Build* and wait for the build to finish
  - Right click on *INSTALL* and run *Project only > Build INSTALL only*

The compilation of the VTK library could take up to two hours depending on the performances of your computer. If it takes longer than that, we suggesat you switch to a more powerful workstation for ease of development, as otherwise development could become very difficult and stressful.

### Building VtkUnityWorkbench

- Open a console or git bash inside `%WORKSPACE%/VtkUnityWorkbench/` (you can also use a git UI client for these steps)
- Clone VtkUnityWorknech's repository as follows: `git clone https://github.com/EnlitHamster/VtkUnityWorkbench.git VtkUnityWorkbench`
- Initialize the repository's submodules: `git submodule update --init`
- From here on, we will refer to `%WORKSPACE%/VtkUnityWorkbench/Source/VtkToUnityPlugin/` as `%PLUGINDIR%`
- Inside `%PLUGINDIR` create the folders `Build` and `Install`
- Open CMake and set the source directory to `%PLUGINDIR%/source` and the build directory to `%PLUGINDIR%/Build`
- Click on `Configure` and select *Visual Studio 15 2017* generator, with platform *x64*, click `Finish` and wait till the configuration ends
- Check `Advanced` in the top bar
- Set `MY_OWN_INSTALL_PREFIX` to `%PLUGINDIR%/Install`
- Make sure the `PYTHON_*` point to the right Python 3.7.X files and directories
- Set `VTK_DIR` to `%WORKSPACE%/Vtk/Build`
- Click `Configure` and wait for CMake to finish
- Click `Generate` and wait for CMake to finish
- Open `%PLUGINDIR%/Build/VtkToUnityPlugin.sln` with Visual Studio 2017
- Make sure the build configuration is `Release x64`
- In the solution explorer
  - Open the *CMakePredefinedTargets* filter
  - Right click on *ALL_BUILD* and run *Build*
  - Right click on *INSTALL* and run *Project only > Build INSTALL only*

At the end of this procedure, you should have some main crucial files:
- `%WORKSPACE%/Vtk/Install/bin` should now contain the DLL files for VTK C++
- `%WORKSPACE%/Vtk/Install/lib/python3.7/site-packages` should contain the pyc files for VTK Python
- `%PLUGINDIR%/Install` should now contain the DLL file and the build files for VtkUnityWorkbench
- `%PYTHONDIR%`, the install directory of Python 3.7, should contain `python37.dll` and `python37_d.dll`

### Setting up for Unity

First of all, we need now to install VTK Python somewhere accessible to Python. We strongly recommend doing this installing it in the global Python install directory `%PYTHONDIR%/Lib/site-packages`, by copying in there `%WORKSPACE%/Vtk/Install/lib/python3.7/site-packages/vtk`, and then the contents of `%WORKSPACE%/Vtk/Install/bin` inside `%PYTHONDIR%/Lib/site-packages/vtk` (or to any place within the PATH environmental variable, the important thing is that Python must be able to access the DLL files). To test whether this works, run a command line terminal, open the Python interpreter `python` and run `import vtk`. If it returns no error, VTK Python should be correctly installed.

**At the moment, VtkUnityWorkbench does not support virtual environments, but it is a feature which we plan to introduce soon**.

From here on we will refer to `%WORKSPACE%/VtkUnityWorkbench/Source/VtkToUnityExample/` as `%UNITYDIR%`

Copy the contents ot `%WORKSPACE%/Vtk/Install/bin` and `%PLUGINDIR%/Install`, and `%PYTHONDIR%/python37.dll` and `%PYTHONDIR%/python37_d.dll` into `%UNITYDIR%/Assets/VtkToUnity/vtktounitybinary/Plugins/x86_64/`.

Add `%UNITYDIR%` to your Unity Hub projects and download the correct Unity version. This should be enough to have your setup ready to go.
