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
    public float MoveToCheckpointSpeed;

    public int YaccelerationForce;
    public int YdecelerationForce;
    public int YspeedMax;
    float hueValue;
    public bool isDead = false;

    public bool hasArrivedAtCheckpoint;
    public Vector2 playerPosition;

    private CheckPointDetector TheCheckpointDetector;

    private float CheckpointArrivalY;

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

        hueValue = Random.Range(0, 10) / 10.0f;
        SetBackgroundColor();
    }


    void Update()
    {
        if (isDead) return;
        MovePlayer();

        print("Player has arrived at checkpoint = " + hasArrivedAtCheckpoint);
        CheckpointArrivalY = TheCheckpointDetector.CurrentSelectedCheckpointPositionY;
        if (transform.position.y == CheckpointArrivalY)
        {
            hasArrivedAtCheckpoint = true;
        } else
        {
            hasArrivedAtCheckpoint = false;
        }
    }


    void MovePlayer()
    {
        Vector2 pos = transform.position;
        
        pos.x = Mathf.Cos(angle) * (GameManagerObj.GetComponent<DisplayManager>().RIGHT * 0.9f);
        //pos.y += 0.002f; This usually made the player move up slowly
        //But we don't want there to be consequences when Jeroen does not move.
        transform.position = pos;
        angle += Time.deltaTime * Xspeed;
        
        if (Input.GetMouseButton(0))
        {
            //if (!hasArrivedAtCheckpoint && transform.position.y <= CheckpointArrivalY) { }
            TheCheckpointDetector.FreezeCurrentSelection(); //FREEZE THE SELECTED CHECKPOINT UNTIL REACHED!
            transform.position = Vector2.MoveTowards(transform.position, TheCheckpointDetector.CurrentSelectedCheckpointPosition, (MoveToCheckpointSpeed / 4));
        }
        else
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
