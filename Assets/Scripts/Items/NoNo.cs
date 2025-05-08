using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoNo : MonoBehaviour, IInventoryItem
{
    [SerializeField]
    private GameObject itemAvatarSkin;

    [SerializeField]
    private GameObject inHandAvatar;

    public Sprite GetItemAvatarSprite() => null;

    private IUser user;

    private Collider colliderForDetection;

    public void Awake()
    {
        colliderForDetection = GetComponent<Collider>();
    }
    
    public void DropOut()
    {
        transform.parent = null;
        itemAvatarSkin.SetActive(true);
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
        itemAvatarSkin.SetActive(false);
        transform.parent = user.HandTransform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }

    public bool TryUsePrimaryAction()
    {
        Debug.Log("NONO primary action");
        return true;
    }

    public bool TryUseSecondaryAction()
    {
        Debug.Log("Explode! just joking. Or am i?");
        return true;
    }
}
