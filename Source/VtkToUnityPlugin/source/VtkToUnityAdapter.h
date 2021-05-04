#pragma once

#include <string>

#define NOMINMAX
#include <windows.h>

#include <sstream>

/// Adapters should be singletons !!!
///
/// Even though Singletons are a difficult pattern to master AND PEOPLE ABUSE IT
// (see this thread FOR HEAVEN'S SAKE: 
//  https://stackoverflow.com/questions/86582/singleton-how-should-it-be-used
//  and for further reference:
//  https://stackoverflow.com/questions/1008019/c-singleton-design-pattern)
/// This case REQUIRES singletons, as we are creating interface adapters that require
/// resources in a real-time rendering system, i.e. without thread-safe, multi-thread at
/// best, singletons, we would waste resources and risk a plethora of memory leaks
///
/// A singleton provider class is implemented in Singleton.h
/// always use the adapters wrapped in a singleton !!!
class VtkToUnityAdapter
{
public:
	template <typename T> using getter = std::stringstream(T::*)(vtkSmartPointer<vtkActor>);
	template <typename T> using setter = void (T::*)(vtkSmartPointer<vtkActor>, LPCSTR);

	virtual ~VtkToUnityAdapter() { }

	virtual void Initialize(
		std::string vtkObjectName, 
		void* args) = 0;

	inline std::string GetAdaptingObject() 
	{
		return m_vtkObjectName;
	}

protected:
	// The name of the VTK object (as written in the wiki) for which
	// the class acts as an adapter
	std::string m_vtkObjectName;

	static inline std::stringstream ReturnError(
		LPCSTR msg)
	{
		std::stringstream buffer;
		buffer << "err::(" << msg << ")";
		return buffer;
	}
};