using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageCounter : MonoBehaviour
{
    public float lifetime = 1.5f; // How long the counter will stay
    public float floatSpeed = 2f; // How fast it moves upward
    public TMP_Text damageText; // Reference to the TMP_Text component
    private Canvas canvas; // Reference to the Canvas component

    private Color textColor;

    [Header("Offset Settings")]
    public float maxXOffset = 0.5f; // Maximum X offset
    public float maxYOffset = 0.5f; // Maximum Y offset

    void Awake()
    {
        // Get the Canvas component
        canvas = GetComponentInChildren<Canvas>();

        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            // Find the camera tagged as "GameplayCamera"
            Camera gameplayCamera = GameObject.FindGameObjectWithTag("GameplayCamera")?.GetComponent<Camera>();

            if (gameplayCamera != null)
            {
                // Assign the gameplay camera to the canvas
                canvas.worldCamera = gameplayCamera;
            }
            else
            {
                Debug.LogError("No camera with tag 'GameplayCamera' found. Assign a gameplay camera to render the DamageCounter.");
            }
        }
    }

    void Start()
    {
        // Add a random offset to the position
        Vector3 randomOffset = new Vector3(
            Random.Range(-maxXOffset, maxXOffset), 
            Random.Range(-maxYOffset, maxYOffset), 
            0
        );
        transform.position += randomOffset;

        // Initialize the text color
        if (damageText != null)
        {
            textColor = damageText.color;
        }
        else
        {
            Debug.LogError("DamageText is not assigned in DamageCounter prefab.");
        }

        // Destroy the game object after its lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        if (damageText != null)
        {
            textColor.a = Mathf.Lerp(textColor.a, 0, Time.deltaTime / lifetime);
            damageText.color = textColor;
        }
    }

    public void SetDamage(int damage)
    {
        // Set the damage text
        if (damageText != null)
        {
            damageText.text = damage.ToString();
        }
    }
}