using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SmoothCameraTargetting : MonoBehaviour
{
    public Transform target;
    public float smoothingSpeed = 5f;
    public float cameraSpeed = 2f;
    private Vector3 offset;

    public void StartTargetting()
    {
        if (GetComponent<NetworkObject>() && !GetComponent<NetworkObject>().IsOwner)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectsWithTag("Player")
            .First(player => player.GetComponent<NetworkObject>().IsOwner);
        target = player.transform;
        offset = transform.position - target.position;
    }
    
    void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothingSpeed * Time.deltaTime);
        
        float distanceFromPlayer = Vector3.Distance(target.position, transform.position);
        if (distanceFromPlayer > 5f)
        {
            float step = cameraSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}