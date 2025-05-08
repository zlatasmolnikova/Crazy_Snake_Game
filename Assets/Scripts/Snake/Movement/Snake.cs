using UnityEngine;

public class Snake : MonoBehaviour, IPlacer
{
    public void Place(PlacementManager manager)
    {
        transform.position = manager.GetTransformPosition(manager.GetPosition(0));
    }
}