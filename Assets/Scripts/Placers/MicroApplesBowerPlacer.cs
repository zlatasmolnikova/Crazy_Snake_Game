using UnityEngine;

public class MicroApplesBowerPlacer : MonoBehaviour, IPlacer
{
    public void Place(PlacementManager manager)
    {
        for (int i = 0; i < 3; i++)
        {
            var position = manager.GetPosition(2);
            Instantiate(gameObject, manager.GetTransformPosition(position), Quaternion.identity);
            manager.AddOnPlacementMap(position, 10);   
        }
    }
}