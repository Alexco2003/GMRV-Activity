import math
import os
from PIL import Image

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


def getConstantVector(v):
    h = v % 4
    if h == 0:
        return (1.0, 1.0)
    elif h == 1:
        return (-1.0, 1.0)
    elif h == 2:
        return (-1.0, -1.0)
    elif h == 3:
        return (1.0, -1.0)


def dot(v1, v2):
    return v1[0] * v2[0] + v1[1] * v2[1]



def perlin(x, y):

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

    dotTopRight = dot(topRight, getConstantVector(valueTopRight))
    dotTopLeft = dot(topLeft, getConstantVector(valueTopLeft))
    dotBottomRight = dot(bottomRight, getConstantVector(valueBottomRight))
    dotBottomLeft = dot(bottomLeft, getConstantVector(valueBottomLeft))

    u = fade(xf)
    v = fade(yf)

    x1 = lerp(u, dotBottomLeft, dotBottomRight)
    x2 = lerp(u, dotTopLeft, dotTopRight)

    return lerp(v, x1, x2)



width, height = 400, 400
image = Image.new("L", (width, height))
pixels = image.load()

frequency = 0.02 # 0.002, 0.02, 0.2

for y in range(height):
    for x in range(width):
        n = perlin(x * frequency, y * frequency)

        color = int((n + 1.0) / 2.0 * 255)
        color = max(0, min(255, color))

        pixels[x, y] = color

folder = "images"
prefix = "perlin_"
extension = ".png"

max_index = 0

if not os.path.exists(folder):
    os.makedirs(folder)

for filename in os.listdir(folder):
    if filename.startswith(prefix) and filename.endswith(extension):
        try:
            number_part = filename[len(prefix):-len(extension)]
            index = int(number_part)
            if index > max_index:
                max_index = index
        except ValueError:
            continue

new_index = max_index + 1
new_filename = f"{folder}/{prefix}{new_index}{extension}"

image.save(new_filename)
print("Perlin generated and saved as:", new_filename)

image.show()