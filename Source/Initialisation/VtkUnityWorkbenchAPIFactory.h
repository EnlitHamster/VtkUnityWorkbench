#pragma once

#include "../VtkUnityWorkbenchAPI.h"

#include <memory>

class VtkUnityWorkbenchAPIFactory
{
public:
	static void Initialise(UnityGfxRenderer apiType);
	static void Shutdown();

	static bool Initialised();
	static std::weak_ptr<VtkUnityWorkbenchAPI> GetAPI();
};
