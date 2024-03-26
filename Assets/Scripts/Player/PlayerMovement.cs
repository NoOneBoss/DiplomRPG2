using System;
using Player;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public static Controls _controls;
    public static bool canMove = true;
    
    public float acceleration;
    public float deceleration;
    private float currentSpeed = 0f;
    public Animator _animator;
    
    private float currentStaminaRegenDelay = 0f;

    void Start()
    {
        _controls = new Controls();
        _controls.Enable();
    }

    void Update()
    {
        if(!canMove) return;
        if(!IsOwner) return;
        if(NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<PlayerDataHandler>() == null) return;
        var playerData = NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<PlayerDataHandler>().playerData;
        
        Vector2 playerMove = _controls.Player.Move.ReadValue<Vector2>();

        if (_controls.Player.Run.IsPressed() && playerData.currentStamina > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 1f, Time.deltaTime * acceleration);
            playerData.currentStamina -= Time.deltaTime * 10f;
            currentStaminaRegenDelay = 0f;
        }
        else if (playerMove.magnitude > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.5f, Time.deltaTime * acceleration);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.deltaTime * deceleration);
        }

        Vector3 targetPosition = transform.position + new Vector3(playerMove.x, playerMove.y, 0) * (currentSpeed/10 * playerData.defaultMovementSpeed);
        transform.position = targetPosition;
        
        _animator.SetFloat("Speed", currentSpeed);
        _animator.SetFloat("Horizontal", playerMove.x);
        _animator.SetFloat("Vertical", playerMove.y);
        
        if (_controls.Player.Run.IsPressed() || !(playerData.currentStamina < playerData.maxStamina)) return;
        currentStaminaRegenDelay += Time.deltaTime;
        
        
        if (!(currentStaminaRegenDelay >= 1f / playerData.staminaRegenRate)) return;
        playerData.currentStamina = Mathf.Min(playerData.currentStamina + 1f, playerData.maxStamina);
        currentStaminaRegenDelay = 0f;
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
