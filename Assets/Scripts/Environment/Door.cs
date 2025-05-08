using System;
using UnityEngine;

namespace Environment
{
    public class Door : MonoBehaviour, IPlacer
    {
        private Animator animator;
        private bool isOpen;

        [SerializeField]
        private NotificationManager notificationManager;

        private string OnPlayerInteractWithoutKeyMsg = "Заперто. Надо попробывать найти ключ";

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Interact()
        {
            if (enabled == false)
            {
                notificationManager?.Notify(OnPlayerInteractWithoutKeyMsg);
                return;
            }

            if (isOpen)
                Close();
            else
                Open();
            isOpen = !isOpen;
        }

        private void Open()
        {
            animator.SetBool("isOpen", true);
        }

        private void Close()
        {
            animator.SetBool("isOpen", false);
        }

        public void Place(PlacementManager manager)
        {
            var position = FindPosition(manager.MazeBuilder.Maze);
            if (position == Vector2Int.zero)
                return;
            
            transform.position = manager.GetTransformPosition(position);
            transform.parent = manager.MazeBuilder.Environment.transform;
            if (position.x == 0)
                transform.Rotate(Vector3.up, 90);
            else if (position.x == manager.MazeBuilder.Maze.GetLength(0) - 1)
                transform.Rotate(Vector3.up, -90);
            else if (position.y == manager.MazeBuilder.Maze.GetLength(1) - 1)
                transform.Rotate(Vector3.up, 180);
        }

        private Vector2Int FindPosition(MazeCell[,] maze)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == MazeCell.ExitDoor)
                        return new Vector2Int(i, j);
                }
            }

            return Vector2Int.zero;
        }
    }
}