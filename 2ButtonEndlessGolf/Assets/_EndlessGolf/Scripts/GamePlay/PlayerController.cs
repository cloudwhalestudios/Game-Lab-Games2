using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLib;

public class PlayerController : MonoBehaviour
{
    // Use this for initialization
    public static event System.Action PlayerDied;

    [SerializeField] private Rigidbody2D myRB;
    [SerializeField] private CircleCollider2D cCollider;

    [SerializeField] private int strokesTaken;

    [SerializeField] private float startTime;

    [SerializeField] private Transform checkGround;
    [SerializeField] private float checkGroundRadius;
    [SerializeField] private LayerMask layer;

    [SerializeField] private TrailRenderer trailRender;
    [SerializeField] private float timeTrail;

    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private Vector3 currentPlayerPosition;

    [SerializeField] private float checkRate = 0.2f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool allowHit;

    [SerializeField] private float force;
    [SerializeField] private int score;

    void OnEnable()
    {
        trailRender = transform.GetComponent<TrailRenderer>();
        timeTrail = trailRender.time;
    }

    // Calls this when the player dies and game over
    public void Die()
    {
        // Fire event
        PlayerDied();
    }

    private void Start()
    {

        myRB = GetComponent<Rigidbody2D>();
        cCollider = GetComponent<CircleCollider2D>();

        // Apply game manager configuration
        checkGroundRadius = GameManager.Instance.checkGroundRadius;
        layer = GameManager.Instance.groundLayer;
        force = GameManager.Instance.force;
        trailRender.material.color = GameManager.Instance.isNotMoving;
        checkRate = GameManager.Instance.checkRate;
        cCollider.sharedMaterial.friction = GameManager.Instance.friction;
        cCollider.sharedMaterial.bounciness = GameManager.Instance.bounciness;

        // Move the transform infront of other sprites (z-axis)
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
        startTime = Time.time;
    }


    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, layer);
        CheckObjectMoving();

        if (!isGrounded)
            myRB.AddForce(new Vector2(-GameManager.Instance.windForce, 0));
    }

    void CheckObjectMoving()
    {
        if (Time.time > startTime)
        {
            if (Mathf.Abs(currentPlayerPosition.magnitude - transform.position.magnitude) < 0.0008f)
            {
                isMoving = false;
                allowHit = true;
            }
            else
                allowHit = false;
            currentPlayerPosition = transform.position;
            startTime = Time.time + checkRate;
        }
    }

    void DisableTrail()
    {
        StartCoroutine(DisableTrail_IEnum());
    }

    IEnumerator DisableTrail_IEnum()
    {
        trailRender.time = 0;
        yield return new WaitForSeconds(timeTrail + 0.03f);
        trailRender.time = timeTrail;
    }
}
