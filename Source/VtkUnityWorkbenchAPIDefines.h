#pragma once

#include <array>

struct Float16 {
	float elements[16];
};

enum DebugLogLevel {
	DebugImmediate = 0,
	DebugLog,
	DebugLogWarning,
	DebugLogError
};
