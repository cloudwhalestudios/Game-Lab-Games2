using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionEventHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Player.PlayerJumped += Player_PlayerJumped;
        Player.PlayerCrossedPlatform += Player_PlayerCrossedPlatform;
        Player.PlayerShot += Player_PlayerShot;
        Player.PlayerLandedOnPlatform += Player_PlayerLandedOnPlatform;
    }
    private void OnDisable()
    {
        Player.PlayerJumped -= Player_PlayerJumped;
        Player.PlayerCrossedPlatform -= Player_PlayerCrossedPlatform;
        Player.PlayerShot -= Player_PlayerShot;
        Player.PlayerLandedOnPlatform -= Player_PlayerLandedOnPlatform;
    }

    private void Player_PlayerLandedOnPlatform(Vector3 position)
    {
        TimeScaleController.Instance.SetSlowMotionActive(false);
    }

    private void Player_PlayerShot()
    {
        
    }

    private void Player_PlayerCrossedPlatform()
    {
        TimeScaleController.Instance.SetSlowMotionActive();
    }

    private void Player_PlayerJumped()
    {
        TimeScaleController.Instance.SetSlowMotionActive(false);
    }
}
