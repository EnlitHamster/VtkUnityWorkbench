/*=========================================================================

  Program:   Visualization Toolkit
  Module:    vtkExternalOpenGLRenderer3dh.h

  Copyright (c) Ken Martin, Will Schroeder, Bill Lorensen
  All rights reserved.
  See Copyright.txt or http://www.kitware.com/Copyright.htm for details.

     This software is distributed WITHOUT ANY WARRANTY; without even
     the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
     PURPOSE.  See the above copyright notice for more information.

=========================================================================*/
/**
 * @class   vtkExternalOpenGLRenderer3dh
 * @brief   OpenGL renderer
 *
 * vtkExternalOpenGLRenderer3dh is a secondary implementation of the class
 * vtkOpenGLRenderer. vtkExternalOpenGLRenderer3dh interfaces to the
 * OpenGL graphics library. This class provides API to preserve the color and
 * depth buffers, thereby allowing external applications to manage the OpenGL
 * buffers. This becomes very useful when there are multiple OpenGL applications
 * sharing the same OpenGL context.
 *
 * vtkExternalOpenGLRenderer3dh makes sure that the camera used in the scene if of
 * type vtkExternalOpenGLCamera. It manages light and camera transformations for
 * VTK objects in the OpenGL context.
 *
 * \sa vtkExternalOpenGLCamera
*/

#ifndef vtkExternalOpenGLRenderer3dh_h
#define vtkExternalOpenGLRenderer3dh_h

#include "vtkRenderingExternalModule.h" // For export macro
#include "vtkOpenGLRenderer.h"

#include <array>

// Forward declarations
class vtkLightCollection;
class vtkExternalLight;

class vtkExternalOpenGLRenderer3dh :
  public vtkOpenGLRenderer
{
public:
  static vtkExternalOpenGLRenderer3dh *New();
  vtkTypeMacro(vtkExternalOpenGLRenderer3dh, vtkOpenGLRenderer);
  void PrintSelf(ostream& os, vtkIndent indent) VTK_OVERRIDE;

  // Set the View matrix (OpenGL style column major array)
  void SetViewMatrix(const std::array<double, 16> & viewMatrix);
  // Set the Projection matrix (OpenGL style column major array)
  void SetProjectionMatrix(const std::array<double, 16> & projectionMatrix);

  /**
   * Synchronize camera and light parameters
   */
  void Render(void) VTK_OVERRIDE;

  ///**
  // * Create a new Camera sutible for use with this type of Renderer.
  // * This function creates the vtkExternalOpenGLCamera.
  // */
  vtkCamera* MakeCamera() VTK_OVERRIDE;

  ///**
  // * Add an external light to the list of external lights.
  // */
  virtual void AddExternalLight(vtkExternalLight *);

  ///**
  // * Remove an external light from the list of external lights.
  // */
  virtual void RemoveExternalLight(vtkExternalLight *);

  ///**
  // * Remove all external lights
  // */
  virtual void RemoveAllExternalLights();


protected:
  vtkExternalOpenGLRenderer3dh();
  ~vtkExternalOpenGLRenderer3dh() VTK_OVERRIDE;

  vtkLightCollection *ExternalLights;
  std::array<double, 16> ViewMatrixArray;
  std::array<double, 16> ProjectionMatrixArray;

private:
  vtkExternalOpenGLRenderer3dh(const vtkExternalOpenGLRenderer3dh&) VTK_DELETE_FUNCTION;
  void operator=(const vtkExternalOpenGLRenderer3dh&) VTK_DELETE_FUNCTION;
};

#endif //vtkExternalOpenGLRenderer3dh_h
