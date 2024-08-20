using System;
using Configs;
using Inventory.Types;
using Items.Specification;
using Netcode.LogModule;
using Other;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class CombatSystem : NetworkBehaviour
    {
        private Controls _controls;
        private PlayerData _playerData;
        private PlayerMovement _playerMovement;
        
        public Animator playerAnimator;
        public Animator mainHandAnimator;
        public Animator offHandAnimator;

        public static Action<Weapon> PullWeaponEvent;
        public static Action<Weapon> UnpullWeaponEvent;

        public static Action ParryEvent;
        public static Action DodgeEvent;
        public static Action HurtEvent;
        public static Action DeadEvent;

        private void Start()
        {
            if(!IsOwner) return;
            _controls = PlayerMovement._controls;
            _playerData = GetComponent<PlayerData>();
            _playerMovement = GetComponent<PlayerMovement>();
            
            _controls.Player.Parry.performed += _ =>
            {
                Parry();
            };
            
            _controls.Player.Dodge.performed += _ =>
            {
                Dodge();
            };
        }

        private bool isDodging = false;
        
        public void DamagePlayer(float damage)
        {
            playerAnimator.Play("Hurt");
            _playerData.CurrentHealth = Math.Clamp(_playerData.CurrentHealth - damage * (float)(1 - 0.06 * _playerData.Armor / (1 + 0.06 * _playerData.Armor)), 0, _playerData.CurrentHealth);
            if(_playerData.CurrentHealth <= 0) Death();
        }

        public void Death()
        {
            playerAnimator.Play("Death");
        }
        
        public void Parry()
        {
            if(!IsOwner) return;
            if(!PlayerMovement.canMove && _playerData.CurrentStamina >= 5f) return;
            
            _playerData.CurrentStamina -= 5f;
            
            PlayerMovement.LockMovement();
            playerAnimator.Play("Parry");
           
            
            StartCoroutine(Schedulers.ExecuteAfterTime(0.5f, PlayerMovement.UnlockMovement));
            
        
            ParryEvent?.Invoke();
            LogService.Singlenton.AddLog(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), _playerData.uuid, "parry", $"{_playerData.CurrentStamina}&{_playerData.CurrentHealth}&{_playerData.CurrentShield}&{_playerData.CurrentMana}&{getNearbyPlayers()}&{getNearbyMonsters()}");
        }

        public void Dodge()
        {
            if(!IsOwner) return;
            if(!PlayerMovement.canMove) return;
            if (!isDodging && _playerData.CurrentStamina >= 25f)
            {
                _playerData.CurrentStamina -= 25f;
        
                Vector3 dodgeDirection = _playerMovement.lastMoveDirection.normalized;
                
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = dodgeDirection * 15;
        
                playerAnimator.Play("Dodge");
                DodgeEvent?.Invoke();

                isDodging = true;
                StartCoroutine(Schedulers.ExecuteAfterTime(0.1f, () =>
                {
                    rb.velocity = Vector2.zero;
                }));
                StartCoroutine(Schedulers.ExecuteAfterTime(3f, () =>
                {
                    isDodging = false;
                }));
                
                LogService.Singlenton.AddLog(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), _playerData.uuid, "dodge", $"{_playerData.CurrentStamina}&{_playerData.CurrentHealth}&{_playerData.CurrentShield}&{_playerData.CurrentMana}&{getNearbyPlayers()}&{getNearbyMonsters()}");
            }
        }

        public void PullWeapon(Weapon weapon)
        {
            playerAnimator.Play("Pull");
            PullWeaponEvent.Invoke(weapon);
        }

        public void UnpullWeapon(Weapon weapon)
        {
            playerAnimator.Play("Unpull");
            UnpullWeaponEvent.Invoke(weapon);
        }

        public int getNearbyPlayers()
        {
            int nearbyPlayers = 0;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 25f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    nearbyPlayers++;
                }
            }

            return nearbyPlayers;
        }
        
        public int getNearbyMonsters()
        {
            int nearbyMonsters = 0;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 25f);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster"))
                {
                    nearbyMonsters++;
                }
            }

            return nearbyMonsters;
        }
    }
}