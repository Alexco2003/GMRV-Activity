from PIL import Image, ImageDraw, ImageFont
import random

import os

# Open image
image = Image.open('images/resized_white.png')

# Resize image
#resized_image = image.resize((800, 600))

# Save the resized image
#resized_image.save('images/resized_white.png')

draw = ImageDraw.Draw(image)

width, height = image.size

font = ImageFont.truetype("arial.ttf", 35)

colors = {
    '~': (46,163,242),
    '#': (39,39,39),
    '.': (65,152,10)
}

offset = 35
for i in range(1, width, offset):
    for j in range(1, height, offset):
        choice = random.randint(1, 10)
        if choice <= 2:  # 20% chance to draw a ~ (water)
            symbol = '~'
        elif choice <= 5:  # 30% chance to draw a # (mountain)
            symbol = '#'
        else:  # 50% chance to draw a . (plain)
            symbol = '.'
        draw.text((i, j), symbol, fill=colors[symbol], font=font)


folder = "images"
prefix = "map_"
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
print("Map generated and saved as:", new_filename)