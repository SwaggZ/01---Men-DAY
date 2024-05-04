using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HeadBobController : NetworkBehaviour
{
    [Range(0f, 0.01f)]
    float Amount = 0.01f;
    [Range(1f, 30f)]
    float Frequency = 13f;
    [Range(10f, 100f)]
    float Smooth = 12f;

    float InputMagnitude;

    float BobFactor = 0f;
    float AmountFactor = 1f;

    public GameObject player;
    public bool isTop;
    public bool isGrounded;

    private void Update() {
        if(!isLocalPlayer)
        {
            return;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isTop = player.GetComponent<PlayerMovement>().isTop;
        isGrounded = player.GetComponent<PlayerMovement>().isGrounded;

        if(isGrounded)
        {
            if(!Input.GetMouseButton(1))
            {
                BobFactor = 0f;
            
                if(Input.GetButton("Crouch") && isTop && (z > 0f || x > 0f))
                {
                    BobFactor -= 0.7f;
                    AmountFactor = 2f;
                }
                else 
                {
                    AmountFactor = 1f;
                    if(Input.GetButton("Run"))
                    {
                        BobFactor += 1f;
                    }
                }
                CheckForHeadBobTrigger();
            }
            
            if(Input.GetMouseButton(1))
            {
                transform.localPosition = Vector3.zero;
            }
            Amount = 0.01f / AmountFactor;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    private void CheckForHeadBobTrigger() {
        InputMagnitude = new Vector3(Input.GetAxis("Horizontal") + BobFactor, 0, Input.GetAxis("Vertical") + BobFactor).magnitude;

        if(InputMagnitude > 0) {
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) * Amount * 1.4f, Smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * Frequency / 2f) * Amount * 1.6f, Smooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }
}
