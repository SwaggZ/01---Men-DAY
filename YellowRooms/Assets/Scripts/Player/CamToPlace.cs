using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CamToPlace : NetworkBehaviour
{
    public Transform standPlace;
    public Transform crouchPlace;
    public float crouchLerp;

    bool top;

    public GameObject player;

    void Start()
    {
            transform.position = Vector3.Lerp(transform.position, standPlace.position, 0.1f * Time.deltaTime);
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        
        top = player.GetComponent<PlayerMovement>().isTop;
        if (Input.GetButton("Crouch"))
        {
            transform.position = Vector3.Lerp(transform.position, crouchPlace.position, crouchLerp * Time.deltaTime);
        }
        else
        {
            if(!top)
            {
                transform.position = Vector3.Lerp(transform.position, standPlace.position, crouchLerp * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, crouchPlace.position, crouchLerp * Time.deltaTime);
            }
        }
    }
}
