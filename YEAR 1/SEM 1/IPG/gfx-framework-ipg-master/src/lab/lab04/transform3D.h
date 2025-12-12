#pragma once

#include "utils/glm_utils.h"


namespace transform3D
{
    // Translation matrix
    inline glm::mat4 Translate(float translateX, float translateY, float translateZ)
    {
        // TODO(student): Ex. 1
		return glm::mat4(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			translateX, translateY, translateZ, 1);
    }

    // Scaling matrix
    inline glm::mat4 Scale(float scaleX, float scaleY, float scaleZ)
    {
        // TODO(student): Ex. 1
        return glm::mat4(
            scaleX, 0, 0, 0,
            0, scaleY, 0, 0,
            0, 0, scaleZ, 0,
			0, 0, 0, 1);
    }

    // Rotation matrix relative to the OZ axis
    inline glm::mat4 RotateOZ(float radians)
    {
        // TODO(student): Ex. 1
        return glm::mat4(
            cos(radians), sin(radians), 0, 0,
            -sin(radians), cos(radians), 0, 0,
            0, 0, 1, 0,
			0, 0, 0, 1);
    }

    // Rotation matrix relative to the OY axis
    inline glm::mat4 RotateOY(float radians)
    {
        // TODO(student): Ex. 1
        return glm::mat4(
            cos(radians), 0, -sin(radians), 0,
            0, 1, 0, 0,
			sin(radians), 0, cos(radians), 0,
			0, 0, 0, 1);
    }

    // Rotate matrix relative to the OX axis
    inline glm::mat4 RotateOX(float radians)
    {
        // TODO(student): Ex. 1
        return glm::mat4(
            1, 0, 0, 0,
            0, cos(radians), sin(radians), 0,
			0, -sin(radians), cos(radians), 0,
			0, 0, 0, 1);
    }

    // Perspective projection matrix
    inline glm::mat4 Perspective(
        float fovy, float aspect, float zNear, float zFar)
    {
        // TODO(student): Ex. 2
        return glm::mat4(
			1 / (tan(fovy / 2) * aspect), 0, 0, 0,
			0, 1 / tan(fovy / 2), 0, 0,
			0, 0, -(zFar + zNear) / (zFar - zNear), -1,
			0, 0, -(2 * zFar * zNear) / (zFar - zNear), 0);
    }

    // View matrix
    inline glm::mat4 View(
        const glm::vec3& position,
        const glm::vec3& forward,
        const glm::vec3& right,
        const glm::vec3& up)
    {
        // TODO(student): Ex. 4
        return glm::mat4(
			right.x, up.x, forward.x, 0,
			right.y, up.y, forward.y, 0,
			right.z, up.z, forward.z, 0,
			0, 0, 0, 1) * Translate(-position.x, -position.y, -position.z);
    }

}   // namespace transform3D
