using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponEffects
{
    void PlayShootAnimation();
    void PlaySoundEffect();
    void ApplyRecoil();
    void CameraShake();
}
