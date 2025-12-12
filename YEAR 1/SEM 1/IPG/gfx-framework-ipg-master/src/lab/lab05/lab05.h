#pragma once

#include <vector>

#include "components/simple_scene.h"

#include "lab/lab03/transform2D.h"

namespace lab
{
    class Lab05 : public gfxc::SimpleScene
    {
     public:
        Lab05();
        ~Lab05();

        void Init() override;

     private:
        void CreateMesh(const char *name, const std::vector<VertexFormat> &vertices, const std::vector<unsigned int> &indices);
        void DrawObjects(gfxc::Camera *camera, const transform2D::ViewportSpace &viewport_space);

        void FrameStart() override;
        void Update(float deltaTimeSeconds) override;
        void FrameEnd() override;

        void OnInputUpdate(float deltaTime, int mods) override;
        void OnKeyPress(int key, int mods) override;
        void OnKeyRelease(int key, int mods) override;
        void OnMouseMove(int mouseX, int mouseY, int deltaX, int deltaY) override;
        void OnMouseBtnPress(int mouseX, int mouseY, int button, int mods) override;
        void OnMouseBtnRelease(int mouseX, int mouseY, int button, int mods) override;
        void OnMouseScroll(int mouseX, int mouseY, int offsetX, int offsetY) override;
        void OnWindowResize(int width, int height) override;

        void RenderMesh(Mesh * mesh, Shader * shader, const glm::mat4 & modelMatrix,
            const glm::mat4 &viewMatrix, const glm::mat4 &projectionMatrix);

     protected:
        transform2D::ViewportSpace viewport_space;
        glm::vec3 color;
        GLenum cullFace;

     private:
        float moveY = 0.5f;
        float moveSpeed = 1.0f; // unitati / secunda
        int moveDir = 1; // 1 = in sus, -1 = in jos
        const float moveMin = 0.0f;   
        const float moveMax = 3.0f;

        float rotateAngle = 0.0f; // in radiani
        float rotateSpeed = glm::radians(45.0f); // 45 grade / secunda

        float pulseScale = 1.0f;
        float pulseSpeed = 0.75f; // unitati / secunda
        int pulseDir = 1; // 1 = se mareste, -1 = se micsoreaza
        const float pulseMin = 0.5f;
        const float pulseMax = 2.0f;

		glm::vec3 cubePos = glm::vec3(-1.5f, 0.5f, 0); // pozitia initiala a cubului de mutat
		float cubeMoveSpeed = 1.0f; // unitati / secunda

		glm::vec3 prevCameraPos = glm::vec3(0);
        bool canMove = true;
    };
}   // namespace lab
