import pygame
import math
import sys

permutation = [
    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
    140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
    247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
    57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
    74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
    60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
    65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
    200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
    52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
    207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
    119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
    218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
    81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
    184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
    222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
]

p = permutation * 2


def fade(t):
    return ((6 * t - 15) * t + 10) * t * t * t


def lerp(t, a1, a2):
    return a1 + t * (a2 - a1)


def dot(v1, v2): return v1[0] * v2[0] + v1[1] * v2[1]


def getConstantVector(v, time):
    h = v % 4
    if h == 0:
        base_angle = math.pi / 4  # 45 degrees top right
    elif h == 1:
        base_angle = 3 * math.pi / 4  # 135 degrees top left
    elif h == 2:
        base_angle = 5 * math.pi / 4  # 225 degrees bottom left
    elif h == 3:
        base_angle = 7 * math.pi / 4  # 315 degrees bottom right

    current_angle = base_angle + time

    return (math.cos(current_angle), math.sin(current_angle))



def perlin(x, y, time):

    X = math.floor(x) & 255
    Y = math.floor(y) & 255

    xf = x - math.floor(x)
    yf = y - math.floor(y)

    topRight = (xf - 1.0, yf - 1.0)
    topLeft = (xf, yf - 1.0)
    bottomRight = (xf - 1.0, yf)
    bottomLeft = (xf, yf)

    valueTopRight = p[p[X + 1] + Y + 1]
    valueTopLeft = p[p[X] + Y + 1]
    valueBottomRight = p[p[X + 1] + Y]
    valueBottomLeft = p[p[X] + Y]

    dotTopRight = dot(topRight, getConstantVector(valueTopRight, time))
    dotTopLeft = dot(topLeft, getConstantVector(valueTopLeft, time))
    dotBottomRight = dot(bottomRight, getConstantVector(valueBottomRight, time))
    dotBottomLeft = dot(bottomLeft, getConstantVector(valueBottomLeft, time))

    u = fade(xf)
    v = fade(yf)

    x1 = lerp(u, dotBottomLeft, dotBottomRight)
    x2 = lerp(u, dotTopLeft, dotTopRight)

    return lerp(v, x1, x2)


pygame.init()

REAL_WIDTH, REAL_HEIGHT = 80, 80
SCALE = 6
WINDOW_WIDTH = REAL_WIDTH * SCALE
WINDOW_HEIGHT = REAL_HEIGHT * SCALE

screen = pygame.display.set_mode((WINDOW_WIDTH, WINDOW_HEIGHT))
pygame.display.set_caption("Perlin Plasma")
surface = pygame.Surface((REAL_WIDTH, REAL_HEIGHT))

clock = pygame.time.Clock()
time = 0.0
frequency = 0.08

running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    for x in range(REAL_WIDTH):
        for y in range(REAL_HEIGHT):
            n = perlin(x * frequency, y * frequency, time)

            r = int((math.sin(n * 10) + 1) / 2 * 255)
            g = int((math.sin(n * 10 + 2.0) + 1) / 2 * 255)
            b = int((math.sin(n * 10 + 4.0) + 1) / 2 * 255)

            surface.set_at((x, y), (r, g, b))

    scaled_surface = pygame.transform.scale(surface, (WINDOW_WIDTH, WINDOW_HEIGHT))
    screen.blit(scaled_surface, (0, 0))
    pygame.display.flip()

    time += 0.05

pygame.quit()
sys.exit()