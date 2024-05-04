using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;
using Mirror;

public class ShakeToWeapon : NetworkBehaviour
{
    public Shaker MyShaker;

    [Header("Guns Game Objects")]
    public GameObject Glock19;
    public GameObject P90;
    public GameObject M3;
    public GameObject HR;
    public GameObject AK47;
    public GameObject M249;

    [Header("Shake Presets")]
    public ShakePreset Glock19Preset;
    public ShakePreset P90Preset;
    public ShakePreset M3Preset;
    public ShakePreset HRPreset;
    public ShakePreset AK47Preset;
    public ShakePreset M249Preset;

    [Header("ADS Shake Presets")]
    public ShakePreset ADSGlock19Preset;
    public ShakePreset ADSP90Preset;
    public ShakePreset ADSM3Preset;
    public ShakePreset ADSHRPreset;
    public ShakePreset ADSAK47Preset;
    public ShakePreset ADSM249Preset;

    [Header("Weapon Shoting")]
    [SerializeField] bool isGlock19Shooting;
    [SerializeField] bool isP90Shooting;
    [SerializeField] bool isM3Shooting;
    [SerializeField] bool isHRShooting;
    [SerializeField] bool isAK47Shooting;
    [SerializeField] bool isM249Shooting;

    [Header("Weapon Aiming")]
    [SerializeField] bool isG19Aiming;
    [SerializeField] bool isP90Aiming;
    [SerializeField] bool isM3Aiming;
    [SerializeField] bool isHRAiming;
    [SerializeField] bool isAK47Aiming;
    [SerializeField] bool isM249Aiming;

    private void Update() {
        if(!isLocalPlayer)
        {
            return;
        }
        
        isGlock19Shooting = Glock19.GetComponent<Gun>().shootingRN;
        isP90Shooting = P90.GetComponent<Gun>().shootingRN;
        isM3Shooting = M3.GetComponent<Gun>().shootingRN;
        isHRShooting = HR.GetComponent<Gun>().shootingRN;
        isAK47Shooting = AK47.GetComponent<Gun>().shootingRN;
        isM249Shooting = M249.GetComponent<Gun>().shootingRN;

        isG19Aiming = Glock19.GetComponent<Gun>().isAiming;
        isP90Aiming = P90.GetComponent<Gun>().isAiming;
        isM3Aiming = M3.GetComponent<Gun>().isAiming;
        isHRAiming = HR.GetComponent<Gun>().isAiming;
        isAK47Aiming = AK47.GetComponent<Gun>().isAiming;
        isM249Aiming = M249.GetComponent<Gun>().isAiming;

        if(isG19Aiming || isP90Aiming || isM3Aiming || isHRAiming || isAK47Aiming || isM249Aiming)
        {
            if(isGlock19Shooting)
            {
                MyShaker.Shake(ADSGlock19Preset);
            }

            if(isP90Shooting)
            {
                MyShaker.Shake(ADSP90Preset);
            }

            if(isM3Shooting)
            {
                MyShaker.Shake(ADSM3Preset);
            }

            if(isHRShooting)
            {
                MyShaker.Shake(ADSHRPreset);
            }

            if(isAK47Shooting)
            {
                MyShaker.Shake(ADSAK47Preset);
            }

            if(isM249Shooting)
            {
                MyShaker.Shake(ADSM249Preset);
            }
        }
        else
        {
            if(isGlock19Shooting)
            {
                MyShaker.Shake(Glock19Preset);
            }

            if(isP90Shooting)
            {
                MyShaker.Shake(P90Preset);
            }

            if(isM3Shooting)
            {
                MyShaker.Shake(M3Preset);
            }

            if(isHRShooting)
            {
                MyShaker.Shake(HRPreset);
            }

            if(isAK47Shooting)
            {
                MyShaker.Shake(AK47Preset);
            }

            if(isM249Shooting)
            {
                MyShaker.Shake(M249Preset);
            }
        }
    }
}
