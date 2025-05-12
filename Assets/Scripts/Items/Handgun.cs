using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Handgun : MonoBehaviour, IInventoryItem
{
    [SerializeField]
    private GameObject onSceneAvatar;
    [SerializeField]
    private Sprite itemAvatarSprite;
    
    public Sprite GetItemAvatarSprite() => itemAvatarSprite;

    [SerializeField]
    private GameObject inHandAvatar;

    private IUser user;

    private Collider colliderForDetection;

    private LineRenderer lineRenderer;

    private readonly float damage = 10;

    private readonly float range = 100;

    private Animator animator;

    private float cooldown = 1f / 3f;
    private float lastShotTime;

    private void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
    }

    private void Start()
    {
        //particleSystem = GetComponentInChildren<ParticleSystem>();      //genius
        // particleSystem.playbackSpeed = 3f;

        // Debug.Log(particleSystem);
        animator = inHandAvatar.GetComponent<Animator>();
        //soundController = GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
        // lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Уменьшите ширину линии для большей реалистичности
        //lineRenderer.positionCount = 2;
        //lineRenderer.startWidth = 0.05f;
        //lineRenderer.endWidth = 0.05f;
        //lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? whatever
        //lineRenderer.material.color = Color.red;
    }

    
    public void DropOut()
    {
        transform.parent = null;
        onSceneAvatar.SetActive(true);
        colliderForDetection.enabled = true;
    }

    public void OnSelect()
    {
        inHandAvatar.SetActive(true);
    }

    public void OnUnselect()
    {
        inHandAvatar.SetActive(false);
    }

    public void SetUser(IUser user)
    {
        colliderForDetection.enabled = false;
        this.user = user;
        onSceneAvatar.SetActive(false);
        transform.parent = user.CameraTransform;
        transform.localPosition = Vector3.zero + new Vector3(0.2f, -0.2f, 1f);
        transform.localEulerAngles = new Vector3(270, 270, 90);
    }

    public bool TryUsePrimaryAction()
    {
        if (Time.time - lastShotTime < cooldown)
            return false;

        if (!animator.GetBool("canShoot"))
            return false;

        //effectPosition = particleSystem.transform.position;
        //particleSystem.Play();
        FixPosition();

        lastShotTime = Time.time;
        animator.SetTrigger("IsShooting");

        RaycastHit hit;
        var shootDirection = user.CameraTransform.forward;
        var startPosition = user.CameraTransform.position + shootDirection * 0.1f;

        if (Physics.Raycast(startPosition, shootDirection, out hit, range))
        {
            Debug.Log(hit.rigidbody);

            if (hit.collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(new DamageInfo(damage));
            }

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + shootDirection * range);
        }

        //soundController.PlaySound("PistolShot", startPosition + user.CameraTransform.forward, 0.8f);

        //animator.SetBool("IsShooting", true);
        //animator.SetBool("IsShooting", false);
        //animator.SetTrigger("IsShooting");

        //StartShooting();

        StartCoroutine(ShowLaser());
        return true;
    }


    private IEnumerator FixPosition()
    {
        /*while (particleSystem.isPlaying)
        {
            transform.position = effectPosition; // Удерживаем позицию во время проигрывания частиц
            yield return null;
        }*/
        yield return null;
    }

    private void StartShooting()
    {
        animator.SetBool("IsShooting", true);
        //StopShooting()
        Invoke("StopShooting", 0.02f);
    }

    private void StopShooting()
    {
        animator.SetBool("IsShooting", false);
    }

    private IEnumerator ShowLaser()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); // Уменьшенное время видимости для имитации вспышки
        lineRenderer.enabled = false;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Shoot yourself if the leg. Now.");
        return true;
    }
}
