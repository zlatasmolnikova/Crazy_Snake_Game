using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParamsUpdater : MonoBehaviour
{
    public GameObject Player;

    private const int MouseSensetivityCoef = 28;
    
    private QuakeCPMPlayerMovement controller;
    void Start()
    {
        controller = Player.GetComponent<QuakeCPMPlayerMovement>();

        SetSensetivity();
    }
    
    void Update()
    {
        SetSensetivity();
    }

    void SetSensetivity()
    {
        if (PlayerPrefs.HasKey("Sensetivity"))
        {
            controller.xMouseSensitivity = PlayerPrefs.GetFloat("Sensetivity") * MouseSensetivityCoef;
            controller.yMouseSensitivity = controller.xMouseSensitivity;
        }
    }
}
