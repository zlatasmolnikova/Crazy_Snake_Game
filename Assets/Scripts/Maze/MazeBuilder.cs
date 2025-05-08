using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Mazes
{
    public class MazeBuilder: MonoBehaviour
    {
        public bool IgnoreSeed = true;

        public int Seed = 42;

        public float PlacementThreshold = 0.5f;

        public float WallThickness = 1;

        public float PassageThickness = 1;

        public float WallHeight = 1;

        public Material WallMaterial;

        public int MazeSize_X = 10;

        public int MazeSize_Z = 10;
        
        public int CentralRoomSize_X = 4;

        public int CentralRoomSize_Z = 4;
        
        public GameObject Environment;
        
        public MazeCell[,] Maze { get; private set; }

        public UnityEvent OnFinishedBuildingMaze;

        public void Start()
        {
            Maze = GenerateMaze();

            var mazeWallsObject = Build();
            
            mazeWallsObject.transform.parent = Environment.transform;

            StartCoroutine(Finish());
            // Destroy(gameObject);    //remove mazeBuilder from scene
        }

        private IEnumerator Finish()
        {
            yield return new WaitForEndOfFrame();
            OnFinishedBuildingMaze?.Invoke();
        }

        private MazeCell[,] GenerateMaze()
        {
            var patternGenerator = new MazePatternGenerator();
            // var maze = patternGenerator.RandomizedDFS(MazeSize_X, MazeSize_Z, IgnoreSeed ? null : Seed);
            var maze = patternGenerator.RandomizedLinear(MazeSize_X, MazeSize_Z, PlacementThreshold,
                IgnoreSeed ? null : Seed);
            
            patternGenerator.MakeCentralRoom(CentralRoomSize_X, CentralRoomSize_Z, maze);
            patternGenerator.MakeExit(maze, IgnoreSeed ? null : Seed);

            return maze;
        }

        private GameObject Build()
        {
            var wallBuilder = new WallBuilderSimple(WallMaterial, WallThickness, PassageThickness, WallHeight);

            var mazeWallsObject = wallBuilder.BuildWalls(Maze);
            mazeWallsObject.transform.position = transform.position;

            return mazeWallsObject;
        }
    }
}
