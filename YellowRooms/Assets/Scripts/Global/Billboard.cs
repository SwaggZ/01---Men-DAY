using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera gameplayCamera;

    void Start()
    {
        // Cache the gameplay camera
        gameplayCamera = GameObject.FindGameObjectWithTag("GameplayCamera")?.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (gameplayCamera == null)
        {
            // If the gameplay camera isn't cached, find it again
            gameplayCamera = GameObject.FindGameObjectWithTag("GameplayCamera")?.GetComponent<Camera>();
        }

        if (gameplayCamera != null)
        {
            // Rotate to face the camera
            transform.LookAt(gameplayCamera.transform);
            transform.Rotate(0, 180, 0); // Adjust if the text appears reversed
        }
    }
}