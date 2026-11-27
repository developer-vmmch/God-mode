import sys
import os
sys.path.append(os.path.dirname(__file__))

import bpy
import math
from blender_utils import clear_scene, create_material, assign_material, export_model

def build_motorcycle():
    clear_scene()

    mat_paint = create_material("Mat_Bike_Paint", (0.8, 0.08, 0.08, 1.0), metallic=0.7, roughness=0.25)
    mat_chrome = create_material("Mat_Bike_Chrome", (0.9, 0.9, 0.95, 1.0), metallic=0.95, roughness=0.1)
    mat_black = create_material("Mat_Bike_Black", (0.05, 0.05, 0.06, 1.0), metallic=0.1, roughness=0.6)
    mat_tire = create_material("Mat_Bike_Tire", (0.03, 0.03, 0.03, 1.0), metallic=0.0, roughness=0.85)
    mat_light = create_material("Mat_Bike_Headlight", (1.0, 0.98, 0.85, 1.0), emission=(1.0, 0.98, 0.85, 1.0))

    parts = []

    # Frame / Engine Block
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.50))
    frame = bpy.context.active_object
    frame.name = "Bike_Frame"
    frame.scale = (0.28, 1.10, 0.45)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(frame, mat_black)
    parts.append(frame)

    # Teardrop Fuel Tank (Red)
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.18, depth=0.55, location=(0, 0.25, 0.82))
    tank = bpy.context.active_object
    tank.name = "Bike_FuelTank"
    tank.rotation_euler = (math.radians(80), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(tank, mat_paint)
    parts.append(tank)

    # Contoured Dual Seat
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -0.32, 0.80))
    seat = bpy.context.active_object
    seat.name = "Bike_Seat"
    seat.scale = (0.24, 0.65, 0.12)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(seat, mat_black)
    parts.append(seat)

    # Front Telescopic Fork
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.03, depth=0.85, location=(0, 0.80, 0.60))
    fork = bpy.context.active_object
    fork.name = "Bike_FrontFork"
    fork.rotation_euler = (math.radians(-24), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(fork, mat_chrome)
    parts.append(fork)

    # Circular Chrome Headlight
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.08, depth=0.08, location=(0, 0.98, 0.88))
    hl = bpy.context.active_object
    hl.name = "Bike_Headlight"
    hl.rotation_euler = (math.radians(90), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(hl, mat_light)
    parts.append(hl)

    # Handlebars
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.02, depth=0.68, location=(0, 0.65, 0.98))
    hbars = bpy.context.active_object
    hbars.name = "Bike_Handlebars"
    hbars.rotation_euler = (0, math.radians(90), 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(hbars, mat_chrome)
    parts.append(hbars)

    # Exhaust Silencer (Chrome Pipe)
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.045, depth=0.90, location=(0.18, -0.35, 0.28))
    exhaust = bpy.context.active_object
    exhaust.name = "Bike_Exhaust"
    exhaust.rotation_euler = (math.radians(85), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(exhaust, mat_chrome)
    parts.append(exhaust)

    # Wheels (Front & Rear)
    for y, wname in [(0.95, "Front"), (-0.80, "Rear")]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=20, radius=0.30, depth=0.09, location=(0, y, 0.30))
        wheel = bpy.context.active_object
        wheel.name = f"Bike_Wheel_{wname}"
        wheel.rotation_euler = (0, math.radians(90), 0)
        bpy.ops.object.transform_apply(rotation=True)
        assign_material(wheel, mat_tire)
        parts.append(wheel)

        bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.18, depth=0.10, location=(0, y, 0.30))
        rim = bpy.context.active_object
        rim.name = f"Bike_Rim_{wname}"
        rim.rotation_euler = (0, math.radians(90), 0)
        bpy.ops.object.transform_apply(rotation=True)
        assign_material(rim, mat_chrome)
        parts.append(rim)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = frame
    bpy.ops.object.join()
    frame.name = "CSVS_Motorcycle_Commuter"
    frame.location = (0, 0, 0)

    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

    blend_out = r"D:\Development\CSVS-GodMode\BlenderAssets\Vehicles\Motorcycle.blend"
    fbx_out = r"D:\Development\CSVS-GodMode\UnityProject\Assets\Art\Models\Vehicles\Motorcycle.fbx"
    export_model(blend_out, fbx_out)

if __name__ == "__main__":
    build_motorcycle()
