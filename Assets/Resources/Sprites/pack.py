import os
from PIL import Image

def create_sprite_sheet(images, output_path):
    """
    Create a sprite sheet from a list of images.
    
    Args:
    images (list): List of PIL Image objects.
    output_path (str): Path to save the sprite sheet.
    """
    if not images:
        print("No images to process.")
        return

    total_width = sum(image.width for image in images)
    max_height = max(image.height for image in images)

    sprite_sheet = Image.new("RGBA", (total_width, max_height))

    current_x = 0
    for image in images:
        sprite_sheet.paste(image, (current_x, 0))
        current_x += image.width

    sprite_sheet.save(output_path)
    print(f"Sprite sheet saved to {output_path}")

def process_directory(directory):
    """
    Process each subdirectory within the given directory to create sprite sheets.
    
    Args:
    directory (str): Root directory containing subdirectories with images.
    """
    for folder_name in os.listdir(directory):
        folder_path = os.path.join(directory, folder_name)
        if os.path.isdir(folder_path):
            images = [Image.open(os.path.join(folder_path, fname)) for fname in os.listdir(folder_path) if fname.endswith(('.png', '.jpg', '.jpeg'))]
            output_path = os.path.join(directory, f"{folder_name.lower()}.png")
            create_sprite_sheet(images, output_path)

if __name__ == '__main__':
    root_directory = '.'
    process_directory(root_directory)
