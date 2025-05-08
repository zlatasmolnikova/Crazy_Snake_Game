using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = System.Object;

public class WallBuilderSimple
{
    private float WallThickness { get; }
    private float PassageThickness { get; }
    private float WallHeight { get; }

    private Material material;


    public WallBuilderSimple(Material material, float wallThickness = 1, float passageThickness = 1, float wallHeight = 1)
    {
        CheckParameter(wallThickness, nameof(wallThickness));
        CheckParameter(passageThickness, nameof(passageThickness));
        CheckParameter(wallHeight, nameof(wallHeight));

        WallThickness = wallThickness;
        PassageThickness = passageThickness;
        WallHeight = wallHeight;
        this.material = material;
    }

    private void CheckParameter(float parameter, string name)
    {
        if (parameter <= 0)
        {
            throw new ArgumentException($"{name} must be > 0");
        }
    }

    public GameObject BuildWalls(MazeCell[,] maze)
    {
        var mazeWallsGameObject = new GameObject("TheMaze");

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int z = 0; z < maze.GetLength(1); z++)
            {
                if (maze[x, z] == MazeCell.Wall)
                {
                    BuildWallPart(x, z, mazeWallsGameObject);
                }
            }
        }

        return mazeWallsGameObject;
    }

    private GameObject BuildWallPart(int x, int z, GameObject mazeWallsGameObject)
    {
        var wallPart = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallPart.GetComponent<Renderer>().material = material;

        wallPart.transform.parent = mazeWallsGameObject.transform;

        var xThickness = WallThickness;
        if (x % 2 == 1)
        {
            xThickness = PassageThickness;
        }

        var zThickness = WallThickness;
        if (z % 2 == 1)
        {
            zThickness = PassageThickness;
        }

        wallPart.transform.localScale = new Vector3(xThickness, WallHeight, zThickness);

        wallPart.transform.localPosition = new Vector3(
                        ((WallThickness + PassageThickness) / 2.0f) * x,
                        WallHeight / 2.0f,
                        ((WallThickness + PassageThickness) / 2.0f) * z
                    );
        return wallPart;
    }
}
