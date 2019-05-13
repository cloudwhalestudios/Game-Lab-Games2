using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public GameObject fx_Dead;
    public GameObject fx_ColorChange;
    GameObject GameManagerObj;

    [Space]
    public AudioClip DeadClip;
    public AudioClip ItemClip;
    AudioSource source;

    Rigidbody2D rb;

    float angle = 0;

    [Space]
    public float Xspeed;
    public float transitionSpeed;

    public int YaccelerationForce;
    public int YdecelerationForce;
    public int YspeedMax;
    float hueValue;
    public bool isDead = false;

    public bool hasArrivedAtCheckpoint;
    public Vector2 playerPosition;

    private CheckPointDetector TheCheckpointDetector;

    private float CheckpointArrivalY;

    private Coroutine activeMoveCoroutine;
    private bool isMoving = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance);
            Instance = this;
        }
    }

    void Start()
    {
        GameManagerObj = GameObject.Find("GameManager");
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();

        TheCheckpointDetector = FindObjectOfType<CheckPointDetector>();

        hasArrivedAtCheckpoint = true;

        hueValue = Random.Range(0, 10) / 10.0f;
        SetBackgroundColor();
    }


    void Update()
    {
        if (isDead) return;
        MovePlayer();
        CheckpointArrivalY = TheCheckpointDetector.CurrentSelectedCheckpointPositionY;
    }


    void MovePlayer()
    {
        if (!isMoving)
        {
            Vector2 pos = transform.position;
            pos.x = Mathf.Cos(angle) * (GameManagerObj.GetComponent<DisplayManager>().RIGHT * 0.9f);
            transform.position = pos;
            angle += Time.deltaTime * Xspeed;
        }

        if (Input.GetMouseButton(0) && hasArrivedAtCheckpoint)
        {
            activeMoveCoroutine = StartCoroutine(MoveToCheckpoint());
        }
        else if (!isMoving)
        {
            if (rb.velocity.y > 0)
            {
                rb.AddForce(new Vector2(0, -YdecelerationForce));
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }

    }

    IEnumerator MoveToCheckpoint()
    {
        hasArrivedAtCheckpoint = false;
        isMoving = true;
        print("Moving to Checkpoints");
        TheCheckpointDetector.FreezeCurrentSelection();
        var currentCheckpointPosition = new Vector3(TheCheckpointDetector.CurrentSelectedCheckpointPosition.x, TheCheckpointDetector.CurrentSelectedCheckpointPosition.y, transform.position.z);
        var distanceCheckpoint = Vector3.Distance(currentCheckpointPosition, transform.position);
        while (transform.position != currentCheckpointPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentCheckpointPosition, transitionSpeed * Time.deltaTime); //(Time.deltaTime / transitionSpeed) * distanceCheckpoint)
            yield return new WaitForEndOfFrame();
        }
        angle = Mathf.Acos(transform.position.x / (GameManagerObj.GetComponent<DisplayManager>().RIGHT * 0.9f));
        hasArrivedAtCheckpoint = true;
        isMoving = false;
        print("Has arrived !!");
        TheCheckpointDetector.UnfreezeCurrentSelection();
        yield break;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item_ColorChange")
        {
            Destroy(Instantiate(fx_ColorChange, other.gameObject.transform.position, Quaternion.identity), 0.5f);
            Destroy(other.gameObject.transform.parent.gameObject);
            SetBackgroundColor();

            GameManagerObj.GetComponent<GameManager>().addScore();

            source.PlayOneShot(ItemClip, 1);
        }

        if (other.gameObject.tag == "Obstacle" && isDead == false)
        {
            isDead = true;

            Destroy(Instantiate(fx_Dead, transform.position, Quaternion.identity), 0.5f);
            StopPlayer();
            GameManagerObj.GetComponent<GameManager>().Gameover();

            source.PlayOneShot(DeadClip, 1);
        }

        if (other.gameObject.tag == "CheckpointDetector")
        {
            return;
        }
    }

    void StopPlayer()
    {
        rb.velocity = new Vector2(0, 0);
        rb.isKinematic = true;
    }


    void SetBackgroundColor()
    {
        hueValue += 0.1f;
        if (hueValue >= 1)
        {
            hueValue = 0;
        }
        Camera.main.backgroundColor = Color.HSVToRGB(hueValue, 0.6f, 0.8f);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
