# VtkToUnity Plugin Build Instructions

If you use this software, please kindly cite our work:

* [Gavin Wheeler, Shujie Deng, Nicolas Toussaint, Kuberan Pushparajah, Julia A. Schnabel, John M. Simpson, and Alberto Gomez. *"Virtual interaction and visualisation of 3D medical imaging data with VTK and Unity."* Healthcare technology letters 5, no. 5 (2018): 148-153.](https://ieeexplore.ieee.org/abstract/document/8527762)

## Software Requirements/Pre-requisits

* **VTK**
    * See these [instructions](VtkModifications) to download and build VTK
	* CMake, Visual Studio and git are also required from there
* **Unity**
	* https://unity.com/
	* Download and install the latest version

## Build Instructions

#### Make the project file for the plugin are

* Add a folder `build` to `.\VtkToUnityPlugin` folder
* Start `CMake` 
	* Set the folder you just created as the `build folder` 
	* Set `.\VtkToUnityPlugin\source` as the `source folder`
* Click `Configure`, and when selecting the compiler choose the 64bit version.
	* There will be an error
* Set the `vtk build folder` from above as `VTK-DIR`
* Click `Configure`
	* There will be new item in red, but this is not a problem
* Click `Configure`
	* Now there should be no error
* Click `Generate`

#### Build the plugin

* Open (**as Administrator**)  `VtkToUnityPlugin.sln` in `\UnityNativeVtkTestPlugin\VtkToUnityPlugin\build`
* `Build` the plugin, in `Release` format for maximum performance

### Acknowledgment

This work was supported by the NIHR i4i funded 3D Heart project [II-LA-0716-20001]. This work was also supported by the Wellcome/EPSRC Centre for Medical Engineering [WT 203148/Z/16/Z]. The research was funded/supported by the National Institute for Health Research (NIHR) Biomedical Research Centre based at Guy's and St Thomas' NHS Foundation Trust and King's College London and supported by the NIHR Clinical Research Facility (CRF) at Guy's and St Thomas'. The views expressed are those of the author(s) and not necessarily those of the NHS, the NIHR or the Department of Health.

