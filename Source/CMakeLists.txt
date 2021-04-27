cmake_minimum_required(VERSION 3.3 FATAL_ERROR)

set(PROJECT_NAME VtkUnityWorkbenchPlugin)
# Output Variables
set(OUTPUT_DEBUG ../Build)
set(OUTPUT_REL ../Build)
# Folders files
set(GLEW_DIR ../Source/GL)
set(CPP_DIR ../Source)
set(INITIALISATION_DIR ../Source/Initialisation)
set(HEADER_DIR_1 ../Source)
set(HEADER_DIR_2 ../Source/GL)
set(HEADER_DIR_3 ../Source/Unity)



############## CMake Project ################
#        The main options of project        #
#############################################

project(${PROJECT_NAME} CXX)


# Definition of Macros
add_definitions(
    -DGLEW_STATIC 
    -D_WINDOWS 
    -D_USRDLL 
    -DRENDERINGPLUGIN_EXPORTS 
    -DUNICODE
    -D_UNICODE
)

# Define VTK directory
find_package(VTK REQUIRED)
include(${VTK_USE_FILE})


# Add vtk headers and lib directory and change their backslash to forward slash in Windows
if(MSVC AND VTK_DIR)
    string(REPLACE "\\" "/" VTK_DIR ${VTK_DIR})
endif()

############## Artefacts Output #################
# Defines outputs , depending Debug or Release. #
#################################################

if(CMAKE_BUILD_TYPE STREQUAL "Debug")
  set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_DEBUG}")
  set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_DEBUG}")
  set(CMAKE_EXECUTABLE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_DEBUG}")
else()
  set(CMAKE_LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_REL}")
  set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_REL}")
  set(CMAKE_EXECUTABLE_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/${OUTPUT_REL}")
endif()

# Set install path, which should be Unity project's plugin folder
SET(MY_OWN_INSTALL_PREFIX "${CMAKE_SOURCE_DIR}/../Bin" CACHE PATH "Prefix prepended to install directories")
SET(CMAKE_INSTALL_PREFIX "${MY_OWN_INSTALL_PREFIX}" CACHE INTERNAL "Prefix prepended to install directories" FORCE)
# Set Unity plugin path
if(CMAKE_SIZEOF_VOID_P EQUAL 8)
	# 64 bits
	set(UNITY_PLUGIN_PATH ${CMAKE_INSTALL_PREFIX}/x86_64)
elseif(CMAKE_SIZEOF_VOID_P EQUAL 4)
	# 32 bits
	set(UNITY_PLUGIN_PATH ${CMAKE_INSTALL_PREFIX}/x86)
endif()	


################# Flags ################
# Defines Flags for Windows and Linux. #
########################################

if(NOT MSVC)
   set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
   if ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "Clang")
       set (CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -stdlib=libc++")
   endif()
endif(NOT MSVC)


################ Files ################
#   --   Add files to project.   --   #
#######################################
SET_SOURCE_FILES_PROPERTIES( ${GLEW_DIR}/glew.c PROPERTIES LANGUAGE CXX )

file(GLOB SRC_FILES
    ${GLEW_DIR}/*.c
    ${CPP_DIR}/*.cpp
    ${INITIALISATION_DIR}/*.cpp
    ${HEADER_DIR_1}/*.h
    ${HEADER_DIR_2}/*.h
    ${HEADER_DIR_3}/*.h
    ${INITIALISATION_DIR}/*.h
    ${VTK_INCLUDE_DIR}/**/*.h
)

# Add library to build.
add_library(${PROJECT_NAME} SHARED
   ${SRC_FILES}
)

set (VTK_REQUIRED_LIBS
    vtkCommonColor-8.2.dll
    vtkCommonCore-8.2.dll
    vtkCommonDataModel-8.2.dll
    vtkFiltersSources-8.2.dll
    vtkInteractionStyle-8.2.dll
    vtkRenderingContextOpenGL2-8.2.dll
    vtkRenderingCore-8.2.dll
    vtkRenderingFreeType-8.2.dll
    vtkRenderingGL2PSOpenGL2-8.2.dll
    vtkRenderingOpenGL2-8.2.dll
    vtkRenderingExternal-8.2.dll
)

# Add post build events to Windows
if(MSVC)
    # Separating in a number of commands to determine potential bugs and issues
    add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
        COMMAND del /Q \"${UNITY_PLUGIN_PATH}/*.dll\"

        COMMAND copy /Y \"$(TargetPath)\" \"${UNITY_PLUGIN_PATH}/$(TargetFileName)\"
        COMMAND robocopy \"C:/Program Files/VTK/bin/\" \"${UNITY_PLUGIN_PATH}/\" ${VTK_REQUIRED_LIBS} /NFL /NDL /NJH /NJS /nc /ns /np \n if %errorlevel% leq 1 set errorlevel = 0
    )
endif()


if(MSVC)
   target_link_libraries(${PROJECT_NAME} opengl32.lib 
     ${VTK_LIBRARIES})
endif(MSVC)

# Install generated library to target directory
if(MSVC)
    # Windows install to VTK bin
    install(TARGETS ${PROJECT_NAME} DESTINATION ${VTK_DIR}/bin/)
else()
    # Otherwise install to UnityProject/Assets/Plugin/
    install(TARGETS ${PROJECT_NAME} DESTINATION ${UNITY_PLUGIN_PATH})
endif()