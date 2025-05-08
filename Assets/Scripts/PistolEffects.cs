using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PistolEffects : IWeaponEffects
{
    public Transform playerCamera; // Присвоить через инспектор

    public PistolEffects()
    {
        playerCamera = GameObject.Find("FirstPersonController")
            .transform.Find("Joint")
            .transform.Find("PlayerCamera");

        playerCamera.gameObject.AddComponent<WeaponRecoil>();
        playerCamera.gameObject.AddComponent<CameraShake>();
    }

    public void ApplyRecoil()
    {
        //var recoil = playerCamera.GetComponent<WeaponRecoil>();
        //if (recoil != null)
        //{
        //    recoil.ApplyRecoil();
        //}
    }

    public void CameraShake()
    {
        var shake = playerCamera.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.ShakeCamera();
        }
    }

    public void PlayShootAnimation()
    {
        throw new System.NotImplementedException();
    }

    public void PlaySoundEffect()
    {
        throw new System.NotImplementedException();
    }
}


public class WeaponRecoil : MonoBehaviour
{
    public float recoilAmount = 50.0f;
    public float maxRecoil = 15.0f;
    public float recoilRecoverySpeed = 10f;
    private Quaternion originalRotation;
    private Quaternion currentRecoilRotation;
    private float currentRecoil = 0f;

    void Start()
    {

    }

    void Update()
    {
        originalRotation = transform.localRotation;
        currentRecoilRotation = originalRotation;

        if (Input.GetMouseButtonDown(0))
        {
            ApplyRecoil();
        }

        RecoverRecoil();
    }

    void ApplyRecoil()
    {
        currentRecoil += recoilAmount;
        currentRecoil = Mathf.Min(currentRecoil, maxRecoil);

        // Случайное направление отдачи
        float recoilY = Random.Range(-currentRecoil, currentRecoil);
        float recoilX = Random.Range(-currentRecoil, currentRecoil);

        currentRecoilRotation = Quaternion.Euler(-recoilY, recoilX, 0) * transform.localRotation;
    }

    void RecoverRecoil()
    {
        if (Quaternion.Angle(transform.localRotation, originalRotation) > 0.1f) // Пороговое значение
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, recoilRecoverySpeed * Time.deltaTime);
        }
    }
}








public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.5f;
    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}


