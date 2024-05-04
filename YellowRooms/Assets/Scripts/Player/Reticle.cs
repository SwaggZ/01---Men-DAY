using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Reticle : NetworkBehaviour
{
    private RectTransform reticle;

    [Range(50f, 250f)]
    public float size;

    private void Start() {
        reticle = GetComponent<RectTransform>();
    }

    private void Update() {
        if(!isLocalPlayer)
        {
            return;
        }
        
        reticle.sizeDelta = new Vector2(size, size);
    }
}
