# All-In-One VR visualization workbench

This is the main repository for the code from the my Master Thesis. Other than me, a number of BSc students have made their theses based on this and I will try to keep the list updated with the different updates, new features and toolkit components that are added.

This Unity plugin is composed of a underlying architecture that connects VTK and Unity using a shared OpenGL context (see Wheeler et al.'s plugin on which this is based [here](https://gitlab.com/3dheart_public/vtktounity)) and a toolbox of UI components that are called for interactive manipulation through introspection on the VTK classes. In particular, these components are to be compounded in order to create a modular UI where the data for the VTK pipelines can be inserted.

If you want to contribute, there a number of parts where you could do so. If you are more interested in the UI part of the plugin, you can develop or refine the UI components in the *Toolbox*. If you want to enhance or expand the architecture you are free to do so. You can fork this repository but we ask you to open a pull request when you are at a stable stage in your development so we can maintain this tool updated and running rather than fragmenting it in numerous repositories.

## Installation

### Requirements

- CMake, preferably 3.14.X (https://cmake.org/files/v3.14/)
- Python, required 3.7.X, suggested 3.7.8 (https://www.python.org/downloads/release/python-378/)
- Visual Studio 2017 and Windows SDK
- Windows 64bit

### Setup

- Create a folder as you please that will be you workspace (e.g. `C:/Users/<your-username>/Documents/VtkUnity/`) referred to from here on as `%WORKSPACE%`
- Inside `%WORKSPACE%` create two folders, `Vtk` and `VtkUnityWorkbench`

### Building VTK

- Inside `%WORKSPACE%/Vtk/` create the folders `source`, `build` and `install`
- Open a console or git bash inside `%WORKSPACE%/Vtk/` (you can also use a git UI client for these steps)
- Clone VTK's repository as follows: `git clone https://github.com/Kitware/VTK.git source`
- Checkout VTK version 8.1.2 (we suggest this version as the software was tested with it): `git checkout v8.1.2`
- Open CMake and set the source directory to `%WORKSPACE%/Vtk/source` and the build directory to `%WORKSPACE%/Vtk/build`
- Click on `Configure` and select Visual Studio 2017 compiler, with architecture x64, and wait till the configuration ends
- Check `Advanced` in the top bar
- Set `CMAKE_INSTALL_PREFIX` to `%WORKSPACE%/Vtk/install`
- Enable the following modules
  - `VTK_MODULE_PYTHON`
  - `VTK_MODULE_RENDERING_EXTERNAL`
  - `VTK_MODULE_WRAPPING_PYTHON`
  - `VTK_MODULE_WRAPPING_CORE`
  - `VTK_MODULE_WRAPPING_TOOLS`
