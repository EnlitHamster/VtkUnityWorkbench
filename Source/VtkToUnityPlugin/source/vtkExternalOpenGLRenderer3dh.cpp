/*=========================================================================

  Program:   Visualization Toolkit
  Module:    vtkExternalOpenGLRenderer3dh.cxx

  Copyright (c) Ken Martin, Will Schroeder, Bill Lorensen
  All rights reserved.
  See Copyright.txt or http://www.kitware.com/Copyright.htm for details.

     This software is distributed WITHOUT ANY WARRANTY; without even
     the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
     PURPOSE.  See the above copyright notice for more information.

=========================================================================*/
#include "vtkExternalOpenGLRenderer3dh.h"

#include "vtkCamera.h"
#include "vtkCommand.h"
#include "vtkExternalLight.h"
#include "vtkExternalOpenGLCamera.h"
#include "vtkLightCollection.h"
#include "vtkLightCollection.h"
#include "vtkLight.h"
#include "vtkMath.h"
#include "vtkMatrix4x4.h"
#include "vtkNew.h"
#include "vtkObjectFactory.h"
#include "vtkOpenGLError.h"
#include "vtkOpenGL.h"
#include "vtkRenderWindow.h"
#include "vtkTexture.h"

#define MAX_LIGHTS 8

vtkStandardNewMacro(vtkExternalOpenGLRenderer3dh);

//----------------------------------------------------------------------------
vtkExternalOpenGLRenderer3dh::vtkExternalOpenGLRenderer3dh()
{
  this->PreserveColorBuffer = 1;
  this->PreserveDepthBuffer = 1;
  this->SetAutomaticLightCreation(0);
  this->ExternalLights = vtkLightCollection::New();

  // This is a fairly dumb way of doing this but 
  this->ViewMatrixArray.fill(0.0);
  this->ViewMatrixArray[0] = 1.0;
  this->ViewMatrixArray[5] = 1.0;
  this->ViewMatrixArray[10] = 1.0;
  this->ViewMatrixArray[15] = 1.0;

  this->ProjectionMatrixArray.fill(0.0);
  this->ProjectionMatrixArray[0] = 1.0;
  this->ProjectionMatrixArray[5] = 1.0;
  this->ProjectionMatrixArray[10] = 1.0;
  this->ProjectionMatrixArray[15] = 1.0;
}

