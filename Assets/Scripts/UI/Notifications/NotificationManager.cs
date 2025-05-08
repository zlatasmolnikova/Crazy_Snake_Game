using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textOutput;
    [SerializeField] private GameObject notificationUI;
    private readonly WaitForSeconds popupDuration = new (1);
    private bool isActive;

    public void Notify(string text)
    {
        if (isActive)
            return;
        
        textOutput.SetText(text);
        notificationUI.SetActive(true);
        isActive = true;
        StartCoroutine(PopupNotification());
    }

    private IEnumerator PopupNotification()
    {
        yield return popupDuration;
        notificationUI.SetActive(false);
        isActive = false;
    }
}