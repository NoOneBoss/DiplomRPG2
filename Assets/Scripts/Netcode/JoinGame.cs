using Unity.Netcode;
using UnityEngine;

public class JoinGame : MonoBehaviour
{
    public bool isServer;
    void Start()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux || isServer)
        {
            NetworkManager.Singleton.StartServer();
        }
        else if (Application.isEditor)
        {
            NetworkManager.Singleton.StartHost();
        }
        else if(Application.isPlaying){
            NetworkManager.Singleton.StartClient();
        }
    }
    
}