//----------------------------------------------------------------------------
vtkExternalOpenGLRenderer3dh::~vtkExternalOpenGLRenderer3dh()
{
  this->ExternalLights->Delete();
  this->ExternalLights = NULL;
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::SetViewMatrix(const std::array<double, 16> &viewMatrix)
{
  // This should be in OpenGL style column major format
  this->ViewMatrixArray = viewMatrix;
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::SetProjectionMatrix(const std::array<double, 16> &projectionMatrix)
{
  // This should be in OpenGL style column major format
  this->ProjectionMatrixArray = projectionMatrix;
}


//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::Render(void)
{
  // OpenGL doesn't seem to pick the camera up from Unity, which VTK expects
  // However, setting it here seems to work OK
  // Actually now it seems to be working without them, 
  // and they are outdated functions too which cause errors
  //{
  //  glMatrixMode(GL_MODELVIEW_MATRIX);
  //  glLoadMatrixd(this->ViewMatrixArray.data());

  //  glMatrixMode(GL_PROJECTION);
  //  glLoadMatrixd(this->ProjectionMatrixArray.data());
  //}

  // This is from the orginal code, but even after the above
  // still seems to get identity matrices, so we'll skip this
  //GLdouble mv[16],p[16];
  //glGetDoublev(GL_MODELVIEW_MATRIX,mv);
  //glGetDoublev(GL_PROJECTION_MATRIX,p);

  vtkExternalOpenGLCamera* camera = vtkExternalOpenGLCamera::SafeDownCast(
    this->GetActiveCameraAndResetIfCreated());

  // Set the matrices we've passed in rather than the OpenGl ones in the camera
  //camera->SetProjectionTransformMatrix(p);
  //camera->SetViewTransformMatrix(mv);
  camera->SetProjectionTransformMatrix(this->ProjectionMatrixArray.data());
  camera->SetViewTransformMatrix(this->ViewMatrixArray.data());

  // use the view matrix we've passed in rather than the one we're not obtaining from OpenGL
  vtkMatrix4x4* matrix = vtkMatrix4x4::New();
  //matrix->DeepCopy(mv);
  matrix->DeepCopy(this->ViewMatrixArray.data());
  matrix->Transpose();
  matrix->Invert();

  // Synchronize camera viewUp
  double viewUp[4] = {0.0, 1.0, 0.0, 0.0}, newViewUp[4];
  matrix->MultiplyPoint(viewUp, newViewUp);
  vtkMath::Normalize(newViewUp);
  camera->SetViewUp(newViewUp);

  // Synchronize camera position
  double position[4] = {0.0, 0.0, 0.0, 1.0}, newPosition[4];
  matrix->MultiplyPoint(position, newPosition);

  if (newPosition[3] != 0.0)
  {
    newPosition[0] /= newPosition[3];
    newPosition[1] /= newPosition[3];
    newPosition[2] /= newPosition[3];
    newPosition[3] = 1.0;
  }
  camera->SetPosition(newPosition);

  // Synchronize focal point
  double focalPoint[4] = {0.0, 0.0, -1.0, 1.0}, newFocalPoint[4];
  matrix->MultiplyPoint(focalPoint, newFocalPoint);
  camera->SetFocalPoint(newFocalPoint);

  matrix->Delete();

  //// Lights
  //// Query lights existing in the external context
  //// and tweak them based on vtkExternalLight objects added by the user
  //GLenum curLight;
  //for (curLight = GL_LIGHT0;
  //     curLight < GL_LIGHT0 + MAX_LIGHTS;
  //     curLight++)
  //{
  //  GLboolean status;
  //  glGetBooleanv(curLight, &status);

  //  int l_ind = static_cast<int> (curLight - GL_LIGHT0);
  //  bool light_created = false;
  //  vtkLight* light = vtkLight::SafeDownCast(
  //                      this->GetLights()->GetItemAsObject(l_ind));
  //  if (light)
  //  {
  //    if (!status)
  //    {
  //      // This is the case when we have a VTK light in the scene but no
  //      // external light corresponding to that index in the context.
  //      // In this case, we remove the VTK light as well.
  //      light->SwitchOff();
  //      this->RemoveLight(light);

  //      // No need to go forward
  //      continue;
  //    }
  //  }
  //  else
  //  {
  //    // No matching light found in the VTK light collection
  //    if (status)
  //    {
  //      // Create a new light only if one is present in the external context
  //      light = vtkLight::New();
  //      // Headlight because VTK will apply transform matrices
  //      light->SetLightTypeToHeadlight();
  //      light_created = true;
  //    }
  //    else
  //    {
  //      // No need to go forward as this light is not being used
  //      continue;
  //    }
  //  }

  //  // Find out if there is an external light object associated with this
  //  // light index.
  //  vtkCollectionSimpleIterator sit;
  //  vtkExternalLight* eLight;
  //  vtkExternalLight* curExtLight = NULL;
  //  for (this->ExternalLights->InitTraversal(sit);
  //       (eLight = vtkExternalLight::SafeDownCast(
  //        this->ExternalLights->GetNextLight(sit))); )
  //  {
  //    if (eLight &&
  //        (static_cast<GLenum>(eLight->GetLightIndex()) == curLight))
  //    {
  //      curExtLight = eLight;
  //      break;
  //    }
  //  }

  //  if (curExtLight &&
  //      (curExtLight->GetReplaceMode() == vtkExternalLight::ALL_PARAMS))
  //  {
  //    // If the replace mode is all parameters, blatantly overwrite the
  //    // parameters of existing/new light
  //    light->DeepCopy(curExtLight);
  //  }
  //  else
  //  {

  //    GLfloat info[4];

  //    // Set color parameters
  //    if (curExtLight && curExtLight->GetIntensitySet())
  //    {
  //      light->SetIntensity(curExtLight->GetIntensity());
  //    }

  //    if (curExtLight && curExtLight->GetAmbientColorSet())
  //    {
  //      light->SetAmbientColor(curExtLight->GetAmbientColor());
  //    }
  //    else
  //    {
  //      glGetLightfv(curLight, GL_AMBIENT, info);
  //      light->SetAmbientColor(info[0], info[1], info[2]);
  //    }
  //    if (curExtLight && curExtLight->GetDiffuseColorSet())
  //    {
  //      light->SetDiffuseColor(curExtLight->GetDiffuseColor());
  //    }
  //    else
  //    {
  //      glGetLightfv(curLight, GL_DIFFUSE, info);
  //      light->SetDiffuseColor(info[0], info[1], info[2]);
  //    }
  //    if (curExtLight && curExtLight->GetSpecularColorSet())
  //    {
  //      light->SetSpecularColor(curExtLight->GetSpecularColor());
  //    }
  //    else
  //    {
  //      glGetLightfv(curLight, GL_SPECULAR, info);
  //      light->SetSpecularColor(info[0], info[1], info[2]);
  //    }

  //    // Position, focal point and positional
  //    glGetLightfv(curLight, GL_POSITION, info);

  //    if (curExtLight && curExtLight->GetPositionalSet())
  //    {
  //      light->SetPositional(curExtLight->GetPositional());
  //    }
  //    else
  //    {
  //      light->SetPositional(info[3] > 0.0 ? 1 : 0);
  //    }

  //    if (!light->GetPositional())
  //    {
  //      if (curExtLight && curExtLight->GetFocalPointSet())
  //      {
  //        light->SetFocalPoint(curExtLight->GetFocalPoint());
  //        if (curExtLight->GetPositionSet())
  //        {
  //          light->SetPosition(curExtLight->GetPosition());
  //        }
  //        else
  //        {
  //          light->SetPosition(info[0], info[1], info[2]);
  //        }
  //      }
  //      else
  //      {
  //        light->SetFocalPoint(0, 0, 0);
  //        if (curExtLight && curExtLight->GetPositionSet())
  //        {
  //          light->SetPosition(curExtLight->GetPosition());
  //        }
  //        else
  //        {
  //          light->SetPosition(-info[0], -info[1], -info[2]);
  //        }
  //      }
  //    }
  //    else
  //    {
  //      if (curExtLight && curExtLight->GetPositionSet())
  //      {
  //        light->SetPosition(curExtLight->GetPosition());
  //      }
  //      else
  //      {
  //        light->SetPosition(info[0], info[1], info[2]);
  //      }

  //      // Attenuation
  //      if (curExtLight && curExtLight->GetAttenuationValuesSet())
  //      {
  //        light->SetAttenuationValues(curExtLight->GetAttenuationValues());
  //      }
  //      else
  //      {
  //        glGetLightfv(curLight, GL_CONSTANT_ATTENUATION, &info[0]);
  //        glGetLightfv(curLight, GL_LINEAR_ATTENUATION, &info[1]);
  //        glGetLightfv(curLight, GL_QUADRATIC_ATTENUATION, &info[2]);
  //        light->SetAttenuationValues(info[0], info[1], info[2]);
  //      }

  //      // Cutoff
  //      if (curExtLight && curExtLight->GetConeAngleSet())
  //      {
  //        light->SetConeAngle(curExtLight->GetConeAngle());
  //      }
  //      else
  //      {
  //        glGetLightfv(curLight, GL_SPOT_CUTOFF, &info[0]);
  //        light->SetConeAngle(info[0]);
  //      }

  //      if (light->GetConeAngle() < 180.0)
  //      {
  //        // Exponent
  //        if (curExtLight && curExtLight->GetExponentSet())
  //        {
  //          light->SetExponent(curExtLight->GetExponent());
  //        }
  //        else
  //        {
  //          glGetLightfv(curLight, GL_SPOT_EXPONENT, &info[0]);
  //          light->SetExponent(info[0]);
  //        }

  //        // Direction
  //        if (curExtLight && curExtLight->GetFocalPointSet())
  //        {
  //          light->SetFocalPoint(curExtLight->GetFocalPoint());
  //        }
  //        else
  //        {
  //          glGetLightfv(curLight, GL_SPOT_DIRECTION, info);
  //          for (unsigned int i = 0; i < 3; ++i)
  //          {
  //            info[i] += light->GetPosition()[i];
  //          }
  //          light->SetFocalPoint(info[0], info[1], info[2]);
  //        }
  //      }
  //    }
  //  }

  //  // If we created a new VTK light, add it to the collection
  //  if (light_created)
  //  {
  //    this->AddLight(light);
  //    light->Delete();
  //  }
  //}

  // Forward the call to the Superclass
  this->Superclass::Render();
}

//----------------------------------------------------------------------------
vtkCamera* vtkExternalOpenGLRenderer3dh::MakeCamera()
{
  vtkCamera* cam = vtkExternalOpenGLCamera::New();
  this->InvokeEvent(vtkCommand::CreateCameraEvent, cam);
  return cam;
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::AddExternalLight(vtkExternalLight *light)
{
  if (!light)
  {
    return;
  }

  vtkExternalLight* aLight;

  vtkCollectionSimpleIterator sit;
  for (this->ExternalLights->InitTraversal(sit);
       (aLight = vtkExternalLight::SafeDownCast(
          this->ExternalLights->GetNextLight(sit))); )
  {
    if (aLight && (aLight->GetLightIndex() == light->GetLightIndex()))
    {
      vtkErrorMacro( << "Attempting to add light with index " <<
                     light->GetLightIndex() <<
                     ". But light with same index already exists.");
      return;
    }
  }

  this->ExternalLights->AddItem(light);
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::RemoveExternalLight(vtkExternalLight *light)
{
  this->ExternalLights->RemoveItem(light);
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::RemoveAllExternalLights()
{
  this->ExternalLights->RemoveAllItems();
}

//----------------------------------------------------------------------------
void vtkExternalOpenGLRenderer3dh::PrintSelf(ostream &os, vtkIndent indent)
{
  this->Superclass::PrintSelf(os, indent);

  os << indent << "External Lights:\n";
  this->ExternalLights->PrintSelf(os, indent.GetNextIndent());
}
