#include "lab/lab01/lab01.h"

#include <vector>

using namespace std;
using namespace lab;

Lab01::Lab01()
{
    window->SetSize(1280, 720);
}


Lab01::~Lab01()
{
}

void Lab01::Initialize()
{
    Exercise1();
    Exercise2();
    Exercise3();
    Exercise4();
    Exercise5();
    Exercise6();
}

void Lab01::Exercise1()
{
    SetExercise();

    //TODO(student): Ex. 1
    {
        SetPanel("1", { -3, -2, 100 }, { 0, 0, 1280, 720 });

        glm::vec2 v1(5, 2);
        glm::vec2 v2(2, 3);

		glm::vec2 v12 = v1 + v2;

        RenderVector(v1, glm::vec3(1, 0, 0), "v1");
        RenderVector(v2, glm::vec3(0, 1, 0), "v2");
        RenderVector(v1, glm::vec3(1, 0, 0), "v1", v2);
        RenderVector(v2, glm::vec3(0, 1, 0), "v2", v1);
        

        RenderVector(v12, glm::vec3(0, 0, 1), "v12");
    }
}

void Lab01::Exercise2()
{
    SetExercise();

    //TODO(student): Ex. 2
    {
        SetPanel("2", { -7, -5, 75 }, { 0, 0, 1280, 720 });

        {
            glm::vec2 p1(1, 1);
            glm::vec2 p2(3, -3);

            RenderPoint(p1, glm::vec3(1, 0, 0), "P1");
            RenderPoint(p2, glm::vec3(0, 0, 1), "P2");

            glm::vec2 v21 = p1 - p2;
            RenderVector(v21, glm::vec3(1, 0, 1), "v21", p2);
        }

        {
            glm::vec2 p3(-1, -2);
            glm::vec2 p4(-5, -4);

            RenderPoint(p3, glm::vec3(1, 0, 0), "P3");
            RenderPoint(p4, glm::vec3(0, 0, 1), "P4");

            glm::vec2 v43 = p3 - p4;
            RenderVector(v43, glm::vec3(1, 0, 1), "v43", p4);
        }

        {
            glm::vec2 p5(6, 3);
            glm::vec2 p6(4, 1);

            RenderPoint(p5, glm::vec3(1, 0, 0), "P5");
            RenderPoint(p6, glm::vec3(0, 0, 1), "P6");

            glm::vec2 v56 = p6 - p5;
            RenderVector(v56, glm::vec3(1, 0, 1), "v56", p5);
        }

        {
            glm::vec2 p7(5, 4);
            glm::vec2 p8(-3, 4);

            RenderPoint(p7, glm::vec3(1, 0, 0), "P7");
            RenderPoint(p8, glm::vec3(0, 0, 1), "P8");

            glm::vec2 v87 = p7 - p8;
            RenderVector(v87, glm::vec3(1, 0, 1), "v87", p8);
        }
    }
}

void Lab01::Exercise3()
{
    SetExercise();

    //TODO(student): Ex. 3
    {
        SetPanel("3", { -7, -5, 75 }, { 0, 0, 1280, 720 });

        glm::vec2 v1(4, 3);
        glm::vec2 v2(-3, 3);
        glm::vec2 v3(-6, -2);
        glm::vec2 v4(-2, -4);
        glm::vec2 v5(3, -2);

        RenderVector(v1, glm::vec3(1, 0, 0), "v1");
        RenderVector(v2, glm::vec3(0, 1, 0), "v2");
        RenderVector(v3, glm::vec3(0, 1, 1), "v3");
        RenderVector(v4, glm::vec3(0, 0, 1), "v4");
        RenderVector(v5, glm::vec3(1, 0, 1), "v5");

        glm::vec2 vu1 = glm::normalize(v1);
        RenderVector(vu1, glm::vec3(0,0,0));
        glm::vec2 vu2 = glm::normalize(v2);
        RenderVector(vu2, glm::vec3(0, 0, 0));
        glm::vec2 vu3 = glm::normalize(v3);
        RenderVector(vu3, glm::vec3(0, 0, 0));
        glm::vec2 vu4 = glm::normalize(v4);
        RenderVector(vu4, glm::vec3(0, 0, 0));
        glm::vec2 vu5 = glm::normalize(v5);
        RenderVector(vu5, glm::vec3(0, 0, 0));
    }
}

void Lab01::Exercise4()
{
    SetExercise ();

    //TODO(student): Ex. 4
    {
        SetPanel("4", { -3, -2, 100 }, { 0, 0, 1280, 720 });

        glm::vec2 v1(4, 1.5);
        glm::vec2 v2(2, 3);

        RenderVector(v1, glm::vec3(1, 0, 0), "v1");
        RenderVector(v2, glm::vec3(0, 1, 0), "v2");

        float res = glm::dot(v1, v2);
        float norm1 = glm::length(v1);
        float norm2 = glm::length(v2);
        float cos = res / (norm1 * norm2);
        float start = atan2(v1.y, v1.x);
        RenderArc(start, acos(cos), glm::vec3(0, 0, 1));
        
        float unghi_mare = 2 * M_PI - acos(cos);
        float start2 = atan2(v2.y, v2.x);
        RenderArc(start2, unghi_mare, glm::vec3(1, 0, 0), 1.0F);

    }
}

