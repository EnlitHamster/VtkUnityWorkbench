#pragma once

#include <queue>
#include <mutex>
#include <condition_variable>

#include <vtkMatrix4x4.h>
#include <vtkSmartPointer.h>

#include "VtkToUnityAPIDefines.h"

#include <sstream>

// --------------------------------------------------------------------------
// Standard structures to ease data exchange

inline vtkSmartPointer<vtkMatrix4x4> Float16ToVtkMatrix4x4(
	const Float16& matrixIn)
{
	auto vtkMatrix = vtkSmartPointer<vtkMatrix4x4>::New();

	for (int row = 0; row < 4; row++)
	{
		for (int col = 0; col < 4; col++)
		{
			vtkMatrix->SetElement(row, col, matrixIn.elements[(row * 4) + col]);
		}
	}

	return vtkMatrix;
}

inline std::stringstream ValueDoubleToCharArray(double val)
{
	std::stringstream buffer;
	buffer << "val::(" << val << ")";
	return buffer;
}

inline std::stringstream ValueVector3ToCharArray(double* vec)
{
	std::stringstream buffer;
	buffer << "val::(" << vec[0] << "," << vec[1] << "," << vec[2] << ")";
	return buffer;
}

inline std::stringstream ValueIntToCharArray(int val)
{
	std::stringstream buffer;
	buffer << "val::(" << val << ")";
	return buffer;
}

inline double CharArrayToValueDouble(LPCSTR strVal)
{
	return atof(strVal);
}

inline int CharArrayToValueInt(LPCSTR strVal)
{
	return atoi(strVal);
}

inline double* CharArrayToValueVector3(LPCSTR strVal)
{
	double* vec = (double*) malloc(3 * sizeof(double));
	char* coord = strtok((LPSTR) strVal, ",");
	for (int i = 0; i < 3 && coord != NULL; ++i)
	{
		vec[i] = atof(coord);
		coord = strtok(NULL, ",");
	}
	return vec;
}

// A threadsafe-queue.
template <class T>
class SafeQueue
{
public:
	SafeQueue(void)
		: q()
		, m()
		, c()
	{}

	~SafeQueue(void)
	{}

	// Add an element to the queue.
	void enqueue(T t)
	{
		std::lock_guard<std::mutex> lock(m);
		q.push(t);
		c.notify_one();
	}

	// Get the "front"-element.
	// If the queue is empty, wait till a element is avaiable.
	T dequeue(void)
	{
		std::unique_lock<std::mutex> lock(m);
		while (q.empty())
		{
			// release lock as long as the wait and reaquire it afterwards.
			c.wait(lock);
		}
		T val = q.front();
		q.pop();
		return val;
	}

	bool empty(void)
	{
		std::unique_lock<std::mutex> lock(m);
		return q.empty();
	}

private:
	std::queue<T> q;
	mutable std::mutex m;
	std::condition_variable c;
};
