import pygame
import random

# Initialize Pygame
pygame.init()

# Set window properties
width, height = 800, 600
screen = pygame.display.set_mode((width, height))
pygame.display.set_caption('Pygame Example')

# Set the drawing colors
white = (255, 255, 255)
red = (255, 0, 0)

# Set the coordinates and the dimensions of the square
x, y = 100, 100  # Upper-left corner
length = 50     # Length of the square

running = True
regen = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_SPACE:
                regen = True

    # Set background color
    screen.fill(white)

    # Draw a square
    #pygame.draw.rect(screen, red, (x, y, length, length))

    if regen:
        for i in range(5):
            for j in range(5):
                color = (random.randint(0, 255), random.randint(0, 255), random.randint(0, 255))
                pygame.draw.rect(screen, color, (x+100*i, y+100*j, length, length))
        regen = False

        # Update the screen
        pygame.display.flip()

