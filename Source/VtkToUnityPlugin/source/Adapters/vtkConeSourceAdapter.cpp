#include "vtkConeSourceAdapter.h"

#include "vtkConeSource.h"
#include "vtkAlgorithm.h"
#include "vtkMapper.h"
#include "vtkAlgorithmOutput.h"

const std::unordered_map<LPCSTR, std::pair<VtkAdapter::getter<VtkConeSourceAdapter>, VtkAdapter::setter<VtkConeSourceAdapter>>>
	VtkConeSourceAdapter::s_attributes =
{
	{ "Height", std::make_pair(&GetHeight, &SetHeight) },
	{ "Radius", std::make_pair(&GetRadius, &SetRadius) },
	{ "Resolution",	std::make_pair(&GetResolution, &SetResolution) },
	{ "Angle", std::make_pair(&GetAngle, &SetAngle) },
	{ "Capping", std::make_pair(&GetCapping, &SetCapping) },
	{ "Center", std::make_pair(&GetCenter, &SetCenter) },
	{ "Direction", std::make_pair(&GetDirection, &SetDirection) }
};

VtkConeSourceAdapter::VtkConeSourceAdapter()
	: VtkAdapter("vtkConeSource") { }


// Source access based on https://kitware.github.io/vtk-examples/site/Cxx/Visualization/ReverseAccess/

void VtkConeSourceAdapter::GetAttribute(
	vtkSmartPointer<vtkActor> actor,
	LPCSTR propertyName,
	char* retValue)
{
	auto algorithm = actor->GetMapper()->GetInputConnection(0, 0)->GetProducer();
	auto coneSource = dynamic_cast<vtkConeSource*>(algorithm);

	auto itAttribute = s_attributes.find(propertyName);

	if (itAttribute == s_attributes.end())
	{
		ReturnError("Property not found") >> retValue;
	}
	else
	{
		(this->*itAttribute->second.first)(actor) >> retValue;
	}
}

void VtkConeSourceAdapter::SetAttribute(
	vtkSmartPointer<vtkActor> actor,
	LPCSTR propertyName,
	LPCSTR newValue)
{
	auto algorithm = actor->GetMapper()->GetInputConnection(0, 0)->GetProducer();
	auto coneSource = dynamic_cast<vtkConeSource*>(algorithm);

	auto itAttribute = s_attributes.find(propertyName);

	if (itAttribute != s_attributes.end())
	{
		(this->*itAttribute->second.second)(actor, newValue);
	}

}