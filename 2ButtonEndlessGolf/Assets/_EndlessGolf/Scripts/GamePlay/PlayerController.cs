using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLib;

public class PlayerController : MonoBehaviour
{
    // Use this for initialization
    public static event System.Action PlayerDied;

    Vector3 lastPosition;
    Vector3 newPosition;
    bool trackMouse = false;

    private float force;
    GameObject invisibleArrow;
    GameObject arrow;
    Rigidbody2D myRB;
    GameObject targetInArrow;
    GameObject targetArrow;
    [HideInInspector]
    public bool canDrag = true;

    [HideInInspector]
    public int hitcount;

    Color isNotMoving;
    Color isMoving;
    float checkRate = 0.2f;
    float startTime;

    Vector3 currentPlayerPosition;

    private Transform checkGround;
    private float checkGroundRadius;
    private LayerMask layer;
    [HideInInspector]
    public bool isGrounded;
    CircleCollider2D cCollider;

    bool checkTwo = false;
    [HideInInspector]
    public bool canHit = true;
    [HideInInspector]
    public bool isHoldingMouse = false;

    private TrailRenderer trailRender;
    private float timeTrail;

    void OnEnable()
    {
        trailRender = transform.GetComponent<TrailRenderer>();
        timeTrail = trailRender.time;
        PlaneGen1.ResetPositionPlayer += DisableTrail;
        PlaneGen2.ResetPositionPlayer += DisableTrail;
    }

    void OnDisable()
    {
        PlaneGen1.ResetPositionPlayer -= DisableTrail;
        PlaneGen2.ResetPositionPlayer -= DisableTrail;
    }
    // Calls this when the player dies and game over
    public void Die()
    {
        // Fire event
        PlayerDied();
    }

    private void Start()
    {
        checkGround = GameManager.Instance.checkGround;
        checkGroundRadius = GameManager.Instance.checkGroundRadius;
        layer = GameManager.Instance.groundLayer;
        invisibleArrow = GameManager.Instance.inviArrow;
        arrow = GameManager.Instance.dragArrow;
        targetArrow = GameManager.Instance.targetArrow;
        targetInArrow = GameManager.Instance.targetInArrow;
        hitcount = GameManager.Instance.numberOfStroke;
        force = GameManager.Instance.force;
        isMoving = GameManager.Instance.isMoving;
        isNotMoving = GameManager.Instance.isNotMoving;
        checkRate = GameManager.Instance.checkRate;
        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
        startTime = Time.time;
        myRB = GetComponent<Rigidbody2D>();
        arrow.SetActive(false);
        cCollider = GetComponent<CircleCollider2D>();

        cCollider.sharedMaterial.friction = GameManager.Instance.friction;
        cCollider.sharedMaterial.bounciness = GameManager.Instance.bounciness;
    }


    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, layer);
        CheckObjectMoving();

        if (!isGrounded)
            myRB.AddForce(new Vector2(-GameManager.Instance.windForce, 0));

        if (canDrag && hitcount == 0)
        {
            StartCoroutine(CheckHitCount());
        }
        MouseInput();

        if (isHoldingMouse)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                isHoldingMouse = false;
            }
        }

        if (Camera.main.GetComponent<CameraFollow>().isMoving)
            arrow.SetActive(false);
    }

    IEnumerator CheckHitCount()
    {
        yield return new WaitForSeconds(0.5f);
        if (canDrag)
        {
            yield return new WaitForSeconds(0.5f);
            if (hitcount == 0 && !checkTwo)
            {
                checkTwo = true;
                Die();
            }
        }
    }

    void MouseInput()
    {
        if (!isHoldingMouse)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                trackMouse = true;
                arrow.SetActive(true);
                lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                invisibleArrow.transform.position = transform.position;
                arrow.transform.position = lastPosition;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                trackMouse = false;

                Vector2 dir = targetInArrow.transform.position - transform.position;
                dir = dir.normalized;
                if (canDrag && hitcount > 0 && canHit)
                {
                    myRB.AddForce(dir * force * (newPosition - lastPosition).magnitude);
                    hitcount--;
                    SoundManager.Instance.PlaySound(SoundManager.Instance.hit);
                }
                else myRB.AddForce(new Vector2(0, 0));
                arrow.SetActive(false);
            }

            if (trackMouse)
            {
                newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 mouse = Input.mousePosition;
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(lastPosition);
                Vector2 offset = new Vector2(mouse.x - screenPoint.x, mouse.y - screenPoint.y);
                float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

                invisibleArrow.transform.rotation = Quaternion.Euler(0, 0, angle);
                invisibleArrow.transform.position = transform.position;
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

                arrow.transform.Find("Body").localScale = new Vector3((newPosition - lastPosition).magnitude, arrow.transform.Find("Body").localScale.y, 1);
                arrow.transform.Find("Head").position = targetArrow.transform.position;

                if (canDrag && hitcount > 0)
                {
                    arrow.transform.Find("Body").GetComponent<SpriteRenderer>().color = isMoving;
                    arrow.transform.Find("Head").GetComponent<SpriteRenderer>().color = isMoving;
                }
                else
                {
                    arrow.transform.Find("Body").GetComponent<SpriteRenderer>().color = isNotMoving;
                    arrow.transform.Find("Head").GetComponent<SpriteRenderer>().color = isNotMoving;
                }
            }
        }
    }

    void CheckObjectMoving()
    {

        if (Time.time > startTime)
        {
            if (Mathf.Abs(currentPlayerPosition.magnitude - transform.position.magnitude) < 0.0008f)
            {
                if (!Camera.main.GetComponent<CameraFollow>().isMoving)
                {
                    canDrag = true;
                }
                else
                {
                    if (Input.GetButton("Fire1"))
                    {
                        isHoldingMouse = true;
                    }
                    canDrag = false;
                }
            }
            else
                canDrag = false;
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
