
using UnityEngine;

public class WeaponPlacer : MonoBehaviour, IPlacer
{
    [SerializeField] private int weaponCount;
    private WeaponFactory weaponFactory;
    public void Place(PlacementManager manager)
    {
        if (weaponFactory == null)
            weaponFactory = GameObject.FindGameObjectWithTag("WeaponFactory").GetComponent<WeaponFactory>();
        for (int i = 0; i < weaponCount; i++)
        {
            PlaceWeapon(manager);
        }
    }

    private void PlaceWeapon(PlacementManager manager)
    {
        var position = manager.GetPosition(0);
        var transformPos = manager.GetTransformPosition(position) + manager.GetShiftInsideCell();
        PlaceCube(transformPos, manager);
        transformPos.y = 1;
        weaponFactory.CreateRandomWeaponLevelOne(transformPos);
        manager.AddOnPlacementMap(position, 5);
    }

    private void PlaceCube(Vector3 position, PlacementManager manager)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.parent = manager.MazeBuilder.Environment.transform;
    }
}