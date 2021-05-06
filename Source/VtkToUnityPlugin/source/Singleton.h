#pragma once

// Based on https://stackoverflow.com/questions/1091314/singleton-and-abstract-base-class-in-c

template <typename T>
class Singleton
{
public:
	static T* Instance()
	{
		static T instance{ };
		return &instance;
	}
};