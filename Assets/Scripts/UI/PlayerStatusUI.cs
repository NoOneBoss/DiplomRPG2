using System;
using Configs;
using Other;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class PlayerStatusUI : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Schedulers.ExecuteAfterTime(1f, () =>
            {
                PlayerData playerData = gameObject.GetComponent<PlayerData>();
                if(playerData.ClientId == NetworkManager.Singleton.LocalClientId) return;
            
                gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                StartCoroutine(Schedulers.RepeatWithInterval(0.3f, () =>
                {
                    gameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{playerData.PlayerName}\n{(int)playerData.CurrentHealth} HP | {(int)playerData.CurrentShield} SHD";
                }));
            }));
        }
    }
}