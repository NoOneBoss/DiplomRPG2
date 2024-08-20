using System.Collections;
using System.Collections.Generic;
using Player;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

namespace Netcode.LogModule
{
    public class LogService : NetworkBehaviour
    {
        public static LogService Singlenton;
        
        [SerializeField] private LogList logsList;
        public ServerConfig serverConfig;
        private const int MaxLogs = 100;

        public void Awake()
        {
            if (NetworkManager.Singleton.IsServer) return;

            Debug.Log("Log system initialized!");
            Singlenton = this;
            logsList = new LogList();
        }

        public void AddLog(string timestamp, string uuid, string action, string context)
        {
            if (logsList.logs.Count >= MaxLogs)
            {
                StartCoroutine(PostLogs(logsList));
                logsList.logs.Clear();
            }

            logsList.logs.Add(new Log(timestamp, uuid, action, context));
        }

        private IEnumerator PostLogs(LogList logsToSend)
        {
            string json = JsonUtility.ToJson(logsToSend);

            UnityWebRequest www = new UnityWebRequest($"http://{serverConfig.address}:{serverConfig.port}/logs", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + PlayerManager.Singleton.getLocalPlayerData().authToken);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending logs: {www.error}");
            }
            else
            {
                Debug.Log("Logs sent successfully.");
            }
        }
    }
}