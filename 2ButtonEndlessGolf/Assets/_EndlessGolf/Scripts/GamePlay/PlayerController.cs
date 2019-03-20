using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO convert to base class
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int _playerID = -1;

    public int PlayerID
    {
        protected set
        {
            _playerID = value;
        }
        get
        {
            return _playerID;
        }
    }

    [SerializeField] private Rigidbody2D myRB;
    [SerializeField] private CircleCollider2D cCollider;

    [SerializeField] private int strokesTaken;

    [SerializeField] private Transform checkGround;
    
    [SerializeField] private TrailRenderer trailRender;
    [SerializeField] private float timeTrail;

    [SerializeField] private Vector3 currentPlayerPosition;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool allowHit;
    [SerializeField] private float startTime;

    [SerializeField] private int score;
    public int availableUndos;
    

    void OnEnable()
    {
        trailRender = transform.GetComponent<TrailRenderer>();
        timeTrail = trailRender.time;
    }

    private void Start()
    {
        // Get components
        myRB = GetComponent<Rigidbody2D>();
        cCollider = GetComponent<CircleCollider2D>();

        // Apply game manager configuration
        trailRender.material.color = GameManager.Instance.isNotMoving;
        cCollider.sharedMaterial.friction = GameManager.Instance.friction;
        cCollider.sharedMaterial.bounciness = GameManager.Instance.bounciness;
        availableUndos = GameManager.Instance.availableUndosPerLevel;

        // Move the transform infront of other sprites (z-axis)
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);

     }

    public void Init(int id)
    {
        PlayerID = id;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, GameManager.Instance.checkGroundRadius, GameManager.Instance.groundLayer);
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
            startTime = Time.time + GameManager.Instance.checkRate;
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
