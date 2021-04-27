#pragma once

#include <queue>
#include <mutex>
#include <condition_variable>

#include <vtkMatrix4x4.h>
#include <vtkSmartPointer.h>

#include "VtkUnityWorkbenchAPIDefines.h"

// --------------------------------------------------------------------------
// Standard structures to ease data exchange

inline vtkSmartPointer<vtkMatrix4x4> Float16ToVtkMatrix4x4(const Float16& matrixIn)
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

// A threadsafe-queue.
template <class T>
class SafeQueue
{
public:
	SafeQueue(void)
		: q()
		, m()
		, c()
	{ }

	~SafeQueue(void) { }

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
