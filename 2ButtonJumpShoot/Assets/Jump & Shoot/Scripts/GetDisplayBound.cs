using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDisplayBound : MonoBehaviour
{

    float mapX = 100.0f;
    [HideInInspector]
    public float Left;
    [HideInInspector]
    public float Right;

    public float desiredOffset = 10f;

    public Transform leftWall;
    public Transform rightWall;

    void Awake()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        float minX = horzExtent - mapX / 2.0f;
        float maxX = mapX / 2.0f - horzExtent;

        Left = Mathf.Max(maxX - 50, -desiredOffset);
        Right = Mathf.Min(minX + 50, desiredOffset);
        leftWall.position = new Vector3(Left - .5f, 0);
        rightWall.position = new Vector3(Right + .5f, 0);
    }

    private void OnEnable()
    {
        Player.PlayerLandedOnPlatform += Player_PlayerLandedOnPlatform;
    }

    private void OnDisable()
    {
        Player.PlayerLandedOnPlatform -= Player_PlayerLandedOnPlatform;
    }

    private void Player_PlayerLandedOnPlatform(Vector3 playerPosition)
    {
        leftWall.position = new Vector3(Left - .5f, playerPosition.y);
        rightWall.position = new Vector3(Right + .5f, playerPosition.y);
    }
}