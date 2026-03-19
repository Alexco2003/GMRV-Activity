import os
from PIL import Image
import random


GRID_WIDTH = 100
GRID_HEIGHT = 80
INITIAL_WALL_CHANCE = 0.45

def initialize_grid(width, height, wall_chance):

    grid = []

    for y in range(height):
        row = []
        for x in range(width):
            if random.random() < wall_chance:
                row.append(1)  # Alive / Wall
            else:
                row.append(0)  # Dead / Floor
        grid.append(row)

    return grid


def count_alive_neighbors(grid, x, y):

    count = 0
    height = len(grid)
    width = len(grid[0])

    for i in range(-1, 2):  # -1, 0, 1
        for j in range(-1, 2):  # -1, 0, 1
            if i == 0 and j == 0:
                continue

            neighbor_y = y + i
            neighbor_x = x + j

            if 0 <= neighbor_x < width and 0 <= neighbor_y < height:
                count += grid[neighbor_y][neighbor_x]

    return count


def generate_next_generation(current_grid):

    height = len(current_grid)
    width = len(current_grid[0])

    new_grid = [[0 for _ in range(width)] for _ in range(height)]

    for y in range(height):
        for x in range(width):
            alive_neighbors = count_alive_neighbors(current_grid, x, y)

            if current_grid[y][x] == 1:  # Alive Cell (Wall)
                # 1. Any live cell with fewer than two live neighbors dies (underpopulation)
                if alive_neighbors < 2:
                    new_grid[y][x] = 0
                # 2. Any live cell with two or three live neighbors lives on to the next generation
                elif alive_neighbors == 2 or alive_neighbors == 3:
                    new_grid[y][x] = 1
                # 3. Any live cell with more than three live neighbors dies (overpopulation)
                elif alive_neighbors > 3:
                    new_grid[y][x] = 0
            else:  # Dead Cell (Floor)
                # 4. Any dead cell with exactly three live neighbors becomes a live cell (reproduction)
                if alive_neighbors == 3:
                    new_grid[y][x] = 1
                else:
                    new_grid[y][x] = 0

    return new_grid

def create_static_dungeon_image():

    random.seed()
    grid = initialize_grid(GRID_WIDTH, GRID_HEIGHT, INITIAL_WALL_CHANCE)

    target_generation = 5
    for gen in range(1, target_generation):
        grid = generate_next_generation(grid)


    pixel_size = 5
    img_width = GRID_WIDTH * pixel_size
    img_height = GRID_HEIGHT * pixel_size

    img = Image.new('RGB', (img_width, img_height), color=(255, 255, 255))
    pixels = img.load()

    for y in range(GRID_HEIGHT):
        for x in range(GRID_WIDTH):
            if grid[y][x] == 1:  # Wall
                color = (0, 0, 0)  # Black
            else:  # Floor
                color = (255, 255, 255)  # White

            # Draw a 'pixel_size' x 'pixel_size' square
            for py in range(pixel_size):
                for px in range(pixel_size):
                    if (x * pixel_size + px) < img_width and (y * pixel_size + py) < img_height:
                        pixels[x * pixel_size + px, y * pixel_size + py] = color


    folder = "images"

    path = os.path.join(folder, "ex2.png")
    img.save(path)

    img.show()


if __name__ == "__main__":
    create_static_dungeon_image()