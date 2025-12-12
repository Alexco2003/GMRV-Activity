#include "lab/lab02/triangle_rasterizer.h"

#include <vector>
#include <iostream>

using namespace std;
using namespace lab;

void TriangleRasterizer::Rasterize(
    const VertexFormat &v1,
    const VertexFormat &v2,
    const VertexFormat &v3,
    Image *image,
    DepthImage *depthImage)
{
    float xMin = min(v1.position.x, min(v2.position.x, v3.position.x));
    float yMin = min(v1.position.y, min(v2.position.y, v3.position.y));
    float xMax = max(v1.position.x, max(v2.position.x, v3.position.x));
    float yMax = max(v1.position.y, max(v2.position.y, v3.position.y));

    for (int row = yMin; row < yMax; row++) {
        for (int column = xMin; column < xMax; column++) {

            if (row < 0 || row >= image->GetHeight() ||
                column < 0 || column >= image->GetWidth()) {
                continue;
            }

            glm::vec2 p(column + 0.5f, row + 0.5f);

            if (CheckPointInsideTriangle(p, v1, v2, v3)) {

                float depth = ComputePixelDepth(p, v1, v2, v3);
                float pixelDepth = depthImage->Get(row, column).x;

                if (pixelDepth > depth) {
                    image->Set(row, column, ComputePixelColor (p, v1, v2, v3));
                    depthImage->Set(row, column, glm::vec3(depth, 0, 0));
                }
            }
        }
    }
}

float TriangleRasterizer::ComputeTriangleArea(
    const glm::vec2 &v1,
    const glm::vec2 &v2,
    const glm::vec2 &v3)
{
    // TODO(student): Ex. 1

    float a = glm::length(v1 - v2);
    float b = glm::length(v1 - v3);
    float c = glm::length(v2 - v3);

    float s = 0.5f * (a + b + c);
    float A = sqrt(s * (s - a) * (s - b) * (s - c));

    return A;
}

bool TriangleRasterizer::CheckPointInsideTriangle(
    const glm::vec2 &p,
    const VertexFormat &v1,
    const VertexFormat &v2,
    const VertexFormat &v3)
{
    const float EPSILON = 5.0f;

    // TODO(student): Ex. 1

    //float a = glm::length(v1.position - v2.position);
    //float b = glm::length(v1.position - v3.position);
    //float c = glm::length(v2.position - v3.position);

    //float s = 0.5f * (a + b + c);
    ////float A = sqrt(s * (s - a) * (s - b) * (s - c));

    float A = ComputeTriangleArea(glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y));


    //float a1 = glm::length(p - glm::vec2(v2.position.x , v2.position.y));
    //float b1 = glm::length(p - glm::vec2(v3.position.x, v3.position.y));
    //float c1 = glm::length(v2.position - v3.position);

    //float s1 = 0.5f * (a1 + b1 + c1);
    //float A1 = sqrt(s1 * (s1 - a1) * (s1 - b1) * (s1 - c1));

    float A1 = ComputeTriangleArea(p, glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y));

    //float a2 = glm::length(p - glm::vec2(v1.position.x, v1.position.y));
    //float b2 = glm::length(p - glm::vec2(v3.position.x, v3.position.y));
    //float c2 = glm::length(v1.position - v3.position);

    //float s2 = 0.5f * (a2 + b2 + c2);
    //float A2 = sqrt(s2 * (s2 - a2) * (s2 - b2) * (s2 - c2));

    float A2 = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v3.position.x, v3.position.y));


    //float a3 = glm::length(p - glm::vec2(v1.position.x, v1.position.y));
    //float b3 = glm::length(p - glm::vec2(v2.position.x, v2.position.y));
    //float c3 = glm::length(v1.position - v2.position);

    //float s3 = 0.5f * (a3 + b3 + c3);
    //float A3 = sqrt(s3 * (s3 - a3) * (s3 - b3) * (s3 - c3));

    float A3 = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y));

    bool inside_triangle = abs(A - (A1 + A2 + A3)) < EPSILON;

    return inside_triangle;
}

glm::vec3 TriangleRasterizer::ComputePixelColor(
    const glm::vec2 &p,
    const VertexFormat &v1,
    const VertexFormat &v2,
    const VertexFormat &v3)
{
    // TODO(student): Ex. 2

    float A = ComputeTriangleArea(glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y));

    float u = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y)) / A;
    float v = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v3.position.x, v3.position.y)) / A;
    float w = ComputeTriangleArea(p, glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y)) / A;

    glm::vec3 color = u * v3.color + v * v2.color + w * v1.color;

    return color;
}

float TriangleRasterizer::ComputePixelDepth(
    const glm::vec2 &p,
    const VertexFormat &v1,
    const VertexFormat &v2,
    const VertexFormat &v3)
{
    // TODO(student): Ex. 3

    float A = ComputeTriangleArea(glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y));

    float u = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v2.position.x, v2.position.y)) / A;
    float v = ComputeTriangleArea(p, glm::vec2(v1.position.x, v1.position.y), glm::vec2(v3.position.x, v3.position.y)) / A;
    float w = ComputeTriangleArea(p, glm::vec2(v2.position.x, v2.position.y), glm::vec2(v3.position.x, v3.position.y)) / A;

    float z = u * v3.position.z + v * v2.position.z + w * v1.position.z;

    return z;
}
