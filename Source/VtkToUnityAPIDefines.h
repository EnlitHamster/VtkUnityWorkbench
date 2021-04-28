#pragma once

#include <array>

// --------------------------------------------------------------------------
// Standard structures to ease data exchange

struct Float4 {
	float x;
	float y;
	float z;
	float w;
};

static const std::array<double, 3> sZeroArray3{ { 0.0, 0.0, 0.0 } };

template<typename T> static Float4 StdArray3ToFloat4(
	const std::array<T, 3>& arrayIn)
{
	Float4 f4;
	f4.x = static_cast<float>(arrayIn[0]);
	f4.y = static_cast<float>(arrayIn[1]);
	f4.z = static_cast<float>(arrayIn[2]);
	f4.w = 0.0f;

	return f4;
}

struct Float16 {
	float elements[16];
};

enum DebugLogLevel {
	DebugImmediate = 0,
	DebugLog,
	DebugLogWarning,
	DebugLogError
};

enum LightColorType {
	LightColorAmbient = 0,
	LightColorDiffuse,
	LightColorSpecular,
	NLightColorType
};

enum VolumeLightType {
	VolumeLightAmbient = 0,
	VolumeLightDiffuse,
	VolumeLightSpecular,
	VolumeLightSpecularPower,
	NVolumeLightType
};

