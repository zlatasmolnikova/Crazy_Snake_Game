using System;
using Environment;
using UnityEngine;

namespace Items
{
    public class DoorKey : MonoBehaviour, IPlacer
    {
        [SerializeField] private Door door;

        private void Start()
        {
            door.enabled = false;
        }

        public void OnPickUp()
        {
            door.enabled = true;
            Destroy(gameObject);
        }
        
        public void Place(PlacementManager manager)
        {
            var position = manager.GetTransformPosition(manager.GetPosition());
            position.y = 0;
            transform.position = position;
        }
    }
}