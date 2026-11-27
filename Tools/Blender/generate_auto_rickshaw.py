import sys
import os
sys.path.append(os.path.dirname(__file__))

import bpy
import math
from blender_utils import clear_scene, create_material, assign_material, export_model

def build_auto_rickshaw():
    clear_scene()

    # Materials
    mat_yellow = create_material("Mat_Rickshaw_Yellow", (0.95, 0.75, 0.05, 1.0), metallic=0.1, roughness=0.35)
    mat_black = create_material("Mat_Rickshaw_BlackTub", (0.08, 0.08, 0.09, 1.0), metallic=0.2, roughness=0.5)
    mat_chrome = create_material("Mat_Rickshaw_Chrome", (0.9, 0.9, 0.95, 1.0), metallic=0.95, roughness=0.1)
    mat_tire = create_material("Mat_Rickshaw_Tire", (0.03, 0.03, 0.03, 1.0), metallic=0.0, roughness=0.85)
    mat_glass = create_material("Mat_Rickshaw_Glass", (0.7, 0.85, 0.95, 0.5), metallic=0.1, roughness=0.05)
    mat_light = create_material("Mat_Rickshaw_Headlight", (1.0, 0.98, 0.8, 1.0), emission=(1.0, 0.98, 0.85, 1.0))
    mat_rearlight = create_material("Mat_Rickshaw_TailLight", (0.9, 0.05, 0.05, 1.0), emission=(0.95, 0.05, 0.05, 1.0))

    parts = []

    # 1. Lower Passenger Tub / Body (Black)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.55))
    tub = bpy.context.active_object
    tub.name = "Rickshaw_Body_Tub"
    tub.scale = (1.30, 2.50, 0.50)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(tub, mat_black)
    parts.append(tub)

    # 2. Yellow Canvas Canopy (Roof)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -0.15, 1.35))
    roof = bpy.context.active_object
    roof.name = "Rickshaw_Roof_Canopy"
    roof.scale = (1.28, 2.10, 0.65)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(roof, mat_yellow)
    parts.append(roof)

    # 3. Slanted Front Nose (Driver Front Apron - Yellow)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 1.15, 0.75))
    nose = bpy.context.active_object
    nose.name = "Rickshaw_Front_Nose"
    nose.scale = (1.05, 0.45, 0.55)
    nose.rotation_euler = (math.radians(-25), 0, 0)
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    assign_material(nose, mat_yellow)
    parts.append(nose)

    # 4. Windshield Glass
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0.95, 1.25))
    windshield = bpy.context.active_object
    windshield.name = "Rickshaw_Windshield"
    windshield.scale = (1.10, 0.04, 0.42)
    windshield.rotation_euler = (math.radians(-18), 0, 0)
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    assign_material(windshield, mat_glass)
    parts.append(windshield)

    # 5. Front Chrome Mudguard & Single Front Fork
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.04, depth=0.65, location=(0, 1.25, 0.45))
    fork = bpy.context.active_object
    fork.name = "Rickshaw_Front_Fork"
    fork.rotation_euler = (math.radians(-15), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(fork, mat_chrome)
    parts.append(fork)

    # 6. Center Circular Chrome Headlight
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.10, depth=0.08, location=(0, 1.40, 0.72))
    hl = bpy.context.active_object
    hl.name = "Rickshaw_Headlight"
    hl.rotation_euler = (math.radians(90), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(hl, mat_light)
    parts.append(hl)

    # Headlight chrome ring guard
    bpy.ops.mesh.primitive_torus_add(major_radius=0.11, minor_radius=0.015, location=(0, 1.44, 0.72))
    hl_ring = bpy.context.active_object
    hl_ring.name = "Rickshaw_Headlight_Ring"
    hl_ring.rotation_euler = (math.radians(90), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(hl_ring, mat_chrome)
    parts.append(hl_ring)

    # 7. Wheels (3 wheels total: 1 front center, 2 rear side)
    # Front Wheel
    bpy.ops.mesh.primitive_cylinder_add(vertices=20, radius=0.22, depth=0.12, location=(0, 1.32, 0.22))
    fw = bpy.context.active_object
    fw.name = "Rickshaw_Wheel_Front"
    fw.rotation_euler = (0, math.radians(90), 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(fw, mat_tire)
    parts.append(fw)

    # Front Rim
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.12, depth=0.13, location=(0, 1.32, 0.22))
    frim = bpy.context.active_object
    frim.name = "Rickshaw_Rim_Front"
    frim.rotation_euler = (0, math.radians(90), 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(frim, mat_chrome)
    parts.append(frim)

    # Rear Wheels (Left and Right)
    for side in (-1, 1):
        sname = "R" if side > 0 else "L"
        bpy.ops.mesh.primitive_cylinder_add(vertices=20, radius=0.22, depth=0.12, location=(side * 0.68, -0.65, 0.22))
        rw = bpy.context.active_object
        rw.name = f"Rickshaw_Wheel_{sname}"
        rw.rotation_euler = (0, math.radians(90), 0)
        bpy.ops.object.transform_apply(rotation=True)
        assign_material(rw, mat_tire)
        parts.append(rw)

        # Rear Rim
        bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.12, depth=0.13, location=(side * 0.68, -0.65, 0.22))
        rrim = bpy.context.active_object
        rrim.name = f"Rickshaw_Rim_{sname}"
        rrim.rotation_euler = (0, math.radians(90), 0)
        bpy.ops.object.transform_apply(rotation=True)
        assign_material(rrim, mat_chrome)
        parts.append(rrim)

    # 8. Rear Tail Lights
    for side in (-1, 1):
        sname = "R" if side > 0 else "L"
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.52, -1.26, 0.52))
        tl = bpy.context.active_object
        tl.name = f"Rickshaw_TailLight_{sname}"
        tl.scale = (0.10, 0.03, 0.08)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(tl, mat_rearlight)
        parts.append(tl)

    # 9. Handlebars & Dashboard
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.025, depth=0.60, location=(0, 0.75, 0.95))
    hbars = bpy.context.active_object
    hbars.name = "Rickshaw_Handlebars"
    hbars.rotation_euler = (0, math.radians(90), 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(hbars, mat_black)
    parts.append(hbars)

    # Join into single unified vehicle mesh
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = tub
    bpy.ops.object.join()
    tub.name = "CSVS_AutoRickshaw_Detailed"
    tub.location = (0, 0, 0)

    # Generate UVs
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

    # Export
    blend_out = r"D:\Development\CSVS-GodMode\BlenderAssets\Vehicles\AutoRickshaw.blend"
    fbx_out = r"D:\Development\CSVS-GodMode\UnityProject\Assets\Art\Models\Vehicles\AutoRickshaw.fbx"
    export_model(blend_out, fbx_out)

if __name__ == "__main__":
    build_auto_rickshaw()
