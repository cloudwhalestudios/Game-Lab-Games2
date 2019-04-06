using AccessibilityInputSystem;
using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event Action PlayerJumped;
    public static event Action PlayerShot;
    public static event Action<Vector3> PlayerLandedOnPlatform;
    public static event Action PlayerCrossedPlatform;

    public enum PlayerState
    {
        Standing, Jumping, Falling
    }
    [HideInInspector] public PlayerState currentState;

    public Transform playerParentTransform;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 3, 0);

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

    float previousPosX;
    float previousPosYofParent;

    [HideInInspector] public bool jump = false;
    [HideInInspector] public bool shoot = false;

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
        //playerParentTransform = BasePlayerManager.Instance.playerParent;

    }

    void Start()
    {
        LeftEnd = GameManager.Instance.GetComponent<GetDisplayBound>().Left;
        RightEnd = GameManager.Instance.GetComponent<GetDisplayBound>().Right;

        InitPlayer();
    }

    public void InitPlayer()
    {
        trailRenderer.startWidth = transform.localScale.x;
        trailRenderer.endWidth = transform.localScale.x;

        currentState = PlayerState.Falling;
        rb.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;

        transform.position = spawnPosition;

        isDead = false;
        jump = false;
        shoot = false;

        hasCrossedPlatform = false;
        isOnPlatform = false;

        rb.isKinematic = false;
        bc2D.enabled = true;
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
            PlayerLandedOnPlatform?.Invoke(transform.position);
        }
    }

    void Update()
    {
        if (jump)
        {
            Jump();
            jump = false;
        }
        if (shoot)
        {
            StartCoroutine(Shoot());
            shoot = false;
        }

        BounceAtWall();
        DeadCheck();

        previousPosX = transform.position.x;

        if (currentState == PlayerState.Jumping)
        {
            if (!TimeScaleController.Instance.IsPaused)
            {
                transform.Rotate(Vector3.forward * Time.unscaledDeltaTime * rb.velocity.x * (-30));
            }

            if (transform.position.y >= previousPosYofParent + StepManager.Instance.DistanceToNextStep)
            {
                PlayerHasCrossedPlatform();
            }

            if (PlayerIsFalling())
            {
                bc2D.enabled = true;
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.Jump);
        PlayerStartedJumping();
        JumpEffect();

        previousPosYofParent = transform.parent.transform.position.y;

        float horizontalVelocity = (transform.position.x - previousPosX) / Time.deltaTime;
        rb.velocity = new Vector2(horizontalVelocity, jumpSpeed);

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
            AudioManager.Instance.PlaySound(AudioManager.Instance.Death);
            isDead = true;
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;

            Destroy(Instantiate(fx_Dead, transform.position, Quaternion.identity), 1.0f);
            GameManager.Instance.GameOver();
        }
    }

    IEnumerator Shoot()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Shoot);

        PlayerStartedShooting();

        currentState = PlayerState.Falling;

        transform.rotation = Quaternion.identity;

        ShootEffect1();

        rb.isKinematic = true;
        rb.velocity = new Vector2(0, 0);

        yield return new WaitForSecondsRealtime(0.5f);

        AudioManager.Instance.PlaySound(AudioManager.Instance.Fly);

        ShootEffect2();
        ColorChanger.ChangeBackgroundColor();

        rb.isKinematic = false;
        rb.velocity = new Vector2(0, -shootSpeed / Time.timeScale);

        bc2D.enabled = true;

        yield break;
    }

    void ShootEffect1()
    {
        GameObject tempObj = Instantiate(fx_Shoot_1, transform.position, Quaternion.identity);
        tempObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.HSVToRGB(ColorChanger.hueValue, 0.6f, 0.8f);
        Destroy(tempObj, 1.0f);
    }

    void ShootEffect2()
    {
        GameObject EffectObj = Instantiate(fx_Shoot_2, transform.position, Quaternion.identity);
        Destroy(EffectObj, 0.5f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isActiveAndEnabled && other.gameObject.tag == "Step" && PlayerIsFalling())
        {
            if (playerParentTransform == null)
            {
                playerParentTransform = BasePlayerManager.Instance.playerParent;
            }

            AudioManager.Instance.PlaySound(AudioManager.Instance.BassCrash);

            PlayerHasLandedOnPlatform();

            Destroy(Instantiate(fx_Land, transform.position, Quaternion.identity), 0.5f);

            rb.velocity = new Vector2(0, 0);
            transform.rotation = Quaternion.identity;
            var pos = transform.position;
            pos.y = other.transform.position.y + 0.5f;
            transform.position = pos;

            currentState = PlayerState.Standing;

            transform.SetParent(other.gameObject.transform);

            other.gameObject.GetComponent<Step>().StartCoroutine_LandingEffect();

            GameManager.Instance.AddScore(1);
        }
    }

    bool PlayerIsFalling()
    {
        return (currentState == PlayerState.Falling || (currentState == PlayerState.Jumping && rb.velocity.y <= 0));
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (!isActiveAndEnabled) return;
        StepManager.Instance.MakeStep();
        StepDestroyEffect(other);

        //Debug.Log("Is player parent: " + (other.transform.Equals(transform.parent)));

        Destroy(other.gameObject, 0.1f);
    }


    void StepDestroyEffect(Collision2D stepCollision)
    {
        GameObject fxObj = Instantiate(fx_StepDestroy, stepCollision.gameObject.transform.position, Quaternion.identity);
        fxObj.transform.localScale = stepCollision.transform.localScale;
        Destroy(fxObj, 0.5f);
    }
}
