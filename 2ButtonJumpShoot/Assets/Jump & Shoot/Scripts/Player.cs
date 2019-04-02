using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action PlayerJumped;
    public static event Action PlayerShot;
    public static event Action PlayerLandedOnPlatform;
    public static event Action PlayerCrossedPlatform;

    enum PlayerState
    {
        Standing, Jumping, Falling
    }
    PlayerState currentState;

    public Transform playerParentTransform;
  
    [Space]
    public GameObject fx_Shoot_1;
    public GameObject fx_Shoot_2;
    public GameObject fx_Jump;
    public GameObject fx_Land;
    public GameObject fx_StepDestroy;
    public GameObject fx_Dead;
    [Space]
    public int jumpSpeed;
    public int shootSpeed;

    Rigidbody2D rb;
    TrailRenderer trailRenderer;
    BoxCollider2D bc2D;

    float previousPosXofParent;

    float hueValue;

    bool isDead = false;

    bool hasCrossedPlatform = false;
    bool isOnPlatform = false;

    float LeftEnd;
    float RightEnd;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc2D = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        LeftEnd = GameManager.Instance.GetComponent<GetDisplayBound>().Left;
        RightEnd = GameManager.Instance.GetComponent<GetDisplayBound>().Right;

        InitPlayer();

        hueValue = UnityEngine.Random.Range(0, 10) / 10.0f;
        ChangeBackgroundColor();
    }

    void PlayerStartedJumping()
    {
        isOnPlatform = false;
        hasCrossedPlatform = false;

        PlayerJumped?.Invoke();
    }

    void PlayerHasCrossedPlatform()
    {
        if(!hasCrossedPlatform)
        {
            hasCrossedPlatform = true;
            PlayerCrossedPlatform?.Invoke();
        }
    }

    void PlayerStartedShooting()
    {
        PlayerShot?.Invoke();
    }

    void PlayerHasLandedOnPlatform()
    {
        if (!isOnPlatform)
        {
            isOnPlatform = true;
            PlayerLandedOnPlatform?.Invoke();
        }
    }

    void InitPlayer()
    {
        trailRenderer.startWidth = transform.localScale.x;
        trailRenderer.endWidth = transform.localScale.x;

        currentState = PlayerState.Falling;
        rb.velocity = new Vector2(0, 0);
    }

    void Update()
    {
        GetInput();
        BounceAtWall();
        DeadCheck();

        previousPosXofParent = transform.parent.transform.position.x;

        if (currentState == PlayerState.Jumping)
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * rb.velocity.x * (-30));

            if (transform.position.y >= playerParentTransform.position.y + StepManager.Instance.DistanceToNextStep)
            {
                PlayerHasCrossedPlatform();
            }
        }
    }



    void GetInput()
    {
        // TODO change to primary
        if (Input.GetMouseButtonDown(0))
        {
            if (currentState == PlayerState.Standing)
            {
                Jump();
            }
            else if (currentState == PlayerState.Jumping)
            {
                StartCoroutine(Shoot());
            }
        }
    }


    void BounceAtWall()
    {
        if (rb.position.x < LeftEnd)
        {
            rb.position = new Vector2(LeftEnd, rb.position.y);
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }

        if (rb.position.x > RightEnd)
        {
            rb.position = new Vector2(RightEnd, rb.position.y);
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
    }


    void Jump()
    {
        PlayerStartedJumping();
        JumpEffect();

        float parentVelocity = (transform.parent.transform.position.x - previousPosXofParent) / Time.deltaTime;
        rb.velocity = new Vector2(parentVelocity, jumpSpeed);

        currentState = PlayerState.Jumping;

        bc2D.enabled = false;

        transform.SetParent(playerParentTransform);
    }


    void JumpEffect()
    {
        GameObject effectObj = Instantiate(fx_Jump, transform.position, Quaternion.identity);
        Destroy(effectObj, 0.5f);
    }


    void DeadCheck()
    {
        if (isDead == false && Camera.main.transform.position.y - transform.position.y > 10)
        {
            isDead = true;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;

            Destroy(Instantiate(fx_Dead, transform.position, Quaternion.identity), 1.0f);


            GameManager.Instance.GameOver();
        }
    }

    IEnumerator Shoot()
    {
        PlayerStartedShooting();

        transform.rotation = Quaternion.identity;

        currentState = PlayerState.Falling;

        ShootEffect1();

        rb.isKinematic = true;
        rb.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(0.5f);

        ShootEffect2();
        ChangeBackgroundColor();

        rb.isKinematic = false;
        rb.velocity = new Vector2(0, -shootSpeed);

        bc2D.enabled = true;

        yield break;
    }

    void ShootEffect1()
    {
        GameObject tempObj = Instantiate(fx_Shoot_1, transform.position, Quaternion.identity);
        tempObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hueValue, 0.6f, 0.8f);
        Destroy(tempObj, 1.0f);
    }

    void ShootEffect2()
    {
        GameObject EffectObj = Instantiate(fx_Shoot_2, transform.position, Quaternion.identity);
        Destroy(EffectObj, 0.5f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Step" && currentState == PlayerState.Falling && rb.velocity == Vector2.zero)
        {
            PlayerHasLandedOnPlatform();

            Destroy(Instantiate(fx_Land, transform.position, Quaternion.identity), 0.5f);

            rb.velocity = new Vector2(0, 0);
            currentState = PlayerState.Standing;

            transform.SetParent(other.gameObject.transform);

            other.gameObject.GetComponent<Step>().StartCoroutine_LandingEffect();

            GameManager.Instance.AddScore(1);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        StepManager.Instance.MakeStep();
        StepDestroyEffect(other);

        Destroy(other.gameObject, 0.1f);
    }


    void StepDestroyEffect(Collision2D stepCollision)
    {
        GameObject fxObj = Instantiate(fx_StepDestroy, stepCollision.gameObject.transform.position, Quaternion.identity);
        fxObj.transform.localScale = stepCollision.transform.localScale;
        Destroy(fxObj, 0.5f);
    }

    void ChangeBackgroundColor()
    {
        Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.6f, 0.8f);
        hueValue += 0.1f;

        if (hueValue >= 1)
        {
            hueValue = 0;
        }
    }
}
