import sys
import os
sys.path.append(os.path.dirname(__file__))

import bpy
import math
from blender_utils import clear_scene, create_material, assign_material, export_model

def build_commercial_building():
    clear_scene()

    # Materials
    mat_wall = create_material("Mat_Bazaar_Plaster", (0.92, 0.88, 0.80, 1.0), metallic=0.05, roughness=0.75) # Warm Tamil limestone
    mat_accent = create_material("Mat_Bazaar_Trim", (0.15, 0.45, 0.65, 1.0), metallic=0.1, roughness=0.5)   # French colonial cyan/blue
    mat_shutter = create_material("Mat_Shop_Shutter", (0.75, 0.77, 0.80, 1.0), metallic=0.85, roughness=0.35) # Corrugated metal shutter
    mat_signboard = create_material("Mat_Shop_Signboard", (0.85, 0.15, 0.15, 1.0), metallic=0.1, roughness=0.4) # Bright Tamil retail red
    mat_chajja = create_material("Mat_Chajja_Sunshade", (0.78, 0.74, 0.68, 1.0), metallic=0.05, roughness=0.8) # Concrete sunshade
    mat_tank = create_material("Mat_Sintex_WaterTank", (0.05, 0.05, 0.05, 1.0), metallic=0.0, roughness=0.6)  # Signature black Sintex
    mat_glass = create_material("Mat_Window_Glass", (0.2, 0.35, 0.45, 1.0), metallic=0.6, roughness=0.1)

    parts = []

    # 1. Main 3-Storey Building Structure (Width: 10m, Depth: 12m, Height: 11m)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 5.5))
    main_block = bpy.context.active_object
    main_block.name = "Bazaar_Main_Block"
    main_block.scale = (10.0, 12.0, 11.0)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(main_block, mat_wall)
    parts.append(main_block)

    # 2. Ground Floor Commercial Shop Shutters (3 Shop units in front)
    for i, x in enumerate([-3.2, 0.0, 3.2]):
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(x, 6.02, 1.75))
        shutter = bpy.context.active_object
        shutter.name = f"Shop_Shutter_{i+1}"
        shutter.scale = (2.6, 0.08, 3.2)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(shutter, mat_shutter)
        parts.append(shutter)

        # Ground Floor Chajja (Concrete sunshade canopy over each shop)
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(x, 6.45, 3.45))
        canopy = bpy.context.active_object
        canopy.name = f"Shop_Canopy_{i+1}"
        canopy.scale = (2.9, 0.85, 0.12)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(canopy, mat_chajja)
        parts.append(canopy)

    # 3. Bilingual Commercial Signboard across frontage
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 6.12, 4.0))
    sign = bpy.context.active_object
    sign.name = "Bazaar_Signboard_Fascia"
    sign.scale = (9.5, 0.15, 0.85)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(sign, mat_signboard)
    parts.append(sign)

    # 4. Upper Floor Windows with Decorative Plaster Surrounds and Sunshades
    # Floor 2 (Z = 5.8m) & Floor 3 (Z = 8.8m)
    for z in [5.8, 8.8]:
        for x in [-3.2, 0.0, 3.2]:
            # Window pane
            bpy.ops.mesh.primitive_cube_add(size=1.0, location=(x, 6.02, z))
            win = bpy.context.active_object
            win.name = f"Window_F{int(z)}_{x}"
            win.scale = (1.6, 0.06, 1.8)
            bpy.ops.object.transform_apply(scale=True)
            assign_material(win, mat_glass)
            parts.append(win)

            # Window Chajja
            bpy.ops.mesh.primitive_cube_add(size=1.0, location=(x, 6.35, z + 1.0))
            wchajja = bpy.context.active_object
            wchajja.name = f"Window_Chajja_{int(z)}_{x}"
            wchajja.scale = (1.9, 0.65, 0.08)
            bpy.ops.object.transform_apply(scale=True)
            assign_material(wchajja, mat_chajja)
            parts.append(wchajja)

    # 5. Colonial Roof Parapet Railing (Height: 11.0m -> 12.0m)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 11.5))
    parapet = bpy.context.active_object
    parapet.name = "Rooftop_Parapet"
    parapet.scale = (10.2, 12.2, 1.0)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(parapet, mat_accent)
    parts.append(parapet)

    # 6. Rooftop Details: Sintex Water Tanks (2 Tanks on concrete plinths)
    for x in [-2.5, 2.5]:
        # Concrete plinth
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(x, -3.0, 11.3))
        plinth = bpy.context.active_object
        plinth.name = f"Tank_Plinth_{x}"
        plinth.scale = (1.6, 1.6, 0.4)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(plinth, mat_chajja)
        parts.append(plinth)

        # Cylindrical Sintex Tank
        bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.65, depth=1.3, location=(x, -3.0, 12.15))
        tank = bpy.context.active_object
        tank.name = f"Sintex_WaterTank_{x}"
        assign_material(tank, mat_tank)
        parts.append(tank)

    # Join into single unified architecture mesh
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = main_block
    bpy.ops.object.join()
    main_block.name = "CSVS_Karaikal_Commercial_Bazaar"
    main_block.location = (0, 0, 0)

    # Generate UVs
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

    # Export
    blend_out = r"D:\Development\CSVS-GodMode\BlenderAssets\Architecture\Commercial_Bazaar.blend"
    fbx_out = r"D:\Development\CSVS-GodMode\UnityProject\Assets\Art\Models\Architecture\Commercial_Bazaar.fbx"
    export_model(blend_out, fbx_out)

if __name__ == "__main__":
    build_commercial_building()
