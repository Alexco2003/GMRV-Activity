#include "lab/lab02/lab02.h"

#include <vector>

using namespace std;
using namespace lab;

Lab02::Lab02()
{
    window->SetSize(1280, 720);
}


Lab02::~Lab02()
{
}

void Lab02::Initialize()
{
    image->Init(1280, 720, 3 /* channels */);
    depthImage->Init(1280, 720);

    //{
    //    vector<VertexFormat> vertices
    //    {
    //        VertexFormat(glm::vec3(290, 90,  0.5), glm::vec3(1, 0, 0)),
    //        VertexFormat(glm::vec3(1099, 450,  0.5), glm::vec3(0, 1, 0)),
    //        VertexFormat(glm::vec3(650, 719,  0.5), glm::vec3(0, 0, 1)),

    //        VertexFormat(glm::vec3(200, 450,  0), glm::vec3(0, 1, 1)),
    //        VertexFormat(glm::vec3(830, 719,  1), glm::vec3(1, 1, 0)),
    //        VertexFormat(glm::vec3(1099, 0,  1), glm::vec3(1, 0, 1)) };

    //    vector<unsigned int> indices
    //    {
    //        0, 1, 2,    // indices for first triangle
    //        3, 4, 5,    // indices for second triangle
    //    };

    //    Rasterize(vertices, indices);
    //}

    // Bonus: cerc
    {
        // nr felii de piza
        const float triangles = 6;
        // raza cercului
        const float radius = 200.0f;
        // centru cercului (pus la centrul ecranului)
        const glm::vec3 center(640.0f, 360.0f, 0.5f);

        // vectorul de vertecsi
        vector<VertexFormat> vertices;

        // adaugam centrul ca prim element
        vertices.push_back(VertexFormat(center, glm::vec3(1.0f, 0.0f, 0.0f)));

        for (float i = 0; i < triangles; i++)
        {
            float alpha = 2.0f * M_PI * i / triangles;
            float x = radius * cosf(alpha) + center.x;
            float y = radius * sinf(alpha) + center.y;
            glm::vec3 color(1, 0, 0);
            vertices.push_back(VertexFormat(glm::vec3(x, y, center.z), color));
        }

        vector<unsigned int> indices;
    
        for (unsigned int i = 0; i < triangles; i++)
        {
            // centrul cercului va fi mereu un vf al oricarui triunghi
            unsigned int a = 0;
            // urmatorul vertex
            unsigned int b = i + 1;
            // vertex ul consecutiv anteriorului, formula circulara
            unsigned int c = (i + 1) % static_cast<int>(triangles) + 1;
            indices.push_back(a);
            indices.push_back(b);
            indices.push_back(c);
        }

        Rasterize(vertices, indices);
    }
}

void Lab02::Rasterize(
    const vector<VertexFormat> &vertices,
    const vector<unsigned int> &indices)
{
    for (int i = 0; i < indices.size(); i += 3) {
        auto v1 = vertices[indices[i]];
        auto v2 = vertices[indices[i+1]];
        auto v3 = vertices[indices[i+2]];

        TriangleRasterizer::Rasterize(
            v1, v2, v3, image, depthImage
        );
    }
}
