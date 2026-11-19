using Unity.Netcode;
using UnityEngine;

// Handles first-person camera and mouse-look behavior for a networked player.
public class PlayerCamera : NetworkBehaviour
{
    // Controls how quickly the camera rotates in response to mouse movement.
    [SerializeField] private float mouseSensitivity;

    // Reference to the camera's transform used for vertical rotation.
    [SerializeField] private Transform cameraTransform;

    // Current vertical camera rotation.
    private float pitch = 0f;

    public override void OnNetworkSpawn()
    {
        // Disable the camera for remote players so each client only uses
        // the camera belonging to its own player object.
        if (!IsOwner)
        {
            cameraTransform.gameObject.SetActive(false);
        }
        else
        {
            // Lock and hide the cursor when the local player's camera is active.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // Only the owning client should control this camera.
        if (!IsOwner)
            return;

        // Pressing Escape releases the cursor so the player can interact
        // with other parts of the application.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // If the cursor is not locked, allow a left mouse click to capture it
        // again and prevent camera movement while it is released.
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            return;
        }

        // Read horizontal and vertical mouse movement and apply sensitivity.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Invert vertical mouse movement by subtracting from the current pitch.
        pitch -= mouseY;

        // Limit vertical rotation to prevent the camera from flipping over.
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Apply vertical rotation to the camera itself.
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Apply horizontal rotation to the player object.
        transform.Rotate(Vector3.up * mouseX);
    }
}
