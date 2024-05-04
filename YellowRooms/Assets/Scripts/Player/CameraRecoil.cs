using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraRecoil : NetworkBehaviour
{
    [Header("Rotations")]
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [Header("Hipfire Recoil")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [Header("Settings")]
    [SerializeField] private float snapppiness;
    [SerializeField] private float returnSpeed;

    [Header("Camera GameObject")]
    public GameObject cam;

    [Header("Aimming Checker")]
    public GameObject Gun;
    bool isAiming;
    int aimFactor;

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        
        isAiming = Gun.GetComponent<Gun>().isAiming;
        if(isAiming)
        {
            aimFactor = 3;
        }
        else
        {
            aimFactor = 1;
        }
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapppiness * Time.fixedDeltaTime);
        cam.transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX / aimFactor, Random.Range(-recoilY / aimFactor, recoilY / aimFactor), Random.Range(-recoilZ / aimFactor, recoilZ / aimFactor));
    }
}
