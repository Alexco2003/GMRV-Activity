import torch
import torch.nn as nn
import matplotlib.pyplot as plt
import os
import random
from mpl_toolkits.axes_grid1 import make_axes_locatable

class Generator(nn.Module):
    def __init__(self):
        super(Generator, self).__init__()
        self.model = nn.Sequential(
            nn.Linear(100, 256),  # First linear layer (input: 100 -> 256)
            nn.ReLU(),            # Activation function
            nn.Linear(256, 512),  # Second linear layer (256 -> 512)
            nn.ReLU(),            # Activation function
            nn.Linear(512, 1024), # Third linear layer (512 -> 1024)
            nn.ReLU(),            # Activation function
            nn.Linear(1024, 64*64),  # Final layer (1024 -> 64x64 terrain)
            nn.Tanh()             # Activation function for output (-1 to 1)
        )

    def forward(self, x):
        return self.model(x).view(-1, 64, 64)


def interpolate_latent_vectors(generator, noise_start, noise_end, steps=10):
    generator.eval()

    terrains = []

    with torch.no_grad():
        for i in range(steps):
            # Calculăm alpha (0 → 1)
            alpha = i / (steps - 1)

            # Interpolare liniară
            noise_interpolated = (1 - alpha) * noise_start + alpha * noise_end

            # Generăm terenul
            terrain = generator(noise_interpolated)

            terrains.append(terrain[0])

    return terrains


def main():
    generator = Generator()

    seed_A = random.randint(0, 100000)
    seed_B = random.randint(0, 100000)

    torch.manual_seed(seed_A)
    noise_start = torch.randn(1, 100)

    torch.manual_seed(seed_B)
    noise_end = torch.randn(1, 100)

    num_steps = 10
    generated_terrains = interpolate_latent_vectors(generator, noise_start, noise_end, steps=num_steps)

    print(f"Vector Interpolation | Seed A: {seed_A} -> Seed B: {seed_B}")


    while True:
        choice = input("Enter the generation mode (0 = Unclamped, 1 = Clamped): ")
        if choice in ['0', '1']:
            is_clamped = (choice == '1')
            break
        print("Please enter only 0 or 1.")

    if is_clamped:
        fig_width = 18
        w_space = 0.1
    else:
        fig_width = 22
        w_space = 0.4

    fig, axes = plt.subplots(1, num_steps, figsize=(fig_width, 3.0))
    im = None

    for i in range(num_steps):
        terrain_img = generated_terrains[i].numpy()
        alpha_val = i / (num_steps - 1)

        if is_clamped:
            im = axes[i].imshow(terrain_img, cmap='terrain', vmin=-1, vmax=1)
        else:
            im = axes[i].imshow(terrain_img, cmap='terrain')

        axes[i].axis('off')
        axes[i].set_title(f"Step {i + 1}\nAlpha: {alpha_val:.2f}")

        if not is_clamped:
            divider = make_axes_locatable(axes[i])
            cax = divider.append_axes("right", size="5%", pad=0.15)
            cbar = fig.colorbar(im, cax=cax)
            if i == num_steps - 1:
                cbar.set_label('Altitude')


    if is_clamped:
        plt.subplots_adjust(left=0.02, right=0.88, top=0.88, bottom=0.05, wspace=w_space)
        cbar_ax = fig.add_axes((0.92, 0.05, 0.015, 0.85))
        cbar = fig.colorbar(im, cax=cbar_ax)
        cbar.set_label('Altitude')
    else:
        plt.subplots_adjust(left=0.02, right=0.95, top=0.88, bottom=0.05, wspace=w_space)

    fig.suptitle(f"Latent Space Interpolation: Seed A ({seed_A}) -> Seed B ({seed_B})", fontweight='bold')



    folder = "images"

    if is_clamped:
        prefix = "interpolation_clamped_"
    else:
        prefix = "interpolation_unclamped_"

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

    plt.savefig(new_filename)
    print("Interpolation saved as:", new_filename)

    plt.show()


if __name__ == "__main__":
    main()