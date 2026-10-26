using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private CharacterController controller;

    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();

        if (!IsOwner)
        {
            controller.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) 
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
