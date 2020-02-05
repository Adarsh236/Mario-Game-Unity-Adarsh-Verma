using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{

    public Transform playerTransform;

    void Update()
    {
        float playerPositionX = playerTransform.position.x;

        if (playerPositionX > transform.position.x)
        {
            transform.position = new Vector3 (playerPositionX, transform.position.y, transform.position.z);
        }

        float worldXConstraint = Camera.main.orthographicSize * Screen.width / Screen.height - 0.5f;

        playerTransform.position = new Vector3(Mathf.Max(playerTransform.position.x, transform.position.x - worldXConstraint), playerTransform.position.y, playerTransform.position.z);
    }
}
