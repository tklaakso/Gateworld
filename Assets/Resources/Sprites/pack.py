import os
from PIL import Image

def pack_sprites_to_spritesheet(directory):
    for folder in os.listdir(directory):
        folder_path = os.path.join(directory, folder)
        if os.path.isdir(folder_path):
            sprite_files = [f for f in os.listdir(folder_path) if f.endswith('.png')]
            if not sprite_files:
                continue

            sprite_size = (32, 32)
            sprites_per_row = int(len(sprite_files)**0.5) + 1
            sheet_size = (sprites_per_row * sprite_size[0], sprites_per_row * sprite_size[1])
            spritesheet = Image.new('RGBA', sheet_size)

            for i, sprite_file in enumerate(sprite_files):
                sprite = Image.open(os.path.join(folder_path, sprite_file))
                row = i // sprites_per_row
                col = i % sprites_per_row
                position = (col * sprite_size[0], row * sprite_size[1])
                spritesheet.paste(sprite, position)

            output_path = os.path.join(directory, f'{folder.lower()}.png')
            spritesheet.save(output_path)
            print(f'Saved spritesheet: {output_path}')

if __name__ == "__main__":
    directory = '.'
    pack_sprites_to_spritesheet(directory)