void Lab01::Exercise5()
{
    SetExercise();

    //TODO(student): Ex. 5
    {
        SetPanel("5", { -7, -5, 75 }, { 0, 0, 1280, 720 });

        {
            glm::vec2 p1(1, 1);
            glm::vec2 p2(3, 1);
            glm::vec2 p3(2, 4);

            RenderTriangle(p1, p2, p3, glm::vec3(0, 1, 1));

            RenderPoint(p1, glm::vec3(1, 0, 0), "P1");
            RenderPoint(p2, glm::vec3(0, 0, 1), "P2");
            RenderPoint(p3, glm::vec3(0, 1, 0), "P3");

            float a = glm::length(p1 - p2);
            float b = glm::length(p1 - p3);
            float c = glm::length(p2 - p3);

            float s = 0.5f * (a + b + c);
            float A = sqrt(s * (s - a) * (s - b) * (s - c));

            float xG = (p1.x + p2.x + p3.x) / 3;
            float yG = (p1.y + p2.y + p3.y) / 3;

            glm::vec2 g(xG, yG);

            RenderText(g, std::to_string(A), glm::vec3(0, 0, 0));



        }

        {
            glm::vec2 p1(-1.5, -3.7);
            glm::vec2 p2(-2.5, 3.2);
            glm::vec2 p3(-6.1, 2.9);

            RenderTriangle(p1, p2, p3, glm::vec3(0, 1, 1));

            RenderPoint(p1, glm::vec3(1, 0, 0), "P1");
            RenderPoint(p2, glm::vec3(0, 0, 1), "P2");
            RenderPoint(p3, glm::vec3(0, 1, 0), "P3");

            float a = glm::length(p1 - p2);
            float b = glm::length(p1 - p3);
            float c = glm::length(p2 - p3);

            float s = 0.5f * (a + b + c);
            float A = sqrt(s * (s - a) * (s - b) * (s - c));

            float xG = (p1.x + p2.x + p3.x) / 3;
            float yG = (p1.y + p2.y + p3.y) / 3;

            glm::vec2 g(xG, yG);

            RenderText(g, std::to_string(A), glm::vec3(0, 0, 0));
        }

        {
            glm::vec2 p1(2.3, -2.1);
            glm::vec2 p2(7.1, -3.5);
            glm::vec2 p3(7.5, 3.7);

            RenderTriangle(p1, p2, p3, glm::vec3(0, 1, 1));

            RenderPoint(p1, glm::vec3(1, 0, 0), "P1");
            RenderPoint(p2, glm::vec3(0, 0, 1), "P2");
            RenderPoint(p3, glm::vec3(0, 1, 0), "P3");

            float a = glm::length(p1 - p2);
            float b = glm::length(p1 - p3);
            float c = glm::length(p2 - p3);

            float s = 0.5f * (a + b + c);
            float A = sqrt(s * (s - a) * (s - b) * (s - c));

            float xG = (p1.x + p2.x + p3.x) / 3;
            float yG = (p1.y + p2.y + p3.y) / 3;

            glm::vec2 g(xG, yG);

            RenderText(g, std::to_string(A), glm::vec3(0, 0, 0));
        }
    }
}

void Lab01::Exercise6()
{
    SetExercise();

    //TODO(student): Ex. 6
    {
        SetPanel("6", { -7, -5, 75 }, { 0, 0, 1280, 720 });

        glm::vec2 p1(-4, 3);
        glm::vec2 p2(6, 2);
        glm::vec2 p3(1, -4);

        RenderTriangle(p1, p2, p3, glm::vec3(0, 1, 1));

        RenderPoint(p1, glm::vec3(0, 0, 1), "P1");
        RenderPoint(p2, glm::vec3(0, 0, 1), "P2");
        RenderPoint(p3, glm::vec3(0, 0, 1), "P3");

        std::vector<glm::vec2> check_points{
            glm::vec2(1, 1), glm::vec2(3.21, 3.2),
            glm::vec2(-2.16, 2.9), glm::vec2(2, -2),
            glm::vec2(-1.1, -0.9), glm::vec2(-1.9, 2.1),
            glm::vec2(-4, 0), glm::vec2(-4, -2.5),
            glm::vec2(4.5, 1.1), glm::vec2(5.9, -3.1),
            glm::vec2(7.1, 1.9), glm::vec2(-5.9, 1.5)
        };

        const float EPSILON = 0.01f;
        //bool inside_triangle = abs(area_v1v2v3 - (area_pv1v3 + area_pv1v2 + area_pv2v3)) < EPSILON;

        for (const glm::vec2 &check_point : check_points) {

            float a = glm::length(p1 - p2);
            float b = glm::length(p1 - p3);
            float c = glm::length(p2 - p3);

            float s = 0.5f * (a + b + c);
            float A = sqrt(s * (s - a) * (s - b) * (s - c));


            float a1 = glm::length(check_point - p2);
            float b1 = glm::length(check_point - p3);
            float c1 = glm::length(p2 - p3);

            float s1 = 0.5f * (a1 + b1 + c1);
            float A1 = sqrt(s1 * (s1 - a1) * (s1 - b1) * (s1 - c1));


            float a2 = glm::length(check_point - p1);
            float b2 = glm::length(check_point - p3);
            float c2 = glm::length(p1 - p3);

            float s2 = 0.5f * (a2 + b2 + c2);
            float A2 = sqrt(s2 * (s2 - a2) * (s2 - b2) * (s2 - c2));


            float a3 = glm::length(check_point - p1);
            float b3 = glm::length(check_point - p2);
            float c3 = glm::length(p1 - p2);

            float s3 = 0.5f * (a3 + b3 + c3);
            float A3 = sqrt(s3 * (s3 - a3) * (s3 - b3) * (s3 - c3));

            bool inside_triangle = abs(A - (A1 + A2 + A3)) < EPSILON;

            if (inside_triangle == true)
            {
                RenderPoint(check_point, glm::vec3(0, 1, 0), "in");
            }
            else
            {
                RenderPoint(check_point, glm::vec3(1, 0, 0), "out");
            }
            
        }
    }
}
