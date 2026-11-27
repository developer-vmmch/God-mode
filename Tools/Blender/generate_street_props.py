import sys
import os
sys.path.append(os.path.dirname(__file__))

import bpy
import math
from blender_utils import clear_scene, create_material, assign_material, export_model

def build_street_props():
    clear_scene()

    mat_pole = create_material("Mat_Pole_Concrete", (0.75, 0.72, 0.68, 1.0), metallic=0.05, roughness=0.85)
    mat_xform = create_material("Mat_Transformer_Steel", (0.35, 0.40, 0.38, 1.0), metallic=0.7, roughness=0.35)
    mat_insulator = create_material("Mat_Ceramic_Bell", (0.65, 0.30, 0.15, 1.0), metallic=0.1, roughness=0.2)
    mat_lamp = create_material("Mat_StreetLamp_Light", (1.0, 0.95, 0.75, 1.0), emission=(1.0, 0.95, 0.75, 1.0))

    parts = []

    # Concrete Pole (Height: 8.5m)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 4.25))
    pole = bpy.context.active_object
    pole.name = "Prop_Concrete_Pole"
    pole.scale = (0.28, 0.28, 8.5)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(pole, mat_pole)
    parts.append(pole)

    # Step-down Transformer (Cylindrical Tank)
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.35, depth=0.85, location=(0.42, 0, 6.2))
    xf = bpy.context.active_object
    xf.name = "Prop_Transformer"
    assign_material(xf, mat_xform)
    parts.append(xf)

    # Crossarms (Horizontal channel beams)
    for z in [7.2, 8.0]:
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, z))
        arm = bpy.context.active_object
        arm.name = f"Prop_Crossarm_{z}"
        arm.scale = (2.2, 0.10, 0.10)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(arm, mat_xform)
        parts.append(arm)

        # Ceramic Insulator Bells
        for x in [-0.95, 0.0, 0.95]:
            bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.06, depth=0.14, location=(x, 0, z + 0.12))
            ins = bpy.context.active_object
            ins.name = f"Prop_Insulator_{z}_{x}"
            assign_material(ins, mat_insulator)
            parts.append(ins)

    # Streetlight Arm & Fixture
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.03, depth=1.6, location=(0.65, 0, 7.8))
    larm = bpy.context.active_object
    larm.name = "Prop_LampArm"
    larm.rotation_euler = (0, math.radians(55), 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(larm, mat_xform)
    parts.append(larm)

    # Glowing Lamp Fixture
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(1.35, 0, 7.3))
    lhead = bpy.context.active_object
    lhead.name = "Prop_LampHead"
    lhead.scale = (0.35, 0.20, 0.12)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(lhead, mat_lamp)
    parts.append(lhead)

    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = pole
    bpy.ops.object.join()
    pole.name = "CSVS_UtilityPole_Transformer"
    pole.location = (0, 0, 0)

    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

    blend_out = r"D:\Development\CSVS-GodMode\BlenderAssets\Props\UtilityPole.blend"
    fbx_out = r"D:\Development\CSVS-GodMode\UnityProject\Assets\Art\Models\Props\UtilityPole.fbx"
    export_model(blend_out, fbx_out)

if __name__ == "__main__":
    build_street_props()
