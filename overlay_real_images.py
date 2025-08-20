#!/usr/bin/env python3
"""
Script to overlay the two provided logo images with a diagonal offset
"""

from PIL import Image
import os

def overlay_logos():
    # Load the two images
    dtlr_path = "/Users/nwilliams-lucas/projects/work/poslauncher/0-dtlr-web-black.png"
    jumpmind_path = "/Users/nwilliams-lucas/projects/work/poslauncher/jumpmind.png"
    
    # Check if files exist
    if not os.path.exists(dtlr_path):
        print(f"Error: {dtlr_path} not found")
        return
    if not os.path.exists(jumpmind_path):
        print(f"Error: {jumpmind_path} not found")
        return
    
    # Load images
    dtlr_img = Image.open(dtlr_path).convert('RGBA')
    jumpmind_img = Image.open(jumpmind_path).convert('RGBA')
    
    print(f"DTLR image size: {dtlr_img.size}")
    print(f"JumpMind image size: {jumpmind_img.size}")
    
    # Calculate canvas size to fit both images with offset
    diagonal_offset_x = 100
    diagonal_offset_y = 80
    
    canvas_width = max(dtlr_img.width, jumpmind_img.width + diagonal_offset_x) + 50
    canvas_height = max(dtlr_img.height, jumpmind_img.height + diagonal_offset_y) + 50
    
    # Create transparent canvas
    result = Image.new('RGBA', (canvas_width, canvas_height), (0, 0, 0, 0))
    
    # Position for base image (DTLR - bottom layer)
    base_x = 25
    base_y = 25
    
    # Position for overlay image (JumpMind - top layer with offset)
    overlay_x = base_x + diagonal_offset_x
    overlay_y = base_y + diagonal_offset_y
    
    # Paste DTLR image first (base layer)
    result.paste(dtlr_img, (base_x, base_y), dtlr_img)
    
    # Paste JumpMind image with diagonal offset (overlay layer)
    result.paste(jumpmind_img, (overlay_x, overlay_y), jumpmind_img)
    
    # Crop to remove excess transparent space
    bbox = result.getbbox()
    if bbox:
        result = result.crop(bbox)
    
    return result

def create_icon_version(image, size=(256, 256)):
    """Create an icon-sized version"""
    return image.resize(size, Image.Resampling.LANCZOS)

if __name__ == "__main__":
    # Create the overlayed image
    overlayed_image = overlay_logos()
    
    if overlayed_image:
        # Save the full result
        output_path = "/Users/nwilliams-lucas/projects/work/poslauncher/logo_overlay.png"
        overlayed_image.save(output_path, "PNG")
        print(f"Overlayed logo saved to: {output_path}")
        
        # Create and save icon version
        icon_image = create_icon_version(overlayed_image)
        icon_path = "/Users/nwilliams-lucas/projects/work/poslauncher/logo_icon.png"
        icon_image.save(icon_path, "PNG")
        print(f"Icon version saved to: {icon_path}")
        
        # Create a banner-sized version (good for splash screens)
        banner_image = overlayed_image.resize((400, 200), Image.Resampling.LANCZOS)
        banner_path = "/Users/nwilliams-lucas/projects/work/poslauncher/logo_banner.png"
        banner_image.save(banner_path, "PNG")
        print(f"Banner version saved to: {banner_path}")
    else:
        print("Failed to create overlayed image")