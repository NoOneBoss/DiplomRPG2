using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Player.Player;
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

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothingSpeed * Time.deltaTime);

        // Check if the player goes beyond an invisible square boundary
        float distanceFromPlayer = Vector3.Distance(target.position, transform.position);
        if (distanceFromPlayer > 5f) // adjust the value as needed
        {
            float step = cameraSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }
}