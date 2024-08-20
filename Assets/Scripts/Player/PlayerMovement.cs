using System;
using System.Collections.Generic;
using Configs;
using Netcode.LogModule;
using Other;
using Player;
using Unity.Netcode;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    public static Controls _controls;
    public static bool canMove = true;
    
    public float acceleration;
    public float deceleration;
    private float currentSpeed = 0f;
    public Animator _animator;
    
    private float currentStaminaRegenDelay = 0f;
    private PlayerData playerData;

    public Vector2 lastMoveDirection = Vector2.zero;
    public Vector2 lastPosition = Vector2.zero;

    private Predictor movementPredictor;
    private Predictor speedhackPredictor;
    private int sequenceLength = 250;
    private List<float[]> movementDataQueue;
    
    private TensorShape speedhackShape;
    private List<float[]> speedhackDataQueue;
    
    private float speedhackCheckInterval = 10f;
    private float speedhackCheckTimer = 0f;
    public static int speedhackWarnings = 0;
    public static float speedhackSpeed = 0f;

    void Start()
    {
        _controls = new Controls();
        _controls.Enable();

        movementPredictor = GetComponents<Predictor>()[0];
        speedhackPredictor = GetComponents<Predictor>()[1];
        
        movementDataQueue = new List<float[]>(sequenceLength);
        speedhackDataQueue = new List<float[]>(sequenceLength);

       StartCoroutine(Schedulers.RepeatWithInterval(3f, CheckSpeedhack));
    }


    private TensorShape movementShape;
    public Vector2 movementPrediction;
    public static float speedhackPrediction;
    
    void Update()
    {
        if(!canMove) return;
        if(!IsOwner) return;
        if(PlayerManager.Singleton == null) return;
        if(PlayerManager.Singleton.getLocalPlayerData() == null) return;
        if(playerData == null) playerData = PlayerManager.Singleton.getLocalPlayerData();
    
        Vector2 playerMove = _controls.Player.Move.ReadValue<Vector2>();
        var deltaTime = Time.deltaTime;
    
        if (_controls.Player.Run.IsPressed() && playerData.CurrentStamina > 0 && playerMove.magnitude > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 1f, deltaTime * acceleration);
            playerData.CurrentStamina -= deltaTime * playerData.StaminaPerMove;
            currentStaminaRegenDelay = 0f;
            LogService.Singlenton.AddLog(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), playerData.uuid, "run", $"{transform.position.x - lastPosition.x}&{transform.position.y - lastPosition.y}&{currentSpeed}&{playerMove.magnitude}&{transform.position.x}&{transform.position.y}&{playerData.CurrentStamina}&{lastMoveDirection.x}&{lastMoveDirection.y}");
            lastMoveDirection = playerMove.normalized;
            lastPosition = new Vector2(transform.position.x, transform.position.y);
            
            float[] movementData = 
            {
                 1, currentSpeed, playerMove.magnitude, playerData.CurrentStamina, lastMoveDirection.x, lastMoveDirection.y
            };
            EnqueueMovementData(movementData);
            
            float[] speedhackData = 
            {
                0, transform.position.x - lastPosition.x, transform.position.y - lastPosition.y, currentSpeed, playerMove.magnitude, playerData.CurrentStamina, lastMoveDirection.x, lastMoveDirection.y
            };
            EnqueueSpeedhackData(speedhackData);
            
        }
        else if (playerMove.magnitude > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0.5f, deltaTime * acceleration);
            LogService.Singlenton.AddLog(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), playerData.uuid, "walk", $"{transform.position.x - lastPosition.x}&{transform.position.y - lastPosition.y}&{currentSpeed}&{playerMove.magnitude}&{transform.position.x}&{transform.position.y}&{playerData.CurrentStamina}&{lastMoveDirection.x}&{lastMoveDirection.y}");
            lastMoveDirection = playerMove.normalized;
            
            float[] movementData = 
            {
                0, currentSpeed, playerMove.magnitude, playerData.CurrentStamina, lastMoveDirection.x, lastMoveDirection.y
            };
            EnqueueMovementData(movementData);
            
            float[] speedhackData = 
            {
                0, transform.position.x - lastPosition.x, transform.position.y - lastPosition.y, currentSpeed, playerMove.magnitude, playerData.CurrentStamina, lastMoveDirection.x, lastMoveDirection.y
            };
            lastPosition = new Vector2(transform.position.x, transform.position.y);
            EnqueueSpeedhackData(speedhackData);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deltaTime * deceleration);
        }
    
        Vector3 targetPosition = transform.position + new Vector3(playerMove.x, playerMove.y, 0) * (currentSpeed * playerData.MovementSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * currentSpeed);

        _animator.SetFloat("Speed", currentSpeed);
        _animator.SetFloat("Horizontal", lastMoveDirection.x);
        _animator.SetFloat("Vertical", lastMoveDirection.y);
        
        if ((_controls.Player.Run.IsPressed() || !(playerData.CurrentStamina < playerData.MaxStamina)) && playerMove.magnitude > 0) return;
        currentStaminaRegenDelay += deltaTime;
    
    
        if (!(currentStaminaRegenDelay >= 1f / playerData.StaminaRegenRate)) return;
        playerData.CurrentStamina = Mathf.Min(playerData.CurrentStamina + (Time.deltaTime * playerData.StaminaRegenRate), playerData.MaxStamina);
        currentStaminaRegenDelay = 0f;
    }
    
    private void EnqueueMovementData(float[] movementData)
    {
        movementDataQueue.Add(movementData);

        if (movementDataQueue.Count == sequenceLength)
        {
            float[] inputData = new float[sequenceLength * movementData.Length];
            int index = 0;
            foreach (var data in movementDataQueue)
            {
                Array.Copy(data, 0, inputData, index, data.Length);
                index += data.Length;
            }
            
            movementShape = new TensorShape(1, sequenceLength, movementData.Length);
            var tensorData = new TensorFloat(movementShape, inputData);
            var result = movementPredictor.Predict(tensorData);
            movementPrediction = new Vector2(result[0], result[1]);
            tensorData.Dispose();
            
            movementDataQueue.RemoveAt(0);
        }
    }
    
    private void EnqueueSpeedhackData(float[] movementData)
    {
        speedhackDataQueue.Add(movementData);

        if (speedhackDataQueue.Count == sequenceLength)
        {
            float[] inputData = new float[sequenceLength * movementData.Length];
            int index = 0;
            foreach (var data in speedhackDataQueue)
            {
                Array.Copy(data, 0, inputData, index, data.Length);
                index += data.Length;
            }
            
            speedhackShape = new TensorShape(1, sequenceLength, movementData.Length);
            var tensorData = new TensorFloat(speedhackShape, inputData);
            var result = speedhackPredictor.Predict(tensorData);
            //Debug.Log($"Anticheat: {result[0]} {Mathf.Sqrt(Mathf.Pow(movementData[1],2) + Mathf.Pow(movementData[2],2))}");
            speedhackSpeed = Mathf.Sqrt(Mathf.Pow(movementData[1],2) + Mathf.Pow(movementData[2],2));
            speedhackPrediction = result[0];
            tensorData.Dispose();
            
            speedhackDataQueue.RemoveAt(0);
        }
    }
    
    private void CheckSpeedhack()
    {
        if (Mathf.Abs(speedhackSpeed - speedhackPrediction) > 0.2f)
        {
            speedhackWarnings++;
            Debug.LogError($"Speedhack detected! Warnings: {speedhackWarnings}/5");
            if (speedhackWarnings >= 5)
            {
                NetworkManager.Shutdown();
                SceneManager.LoadScene("AuthScene");
            }
        }
    }

    public static void LockMovement()
    {
        canMove = false;
    }

    public static void UnlockMovement()
    {
        canMove = true;
    }
    
    void OnDrawGizmos()
    {
        if (movementPrediction != null)
        {
            Gizmos.color = Color.red;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + new Vector3(movementPrediction.x*5, movementPrediction.y*5, 0);
            Gizmos.DrawLine(startPosition, endPosition);
        }
    }
}
