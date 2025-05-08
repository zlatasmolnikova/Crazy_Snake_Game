using UnityEngine;

public class Pistol : Weapon
{
    public IWeaponEffects effects;
    public LineRenderer lineRenderer;
    private static Camera playerCamera;

    private void Start()
    {
        damage = 10;
        range = 100;

        playerCamera = GameObject.Find("FirstPersonController")
            .transform.Find("Joint")
            .transform.Find("PlayerCamera")
            .GetComponent<Camera>();

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Уменьшите ширину линии для большей реалистичности
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
    }

    public override void Fire()
    {
        RaycastHit hit;
        Vector3 shootDirection = playerCamera.transform.forward;
        // Добавьте небольшое смещение к startPosition, чтобы избежать перекрытия с камерой
        Vector3 startPosition = playerCamera.transform.position + shootDirection * 0.1f;

        //Debug.DrawLine(startPosition, startPosition + shootDirection * range, Color.red, 2.0f);


        if (Physics.Raycast(startPosition, shootDirection, out hit, range))
        {
            if (hit.collider.gameObject.CompareTag("Creature"))
            {
                hit.collider.gameObject.GetComponent<Creature>().TakeDamage(damage);
            }
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + shootDirection * range);
        }

        //effects.CameraShake();
        effects = new PistolEffects();
        effects.ApplyRecoil();

        StartCoroutine(ShowLaser());
    }

    private System.Collections.IEnumerator ShowLaser()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); // Уменьшенное время видимости для имитации вспышки
        lineRenderer.enabled = false;
    }
}
