using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    // Controls how quickly the player moves.
    [SerializeField] private float moveSpeed;

    // Handles movement and collision detection for the player.
    private CharacterController controller;

    public override void OnNetworkSpawn()
    {
        // Get the CharacterController attached to this player.
        controller = GetComponent<CharacterController>();

        // Only the owning client should control this player.
        if (!IsOwner)
        {
            // Disable movement processing for players controlled by other clients.
            controller.enabled = false;
        }
    }

    private void Update()
    {
        // Ignore input on clients that do not own this player.
        if (!IsOwner)
            return;

        // Read horizontal and vertical movement input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Convert input into movement relative to the player's facing direction.
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Move the player while keeping movement consistent across frame rates.
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
