using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO convert to base class
public class PlayerController : MonoBehaviour
{
    [SerializeField, ReadOnly] private int _playerID = -1;

    public int PlayerID { get => _playerID; set => _playerID = value; }
    public Rigidbody2D Rb { get => _rb; set => _rb = value; }
    public int StrokesTaken { get => _strokesTaken; set => _strokesTaken = value; }

    public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
    public bool IsMoving { get => _isMoving; set => _isMoving = value; }

    [SerializeField, ReadOnly] private Rigidbody2D _rb;
    [SerializeField, ReadOnly] private CircleCollider2D circleCollider;

    [SerializeField, ReadOnly] private int _strokesTaken;

    [SerializeField, ReadOnly] private Transform checkGround;
    
    [SerializeField, ReadOnly] private TrailRenderer trailRender;
    [SerializeField, ReadOnly] private SpriteRenderer spriteRenderer;

    [SerializeField, ReadOnly] private Vector3 currentPlayerPosition;
    [SerializeField, ReadOnly] private bool _isGrounded;
    [SerializeField, ReadOnly] private bool _isMoving;
    [SerializeField, ReadOnly] private float startTime;

    [SerializeField, ReadOnly] private int score;
    public int availableUndos;

    public void Init(int id)
    {
        PlayerID = id;
        startTime = Time.time;

        // Get components
        Rb = GetComponent<Rigidbody2D>();
        Rb.isKinematic = true;
        circleCollider = GetComponent<CircleCollider2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (trailRender == null)
        {
            trailRender = GetComponent<TrailRenderer>();
        }

        // Apply game manager configuration
        circleCollider.sharedMaterial.friction = GameManager.Instance.friction;
        circleCollider.sharedMaterial.bounciness = GameManager.Instance.bounciness;
        availableUndos = GameManager.Instance.availableUndosPerLevel;

        // Move the transform infront of other sprites (z-axis)
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
    }

    public void SetColor(Color color)
    {
        trailRender.material.color = color;
        spriteRenderer.color = color;
    }

    void Update()
    {
        IsGrounded = Physics2D.OverlapCircle(checkGround.position, GameManager.Instance.checkGroundRadius, GameManager.Instance.groundLayer);
        CheckObjectMoving();

        /*if (!isGrounded)
            myRB.AddForce(new Vector2(-GameManager.Instance.windForce, 0));*/
    }

    void CheckObjectMoving()
    {
        if (Time.time > startTime && IsMoving)
        {
            if (Mathf.Abs(currentPlayerPosition.magnitude - transform.position.magnitude) < 0.0008f)
            {
                Debug.Log(PlayerID + ": No Longer Moving");
                IsMoving = false;
                Rb.velocity = Vector2.zero;
            }
            currentPlayerPosition = transform.position;
            startTime = Time.time + GameManager.Instance.checkRate;
        }
    }
}
