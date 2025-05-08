using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store layers - in case layers will be changed, they can be changed in one place
/// try to access layers masks from here pls
/// </summary>
public static class LayersStorage
{
    public static LayerMask Pierceable = LayerMask.GetMask("MobSoft", "ProjectileSoft");

    public static LayerMask NotPierceableObstacles = LayerMask.GetMask("Default", "MazeWalls", "MobHard", "ProjectileHard");

    public static LayerMask NotHurtableHard = LayerMask.GetMask("Default", "MazeWalls");

    public static LayerMask PossiblyHurtables = LayerMask.GetMask("MobSoft", "ProjectileSoft", "MobHard", "ProjectileHard", "ProjectileSemiHard");
}
