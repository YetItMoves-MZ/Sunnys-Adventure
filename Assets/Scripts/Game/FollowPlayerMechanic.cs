using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayerMechanic : MonoBehaviour
{
    public GameObject following;
    public bool followXOnly;
    PlayerMovement playerMovement;
    void Start()
    {
        if (following.TryGetComponent(out PlayerMovement playerMovementComponent))
            playerMovement = playerMovementComponent;
    }
    void Update()
    {
        if (playerMovement != null && playerMovement.playerDied)
            return;

        Vector3 followingPosition = following.transform.position;
        float x = followingPosition.x;
        float y = followXOnly ? transform.position.y : followingPosition.y;
        float z = transform.position.z;

        transform.position = new Vector3(x, y, z);
    }
}
