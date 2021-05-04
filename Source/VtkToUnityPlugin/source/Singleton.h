#pragma once

// Based on https://stackoverflow.com/questions/1091314/singleton-and-abstract-base-class-in-c

template <typename T>
class Singleton
{
public:
	static T* Instance()
	{
		if (m_instance == NULL)
		{
			m_instance = new T();
		}

		return m_instance;
	}

private:
	static T* m_instance;
};