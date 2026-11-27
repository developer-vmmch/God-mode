import sys
import os
sys.path.append(os.path.dirname(__file__))

import bpy
import math
from blender_utils import clear_scene, create_material, assign_material, export_model

def build_hero():
    clear_scene()
    
    # Materials
    mat_navy = create_material("Mat_Hero_Navy", (0.05, 0.08, 0.16, 1.0), metallic=0.75, roughness=0.25)
    mat_cobalt = create_material("Mat_Hero_Cobalt", (0.02, 0.15, 0.40, 1.0), metallic=0.6, roughness=0.3)
    mat_silver = create_material("Mat_Hero_Silver", (0.85, 0.88, 0.92, 1.0), metallic=0.9, roughness=0.15)
    mat_cyan = create_material("Mat_Hero_CyanGlow", (0.0, 0.9, 1.0, 1.0), emission=(0.0, 0.95, 1.0, 1.0))
    mat_cape = create_material("Mat_Hero_Cape", (0.03, 0.05, 0.12, 1.0), metallic=0.1, roughness=0.8)

    parts = []

    # Head / Helmet
    bpy.ops.mesh.primitive_uv_sphere_add(segments=24, ring_count=16, radius=0.12, location=(0, 0, 1.75))
    helmet = bpy.context.active_object
    helmet.name = "Hero_Helmet"
    helmet.scale = (0.9, 1.05, 1.15)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(helmet, mat_navy)
    parts.append(helmet)

    # Visor (Cyan Glow)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0.09, 1.76))
    visor = bpy.context.active_object
    visor.name = "Hero_Visor"
    visor.scale = (0.16, 0.04, 0.035)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(visor, mat_cyan)
    parts.append(visor)

    # Torso / Cuirass (Chest armor)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 1.45))
    torso = bpy.context.active_object
    torso.name = "Hero_Torso"
    torso.scale = (0.42, 0.24, 0.40)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(torso, mat_navy)
    parts.append(torso)

    # Chest Insignia / Arc Core
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.06, depth=0.03, location=(0, 0.125, 1.48))
    core = bpy.context.active_object
    core.name = "Hero_ChestCore"
    core.rotation_euler = (math.radians(90), 0, 0)
    bpy.ops.object.transform_apply(rotation=True)
    assign_material(core, mat_cyan)
    parts.append(core)

    # Silver Chest Chevrons
    for side in (-1, 1):
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.10, 0.125, 1.54))
        chev = bpy.context.active_object
        chev.name = f"Hero_Chevron_{'R' if side > 0 else 'L'}"
        chev.scale = (0.09, 0.015, 0.025)
        chev.rotation_euler = (0, 0, math.radians(-side * 25))
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        assign_material(chev, mat_silver)
        parts.append(chev)

    # Abdomen / Waist
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 1.15))
    waist = bpy.context.active_object
    waist.name = "Hero_Waist"
    waist.scale = (0.32, 0.20, 0.22)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(waist, mat_cobalt)
    parts.append(waist)

    # Pelvis
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.96))
    pelvis = bpy.context.active_object
    pelvis.name = "Hero_Pelvis"
    pelvis.scale = (0.34, 0.22, 0.18)
    bpy.ops.object.transform_apply(scale=True)
    assign_material(pelvis, mat_navy)
    parts.append(pelvis)

    # Shoulders / Pauldrons & Arms
    for side in (-1, 1):
        prefix = 'R' if side > 0 else 'L'
        # Pauldron
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.28, 0, 1.60))
        pld = bpy.context.active_object
        pld.name = f"Hero_Pauldron_{prefix}"
        pld.scale = (0.16, 0.18, 0.12)
        pld.rotation_euler = (0, math.radians(-side * 15), 0)
        bpy.ops.object.transform_apply(scale=True, rotation=True)
        assign_material(pld, mat_silver)
        parts.append(pld)

        # Upper Arm
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.065, depth=0.28, location=(side * 0.26, 0, 1.38))
        uarm = bpy.context.active_object
        uarm.name = f"Hero_UpperArm_{prefix}"
        assign_material(uarm, mat_cobalt)
        parts.append(uarm)

        # Forearm / Gauntlet
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.06, depth=0.26, location=(side * 0.27, 0, 1.12))
        farm = bpy.context.active_object
        farm.name = f"Hero_Forearm_{prefix}"
        assign_material(farm, mat_navy)
        parts.append(farm)

        # Gauntlet Power Node
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.33, 0, 1.12))
        gnode = bpy.context.active_object
        gnode.name = f"Hero_GauntletNode_{prefix}"
        gnode.scale = (0.02, 0.04, 0.14)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(gnode, mat_cyan)
        parts.append(gnode)

        # Hand
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.27, 0, 0.94))
        hand = bpy.context.active_object
        hand.name = f"Hero_Hand_{prefix}"
        hand.scale = (0.07, 0.10, 0.10)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(hand, mat_silver)
        parts.append(hand)

        # Thigh
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.09, depth=0.42, location=(side * 0.13, 0, 0.68))
        thigh = bpy.context.active_object
        thigh.name = f"Hero_Thigh_{prefix}"
        assign_material(thigh, mat_navy)
        parts.append(thigh)

        # Knee Guard
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.13, 0.08, 0.48))
        knee = bpy.context.active_object
        knee.name = f"Hero_KneeGuard_{prefix}"
        knee.scale = (0.11, 0.04, 0.08)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(knee, mat_silver)
        parts.append(knee)

        # Shin / Boot
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.075, depth=0.44, location=(side * 0.13, 0, 0.24))
        shin = bpy.context.active_object
        shin.name = f"Hero_Shin_{prefix}"
        assign_material(shin, mat_cobalt)
        parts.append(shin)

        # Foot
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(side * 0.13, 0.05, 0.04))
        foot = bpy.context.active_object
        foot.name = f"Hero_Foot_{prefix}"
        foot.scale = (0.10, 0.22, 0.08)
        bpy.ops.object.transform_apply(scale=True)
        assign_material(foot, mat_silver)
        parts.append(foot)

    # Dynamic Cape
    bpy.ops.mesh.primitive_plane_add(size=1.0, location=(0, -0.14, 1.05))
    cape = bpy.context.active_object
    cape.name = "Hero_Cape"
    cape.scale = (0.42, 0.01, 1.05)
    cape.rotation_euler = (math.radians(8), 0, 0)
    bpy.ops.object.transform_apply(scale=True, rotation=True)
    assign_material(cape, mat_cape)
    parts.append(cape)

    # Join into a single unified hero mesh or leave hierarchical
    # For clean Unity import, join into one mesh while keeping material slots
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:
        p.select_set(True)
    bpy.context.view_layer.objects.active = torso
    bpy.ops.object.join()
    torso.name = "CSVS_Superhero_Heroic"
    torso.location = (0, 0, 0)

    # Generate UVs
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='SELECT')
    bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
    bpy.ops.object.mode_set(mode='OBJECT')

    # Export
    blend_out = r"D:\Development\CSVS-GodMode\BlenderAssets\Hero\Hero_Armored.blend"
    fbx_out = r"D:\Development\CSVS-GodMode\UnityProject\Assets\Art\Models\Hero\Hero_Armored.fbx"
    export_model(blend_out, fbx_out)

if __name__ == "__main__":
    build_hero()
