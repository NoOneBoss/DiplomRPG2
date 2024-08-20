using System;
using Configs;
using Inventory.Types;
using Items.Specification;
using Other;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerRegeneration : NetworkBehaviour
    {
        private PlayerData _playerData;
        private void Start()
        {
            if(!IsOwner || IsServer) return;
            StartCoroutine(Schedulers.ExecuteAfterTime(1f, () =>
            {
                _playerData = PlayerManager.Singleton.getLocalPlayerData();
            }));
            StartCoroutine(Schedulers.RepeatWithInterval(0.2f, () =>
                {
                    if(_playerData == null) return;
                    
                    var healthAttribute = _playerData.healthAttribute.Value;
                    if (healthAttribute.lastModifiedTime + 1000*3 <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    {
                        _playerData.CurrentHealth = Math.Clamp(healthAttribute.currentHealth + healthAttribute.healthRegen/5, 0, healthAttribute.maxHealth);
                    }
                    
                    var shieldAttribute = _playerData.shieldAttribute.Value;
                    if (shieldAttribute.lastModifiedTime + 1000*3 <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    {
                        _playerData.CurrentShield = Math.Clamp(shieldAttribute.currentShield + shieldAttribute.shieldRegen/5, 0, shieldAttribute.maxShield);
                    }
                    
                    var manaAttribute = _playerData.manaAttribute.Value;
                    if (manaAttribute.lastModifiedTime + 1000*3 <= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    {
                        _playerData.CurrentMana = Math.Clamp(manaAttribute.currentMana + manaAttribute.manaRegen/5, 0, manaAttribute.maxMana);
                    }
                })
            );
        }
    }
}