#!/usr/bin/env python3
"""
Script to convert PNG to ICO format with multiple sizes
"""

from PIL import Image
import os

def create_ico_from_png():
    # Path to the icon PNG
    png_path = "/Users/nwilliams-lucas/projects/work/poslauncher/logo_icon.png"
    
    # Check if PNG exists
    if not os.path.exists(png_path):
        print(f"Error: {png_path} not found")
        return False
    
    # Load the PNG image
    img = Image.open(png_path).convert('RGBA')
    
    # Common ICO sizes (Windows standard)
    ico_sizes = [16, 24, 32, 48, 64, 128, 256]
    
    # Create images for each size
    icon_images = []
    for size in ico_sizes:
        resized = img.resize((size, size), Image.Resampling.LANCZOS)
        icon_images.append(resized)
    
    # Save as ICO file in both project directories
    ico_paths = [
        "/Users/nwilliams-lucas/projects/work/poslauncher/POSLauncher/icon.ico",
        "/Users/nwilliams-lucas/projects/work/poslauncher/POSLauncher.Portable/icon.ico",
        "/Users/nwilliams-lucas/projects/work/poslauncher/icon.ico"  # Root for convenience
    ]
    
    for ico_path in ico_paths:
        # Ensure directory exists
        os.makedirs(os.path.dirname(ico_path), exist_ok=True)
        
        # Save ICO with multiple sizes
        icon_images[0].save(
            ico_path,
            format='ICO',
            sizes=[(size, size) for size in ico_sizes],
            append_images=icon_images[1:]
        )
        print(f"ICO file created: {ico_path}")
    
    return True

if __name__ == "__main__":
    if create_ico_from_png():
        print("Successfully converted PNG to ICO format with multiple sizes")
    else:
        print("Failed to create ICO file")