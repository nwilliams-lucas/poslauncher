#!/usr/bin/env python3
"""
Script to create various banner sizes from the DTLR image
"""

from PIL import Image
import os

def create_banner_sizes():
    # Path to the source image
    source_path = "/Users/nwilliams-lucas/projects/work/poslauncher/DTLR-1920x1080.jpg"
    
    # Check if source exists
    if not os.path.exists(source_path):
        print(f"Error: {source_path} not found")
        return False
    
    # Load the source image
    source_img = Image.open(source_path)
    print(f"Source image size: {source_img.size}")
    
    # Define banner sizes for different uses
    banner_sizes = {
        "splash_screen": (800, 450),      # 16:9 ratio for splash screens
        "header_banner": (600, 200),      # Wide banner for app headers
        "compact_banner": (400, 133),     # Compact 3:1 ratio
        "square_banner": (400, 400),      # Square version
        "wide_banner": (960, 320),        # Ultra-wide for large displays
        "mobile_banner": (320, 180),      # Mobile-friendly size
    }
    
    # Create output directory
    banner_dir = "/Users/nwilliams-lucas/projects/work/poslauncher/banners"
    os.makedirs(banner_dir, exist_ok=True)
    
    results = []
    
    for name, size in banner_sizes.items():
        # Resize image maintaining aspect ratio and crop if needed
        resized_img = resize_and_crop(source_img, size)
        
        # Save as PNG and JPG
        png_path = os.path.join(banner_dir, f"dtlr_banner_{name}.png")
        jpg_path = os.path.join(banner_dir, f"dtlr_banner_{name}.jpg")
        
        resized_img.save(png_path, "PNG", quality=95)
        resized_img.convert('RGB').save(jpg_path, "JPEG", quality=95)
        
        results.append((name, size, png_path, jpg_path))
        print(f"Created {name}: {size} -> {png_path}")
    
    # Also copy to project directories
    copy_to_projects(banner_dir)
    
    return results

def resize_and_crop(image, target_size):
    """Resize image to target size, cropping if necessary to maintain aspect ratio"""
    target_width, target_height = target_size
    
    # Calculate scaling to fit target size
    width_ratio = target_width / image.width
    height_ratio = target_height / image.height
    
    # Use the larger ratio to ensure we fill the target size
    scale = max(width_ratio, height_ratio)
    
    # Calculate new size
    new_width = int(image.width * scale)
    new_height = int(image.height * scale)
    
    # Resize image
    resized = image.resize((new_width, new_height), Image.Resampling.LANCZOS)
    
    # Calculate crop box to center the image
    left = (new_width - target_width) // 2
    top = (new_height - target_height) // 2
    right = left + target_width
    bottom = top + target_height
    
    # Crop to exact target size
    cropped = resized.crop((left, top, right, bottom))
    
    return cropped

def copy_to_projects(banner_dir):
    """Copy banner images to project directories"""
    project_dirs = [
        "/Users/nwilliams-lucas/projects/work/poslauncher/POSLauncher/Images",
        "/Users/nwilliams-lucas/projects/work/poslauncher/POSLauncher.Portable/Images",
    ]
    
    # Copy the most commonly used banners
    common_banners = ["splash_screen", "header_banner", "compact_banner"]
    
    for project_dir in project_dirs:
        os.makedirs(project_dir, exist_ok=True)
        
        for banner_name in common_banners:
            src_png = os.path.join(banner_dir, f"dtlr_banner_{banner_name}.png")
            src_jpg = os.path.join(banner_dir, f"dtlr_banner_{banner_name}.jpg")
            
            if os.path.exists(src_png):
                dst_png = os.path.join(project_dir, f"dtlr_banner_{banner_name}.png")
                dst_jpg = os.path.join(project_dir, f"dtlr_banner_{banner_name}.jpg")
                
                # Copy files
                import shutil
                shutil.copy2(src_png, dst_png)
                shutil.copy2(src_jpg, dst_jpg)
                print(f"Copied banners to {project_dir}")

if __name__ == "__main__":
    results = create_banner_sizes()
    if results:
        print(f"\nSuccessfully created {len(results)} banner variations:")
        for name, size, png_path, jpg_path in results:
            print(f"  {name}: {size[0]}x{size[1]}")
        print(f"\nAll banners saved in: /Users/nwilliams-lucas/projects/work/poslauncher/banners/")
        print("Common banners copied to project Images/ directories")
    else:
        print("Failed to create banners")