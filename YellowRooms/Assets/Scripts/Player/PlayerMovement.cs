using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;
    public Transform cylinder;
    public GameObject player;
    public GameObject playerFace;

    float speed = 18f;
    float runSpeed = 26f;
    float strafeSpeed = 15f;
    float crouchSpeedFactor = 8f;
    float crouchYScale = 1.8f;
    float startYScale = 3.8f;
    float cylinderScale = 1.9f;
    float cylinderCrouchScale = 0.9f;
    float gravity = -19.62f;
    float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform topCheck;
    public LayerMask topMask;

    [Header("Look Handler")]
    public Transform CameraHolder;
    public GameObject playerCamera;
    public GameObject weaponCamera;

    Vector3 velocity;
    public bool isGrounded;
    public bool isTop;
    [SerializeField] bool isCrouching;

    bool isAimingG19;
    bool isAimingP90;
    bool isAimingM3;
    bool isAimingHR;
    bool isAimingAK47;
    bool isAimingM249;
    
    [Header("Guns")]
    public GameObject G19;
    public GameObject P90;
    public GameObject M3;
    public GameObject HR;
    public GameObject AK47;
    public GameObject M249;
    float aimSpeedFactor = 5f;

    void Start() {
        if(!isLocalPlayer)
        {
            playerCamera.gameObject.GetComponent<Camera>().enabled = false;
            playerCamera.gameObject.GetComponent<AudioListener>().enabled = false;
            weaponCamera.gameObject.GetComponent<Camera>().enabled = false;
            G19.layer = LayerMask.NameToLayer("Enemy");
            P90.layer = LayerMask.NameToLayer("Enemy");
            M3.layer = LayerMask.NameToLayer("Enemy");
            HR.layer = LayerMask.NameToLayer("Enemy");
            AK47.layer = LayerMask.NameToLayer("Enemy");
            M249.layer = LayerMask.NameToLayer("Enemy");
            foreach(Transform child in G19.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            foreach(Transform child in P90.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            foreach(Transform child in M3.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            foreach(Transform child in HR.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            foreach(Transform child in AK47.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            foreach(Transform child in M249.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
        startYScale = transform.localScale.y;
        controller.height = 3.8f;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        player.SetActive(false);
        playerFace.SetActive(false);
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        isAimingG19 = G19.GetComponent<Gun>().isAiming;
        isAimingP90 = P90.GetComponent<Gun>().isAiming;
        isAimingM3 = M3.GetComponent<Gun>().isAiming;
        isAimingHR = HR.GetComponent<Gun>().isAiming;
        isAimingAK47 = AK47.GetComponent<Gun>().isAiming;
        isAimingM249 = M249.GetComponent<Gun>().isAiming;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isTop = Physics.CheckSphere(topCheck.position, groundDistance, topMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * z;
        Vector3 strafeMove = transform.right * x;
        if(isAimingG19 || isAimingP90 || isAimingM3 || isAimingHR || isAimingAK47 || isAimingM249)
        {
            aimSpeedFactor = 5f;
        }
        else
        {
            aimSpeedFactor = 0f;
        }
        
        if (Input.GetButton("Crouch"))
        {
            crouchSpeedFactor = 8f;
            controller.height = crouchYScale;
            cylinder.localScale = new Vector3(1.2f, cylinderCrouchScale, 1.2f);
            controller.Move(move * (speed - crouchSpeedFactor - aimSpeedFactor) * Time.deltaTime);
            isCrouching = true;
        }
        else
        {
            crouchSpeedFactor = 0f;
            if(isTop)
            {
                crouchSpeedFactor = 8f;
            }
            else
            {
                crouchSpeedFactor = 0f;
                controller.height = 3.8f;
                cylinder.localScale = new Vector3(1.2f, cylinderScale, 1.2f);
            }
            isCrouching = false;
            if(Input.GetButton("Run") && isGrounded && !isTop)
            {
                controller.Move(move * (runSpeed - crouchSpeedFactor - aimSpeedFactor) * Time.deltaTime);
            }
            else
            {
                controller.Move(move * (speed - crouchSpeedFactor - aimSpeedFactor) * Time.deltaTime);
            }
        }
        
        controller.Move(strafeMove * (strafeSpeed - crouchSpeedFactor - aimSpeedFactor) * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
