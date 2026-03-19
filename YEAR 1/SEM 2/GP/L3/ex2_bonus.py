import sys
import pygame
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


CELL_SIZE = 8
FPS = 2
MAX_GENERATIONS = 20
UI_HEIGHT = 40


COLOR_WALL = (0, 0, 0)
COLOR_FLOOR = (255, 255, 255)
COLOR_TEXT = (255, 0, 0)


def draw_grid_pygame(screen, grid):

    screen.fill((40, 40, 40))

    grid_rect = pygame.Rect(0, UI_HEIGHT, GRID_WIDTH * CELL_SIZE, GRID_HEIGHT * CELL_SIZE)
    pygame.draw.rect(screen, COLOR_FLOOR, grid_rect)

    height = len(grid)
    width = len(grid[0])

    for y in range(height):
        for x in range(width):
            if grid[y][x] == 1:  # Wall
                rect = pygame.Rect(x * CELL_SIZE, y * CELL_SIZE + UI_HEIGHT, CELL_SIZE, CELL_SIZE)
                pygame.draw.rect(screen, COLOR_WALL, rect)


def run_animation():
    pygame.init()

    win_width = GRID_WIDTH * CELL_SIZE
    win_height = (GRID_HEIGHT * CELL_SIZE) + UI_HEIGHT
    screen = pygame.display.set_mode((win_width, win_height))
    pygame.display.set_caption("Dungeon Generator - Conway's Game of Life")

    clock = pygame.time.Clock()
    font = pygame.font.SysFont('Arial', 20, bold=True)

    grid = initialize_grid(GRID_WIDTH, GRID_HEIGHT, INITIAL_WALL_CHANCE)
    current_generation = 1
    paused = True
    finished = False

    running = True
    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_ESCAPE:
                    running = False
                if event.key == pygame.K_SPACE and not finished:
                    paused = not paused
                if event.key == pygame.K_r:
                    grid = initialize_grid(GRID_WIDTH, GRID_HEIGHT, INITIAL_WALL_CHANCE)
                    current_generation = 1
                    finished = False
                    paused = True

        if not paused and not finished:
            if current_generation < MAX_GENERATIONS:
                grid = generate_next_generation(grid)
                current_generation += 1
            else:
                finished = True


        draw_grid_pygame(screen, grid)

        hud_text = f"Gen: {current_generation}/{MAX_GENERATIONS}   |   SPACE: Pause/Play   |   R: Restart   |   ESC: Quit"

        if paused:
            hud_text += "   [ PAUSED ]"
        elif finished:
            hud_text += "   [ FINISHED ]"

        text_surface = font.render(hud_text, True, COLOR_TEXT)
        screen.blit(text_surface, (15, 8))

        pygame.display.flip()

        clock.tick(FPS)

    pygame.quit()
    sys.exit()


if __name__ == "__main__":
    run_animation()