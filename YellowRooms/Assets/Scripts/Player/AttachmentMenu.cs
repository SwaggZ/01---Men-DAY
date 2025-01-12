using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttachmentMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject attachmentMenu; // Reference to the attachment menu UI
    public Transform scopeSection; // UI section for scopes
    public Transform laserSection; // UI section for lasers
    public Transform suppressorSection; // UI section for suppressors
    public GameObject attachmentButtonPrefab; // Prefab for the attachment button

    [Header("Gameplay References")]
    public GameObject weaponHolder; // Reference to the WeaponHolder GameObject
    public GameObject CameraHolder;

    private AttachmentsSystem currentAttachments; // Reference to the active weapon's AttachmentsSystem
    private GameObject lastActiveWeapon; // Keeps track of the last active weapon
    private bool isMenuOpen = false; // Tracks if the menu is open

    [Header("UI References")]
    public TMP_Text weaponNameText; // Reference to the TMP_Text for the weapon name

    void Start()
    {
        // Initialize the menu as closed
        attachmentMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle the menu when "I" is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleAttachmentMenu();
        }

        // Detect weapon change
        GameObject activeWeapon = GetActiveWeapon();
        if (activeWeapon != lastActiveWeapon)
        {
            HandleShootingScripts(activeWeapon, lastActiveWeapon); // Update shooting scripts
            lastActiveWeapon = activeWeapon;

            if (isMenuOpen)
            {
                UpdateAttachmentMenu(); // Refresh the menu if open
            }
        }
    }

    void ToggleAttachmentMenu()
    {
        isMenuOpen = !attachmentMenu.activeSelf;

        if (isMenuOpen)
        {
            // Opening the menu
            attachmentMenu.SetActive(true);
            UpdateAttachmentMenu();

            // Release the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Disable gameplay
            DisableGameplay();

            // Disable the active weapon's gun script
            if (lastActiveWeapon != null)
            {
                var gunScript = lastActiveWeapon.GetComponent<Gun>();
                if (gunScript != null)
                {
                    gunScript.enabled = false;
                }
            }
        }
        else
        {
            // Closing the menu
            attachmentMenu.SetActive(false);

            // Lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Enable gameplay
            EnableGameplay();

            // Enable the active weapon's gun script
            if (lastActiveWeapon != null)
            {
                var gunScript = lastActiveWeapon.GetComponent<Gun>();
                if (gunScript != null)
                {
                    gunScript.enabled = true;
                }
            }
        }
    }

    void UpdateAttachmentMenu()
    {
        // Clear existing buttons in the UI sections
        ClearSection(scopeSection);
        ClearSection(laserSection);
        ClearSection(suppressorSection);

        // Get the active weapon
        GameObject activeWeapon = GetActiveWeapon();
        if (activeWeapon == null)
        {
            Debug.LogWarning("No active weapon found in WeaponHolder!");
            weaponNameText.text = "No Weapon Selected";
            return;
        }

        // Update the weapon name text
        weaponNameText.text = activeWeapon.name;

        // Check if the active weapon has an AttachmentsSystem
        currentAttachments = activeWeapon.GetComponent<AttachmentsSystem>();
        if (currentAttachments == null)
        {
            Debug.LogWarning("Active weapon does not have an AttachmentsSystem!");
            return;
        }

        // Populate the attachment menu dynamically
        foreach (var scope in currentAttachments.Scopes)
        {
            CreateAttachmentButton(scope, scopeSection, currentAttachments.Scopes);
        }

        foreach (var laser in currentAttachments.Lasers)
        {
            CreateAttachmentButton(laser, laserSection, currentAttachments.Lasers);
        }

        foreach (var suppressor in currentAttachments.Supressors)
        {
            CreateAttachmentButton(suppressor, suppressorSection, currentAttachments.Supressors);
        }
    }

    GameObject GetActiveWeapon()
    {
        foreach (Transform weapon in weaponHolder.transform)
        {
            if (weapon.gameObject.activeSelf)
            {
                return weapon.gameObject;
            }
        }
        return null;
    }

    void CreateAttachmentButton(GameObject attachment, Transform section, GameObject[] allAttachments)
    {
        // Instantiate a button in the specified section
        GameObject button = Instantiate(attachmentButtonPrefab, section);

        // Set the button's text to the attachment's name
        TMPro.TMP_Text buttonText = button.GetComponentInChildren<TMPro.TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = attachment.name;
        }
        else
        {
            Debug.LogError("TMP_Text component not found on AttachmentButton prefab!");
        }

        // Add a listener to activate the attachment when clicked
        button.GetComponent<Button>().onClick.AddListener(() => ActivateAttachment(attachment, allAttachments));
    }

    void ActivateAttachment(GameObject selectedAttachment, GameObject[] allAttachments)
    {
        // Deactivate all attachments in the same section
        foreach (var attachment in allAttachments)
        {
            attachment.SetActive(false);
        }

        // Activate the selected attachment
        selectedAttachment.SetActive(true);

        // Update the gun's aiming position if the attachment is a scope
        if (currentAttachments != null && currentAttachments.Scopes != null)
        {
            int scopeIndex = System.Array.IndexOf(currentAttachments.Scopes, selectedAttachment);
            if (scopeIndex >= 0 && scopeIndex < currentAttachments.ScopeAimingPositions.Length)
            {
                Gun gun = GetActiveWeapon()?.GetComponent<Gun>();
                if (gun != null)
                {
                    gun.currentAimingPosition = currentAttachments.ScopeAimingPositions[scopeIndex];
                    gun.currentFOV = selectedAttachment.GetComponent<Scope>().fieldOfView; // Assuming each scope has a custom FOV
                }
            }
        }

        // Update the weapon preview
        GameObject activeWeapon = GetActiveWeapon();
        UpdateWeaponPreview(activeWeapon);
    }

    void ClearSection(Transform section)
    {
        // Destroy all children of the specified section
        foreach (Transform child in section)
        {
            Destroy(child.gameObject);
        }
    }

    void EnableGameplay()
    {
        // Enable shooting for the active weapon
        if (lastActiveWeapon != null)
        {
            var gunScript = lastActiveWeapon.GetComponent<Gun>();
            if (gunScript != null) gunScript.enabled = true;
        }

        // Enable camera movement
        var mouseLookScript = CameraHolder.GetComponentInChildren<MouseLook>();
        if (mouseLookScript != null) mouseLookScript.enabled = true;
    }

    void DisableGameplay()
    {
        // Disable shooting for the active weapon
        if (lastActiveWeapon != null)
        {
            var gunScript = lastActiveWeapon.GetComponent<Gun>();
            if (gunScript != null) gunScript.enabled = false;
        }

        // Disable camera movement
        var mouseLookScript = CameraHolder.GetComponentInChildren<MouseLook>();
        if (mouseLookScript != null) mouseLookScript.enabled = false;
    }

    void HandleShootingScripts(GameObject newWeapon, GameObject previousWeapon)
    {
        if (previousWeapon != null)
        {
            // Always disable the shooting script of the previous weapon
            var previousGunScript = previousWeapon.GetComponent<Gun>();
            if (previousGunScript != null)
            {
                previousGunScript.enabled = false;
            }
        }

        if (newWeapon != null)
        {
            // Enable the shooting script of the new weapon only if the menu is closed
            var newGunScript = newWeapon.GetComponent<Gun>();
            if (newGunScript != null)
            {
                newGunScript.enabled = !isMenuOpen;
            }
        }
    }

    void UpdateWeaponPreview(GameObject weapon)
    {
        if (weapon == null) return;

        // Example of adjusting the weapon layer for preview
        weapon.layer = LayerMask.NameToLayer("WeaponPreview");
        foreach (Transform child in weapon.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("WeaponPreview");
        }
    }
}