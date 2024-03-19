using Player;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Controls _controls;

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
        if(!IsOwner) return;
        
        Vector2 playerMove = _controls.Player.Move.ReadValue<Vector2>();

        if (_controls.Player.Run.IsPressed() && PlayerManager.Singleton.playerData.currentStamina > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 1f, Time.deltaTime * acceleration);
            PlayerManager.Singleton.playerData.currentStamina -= Time.deltaTime * 10f;
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

        Vector3 targetPosition = transform.position + new Vector3(playerMove.x, playerMove.y, 0) * (currentSpeed/10 * PlayerManager.Singleton.playerData.defaultMovementSpeed);
        transform.position = targetPosition;
        
        _animator.SetFloat("Speed", currentSpeed);
        _animator.SetFloat("Horizontal", playerMove.x);
        _animator.SetFloat("Vertical", playerMove.y);
        
        if (!(_controls.Player.Run.IsPressed()) && PlayerManager.Singleton.playerData.currentStamina < PlayerManager.Singleton.playerData.maxStamina)
        {
            currentStaminaRegenDelay += Time.deltaTime;
            if (currentStaminaRegenDelay >= 1f / PlayerManager.Singleton.playerData.staminaRegenRate)
            {
                PlayerManager.Singleton.playerData.currentStamina = Mathf.Min(PlayerManager.Singleton.playerData.currentStamina + 1f, PlayerManager.Singleton.playerData.maxStamina);
                currentStaminaRegenDelay = 0f;
            }
        }
    }
}
