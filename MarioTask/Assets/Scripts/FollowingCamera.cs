using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{

    public Transform playerTransform;

    private float gameNotFinished= 115f;
    private float gameFinished = 121.7f;// animation start

    void Update()
    {
        float playerPositionX;
        if (playerTransform.position.x < gameNotFinished)
        {
            playerPositionX = playerTransform.position.x;
        }
        else
        {
            playerPositionX = gameFinished;// fixed camera
        }

        if (playerPositionX > transform.position.x)
        {
            transform.position = new Vector3 (playerPositionX, transform.position.y, transform.position.z);
        }

        float worldXConstraint = Camera.main.orthographicSize * Screen.width / Screen.height - 0.5f;

        playerTransform.position = new Vector3(Mathf.Max(playerTransform.position.x, transform.position.x - worldXConstraint), playerTransform.position.y, playerTransform.position.z);
    }
}
