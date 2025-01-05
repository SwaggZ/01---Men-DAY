using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Gun : NetworkBehaviour
{
    [Header("Gun Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool auto;
    int bulletsLeft, bulletsShot;
    public float spreadTemp;

    bool shooting, readyToShoot, reloading;

    [Header("Shooting Check")]
    public bool shootingRN = false;

    [Header("Cameras")]
    public Camera fpsCam;
    public Camera weaponCam;

    [Header("Points")]
    public Transform attackPoint;
    public Transform shootPoint;

    [Header("Shooting Physics")]
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    [Header("Prefabs")]
    public GameObject muzzleFlash, bulletHoleGraphics;

    [Header("Ammo Object")]
    public TextMeshProUGUI ammo;

    [Header("ADS Manager")]
    public Transform defaultPos;
    public Transform ADSPos;
    public float ADSLerp;
    public bool isAiming = false;
    public float adsFOV;

    [Header("Clipping Handler")]
    public GameObject clipProjector;
    public float checkDistance;
    public Vector3 newDirection;
    float lerpPos;
    RaycastHit hit;
    bool isClipping = false;
    public LayerMask notPlayers;

    [Header("Recoil Handler")]
    private CameraRecoil Recoil_Script;

    [Header("Sounds")]
    AudioSource shootingSound;
    public float pitchOffset;
    public AudioClip Clip;

    [Header("Aiming")]
    public Transform defaultAimingPosition; // Default aiming position
    public float defaultFOV = 60f; // Default field of view
    public float aimSpeed = 5f; // Speed of aiming transition

    public Transform currentAimingPosition; // Current aiming position
    public float currentFOV; // Current field of view

    private void Awake()
    {
        //if(!isLocalPlayer)
        //{
        // return;
        // }

        bulletsLeft = magazineSize;
        readyToShoot = true;
        spreadTemp = spread;

        Recoil_Script = GetComponent<CameraRecoil>();

        shootingSound = GetComponent<AudioSource>();
    }

    private void Update()
    {

        shootingRN = false;
        Aim();
        MyInput();

        ammo.text = (bulletsLeft / bulletsPerTap) + "\n" + (magazineSize / bulletsPerTap); //((magazineSize / bulletsPerTap) - (magazineSize - bulletsLeft)) + 

        if (Physics.Raycast(clipProjector.transform.position, clipProjector.transform.forward, out hit, checkDistance, notPlayers))
        {
            lerpPos = 1 - (hit.distance / checkDistance);
            isClipping = true;
        }
        else
        {
            lerpPos = 0;
            isClipping = false;
        }

        Mathf.Clamp01(lerpPos);

        transform.localRotation =
        Quaternion.Lerp(
            Quaternion.Euler(Vector3.zero),
            Quaternion.Euler(newDirection),
            lerpPos
        );
    }

    private void MyInput()
    {
        if (auto)
        {
            //if(GetComponent<NetworkView>().isMine)
            //{
            shooting = Input.GetKey(KeyCode.Mouse0);
            //}
        }
        else
        {
            //if(GetComponent<NetworkView>().isMine)
            //{
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
            //}
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!isClipping)
        {
            readyToShoot = false;
            shootingRN = true;

            shootingSound.pitch = Random.Range(1f - pitchOffset, 1f + pitchOffset);

            shootingSound.PlayOneShot(Clip);

            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            float z = Random.Range(-spread, spread);

            Vector3 direction = shootPoint.transform.forward + new Vector3(x, y, z);

            Debug.DrawRay(shootPoint.transform.position, direction * range, Color.yellow, 10f);
            if (Physics.Raycast(attackPoint.transform.position, direction, out rayHit, range))
            {
                if (rayHit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy Hit For: " + damage + " Damage");
                    rayHit.collider.GetComponent<PlayerStats>().TakeDamage(damage);
                }
                if (rayHit.collider.CompareTag("AIEnemy"))
                {
                    Debug.Log("Enemy Hit For: " + damage + " Damage");
                    rayHit.collider.GetComponent<Enemy>().TakeDamage(damage);
                }
            }

            Recoil_Script.RecoilFire();

            Instantiate(bulletHoleGraphics, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.LookRotation(transform.forward));

            bulletsLeft--;
            bulletsShot--;

            Invoke("ResetShot", timeBetweenShooting);

            if (bulletsShot > 0 && bulletsLeft > 0)
            {
                Invoke("Shoot", timeBetweenShots);
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    void Aim()
    {
        if (Input.GetMouseButton(1) && !reloading)
        {
            // Set aiming position and FOV
            transform.position = Vector3.Lerp(transform.position, currentAimingPosition.position, aimSpeed * Time.deltaTime);
            SetFieldOfView(Mathf.Lerp(fpsCam.fieldOfView, currentFOV, aimSpeed * Time.deltaTime));
            isAiming = true;
        }
        else
        {
            // Return to default position and FOV
            transform.position = Vector3.Lerp(transform.position, defaultAimingPosition.position, aimSpeed * Time.deltaTime);
            SetFieldOfView(Mathf.Lerp(fpsCam.fieldOfView, defaultFOV, aimSpeed * Time.deltaTime));
            isAiming = false;
        }
    }

    void SetFieldOfView(float fov)
    {
        fpsCam.fieldOfView = fov;
        weaponCam.fieldOfView = fov;
    }
}
