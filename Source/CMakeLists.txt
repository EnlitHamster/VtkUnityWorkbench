cmake_minimum_required(VERSION 3.3 FATAL_ERROR)

SET(name VtkUnityWorkbench)

project(VtkUnityWorkbench)

find_package(VTK COMPONENTS 
  vtkCommonColor
  vtkCommonCore
  vtkCommonDataModel
  vtkFiltersSources
  vtkInteractionStyle
  vtkRenderingContextOpenGL2
  vtkRenderingCore
  vtkRenderingFreeType
  vtkRenderingGL2PSOpenGL2
  vtkRenderingOpenGL2
  QUIET
)

if (NOT VTK_FOUND)
  message("Skipping VtkUnityWorkbench: ${VTK_NOT_FOUND_MESSAGE}")
  return ()
endif()
message (STATUS "VTK_VERSION: ${VTK_VERSION}")
if (VTK_VERSION VERSION_LESS "8.90.0")
  # old system
  include(${VTK_USE_FILE})
  add_executable(${name} MACOSX_BUNDLE VtkUnityWorkbenchPlugin.cpp Cone.cxx GL/gl3w.c GL/gl3w.h GL/glcorearb.h GL/khrplatform.h Unity/IUnityGraphics.h Unity/IUnityInterface.h PlatformBase.h VtkUnityWorkbenchAPI.h VtkUnityWorkbenchAPI_OpenGLCoreES.h VtkUnityWorkbenchAPIDefines.h VtkUnityWorkbenchPlugin.h )
  target_link_libraries(${name} PRIVATE ${VTK_LIBRARIES})
else ()
  # include all components
  add_executable(${name} MACOSX_BUNDLE VtkUnityWorkbenchPlugin.cpp Cone.cxx GL/gl3w.c GL/gl3w.h GL/glcorearb.h GL/khrplatform.h Unity/IUnityGraphics.h Unity/IUnityInterface.h PlatformBase.h VtkUnityWorkbenchAPI.h VtkUnityWorkbenchAPI_OpenGLCoreES.h VtkUnityWorkbenchAPIDefines.h VtkUnityWorkbenchPlugin.h )
  target_link_libraries(${name} PRIVATE ${VTK_LIBRARIES})
  # vtk_module_autoinit is needed
  vtk_module_autoinit(
    TARGETS ${name}
    MODULES ${VTK_LIBRARIES}
    )
endif ()
