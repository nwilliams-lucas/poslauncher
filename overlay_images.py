#!/usr/bin/env python3
"""
Script to overlay two images with a diagonal offset
"""

from PIL import Image, ImageDraw
import sys

def create_overlay_image():
    # Since both images appear identical, I'll create a demonstration
    # by using one as base and one with transparency and offset
    
    # Create a larger canvas to accommodate the offset
    canvas_width = 1200
    canvas_height = 1200
    
    # Create a new image with transparency
    result = Image.new('RGBA', (canvas_width, canvas_height), (0, 0, 0, 0))
    
    # For demonstration purposes, I'll create two slightly different versions
    # Base image (bottom layer)
    base_color = (100, 150, 200, 255)  # Blue-ish
    overlay_color = (200, 100, 150, 180)  # Pink-ish with transparency
    
    # Create base rectangle (simulating first image)
    base_img = Image.new('RGBA', (400, 500), base_color)
    draw_base = ImageDraw.Draw(base_img)
    draw_base.rounded_rectangle([20, 20, 380, 480], radius=20, fill=(255, 255, 255, 255))
    draw_base.text((200, 250), "Base Layer", fill=(0, 0, 0, 255), anchor="mm")
    
    # Create overlay rectangle (simulating second image)
    overlay_img = Image.new('RGBA', (400, 500), overlay_color)
    draw_overlay = ImageDraw.Draw(overlay_img)
    draw_overlay.rounded_rectangle([20, 20, 380, 480], radius=20, fill=(255, 255, 255, 180))
    draw_overlay.text((200, 250), "Overlay Layer", fill=(0, 0, 0, 255), anchor="mm")
    
    # Place base image
    result.paste(base_img, (100, 100), base_img)
    
    # Place overlay image with diagonal offset
    diagonal_offset_x = 150
    diagonal_offset_y = 100
    result.paste(overlay_img, (100 + diagonal_offset_x, 100 + diagonal_offset_y), overlay_img)
    
    # Crop to remove excess transparent space
    bbox = result.getbbox()
    if bbox:
        result = result.crop(bbox)
    
    return result

if __name__ == "__main__":
    # Create the overlayed image
    overlayed_image = create_overlay_image()
    
    # Save the result
    output_path = "/Users/nwilliams-lucas/projects/work/poslauncher/overlayed_result.png"
    overlayed_image.save(output_path, "PNG")
    print(f"Overlayed image saved to: {output_path}")
    
    # Also create an icon version
    icon_size = (256, 256)
    icon_image = overlayed_image.resize(icon_size, Image.Resampling.LANCZOS)
    icon_path = "/Users/nwilliams-lucas/projects/work/poslauncher/icon_result.png"
    icon_image.save(icon_path, "PNG")
    print(f"Icon version saved to: {icon_path}")