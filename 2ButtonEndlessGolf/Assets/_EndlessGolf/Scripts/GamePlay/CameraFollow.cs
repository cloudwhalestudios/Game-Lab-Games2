using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [Header("Config")]
    [SerializeField]
    private GameObject holeFlag;

    [SerializeField]
    private GroundManager groundManager;
    Camera cam;
    Vector3 start;
    GameObject player;
    [HideInInspector]
    public bool isMoving = false;
    float height;
    float width;

    Vector3 a;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = Camera.main;
        transform.position = new Vector2(
            GameObject.FindGameObjectWithTag("Ground2").transform.position.x,
            GameManager.Instance.len / 2 + cam.orthographicSize / 2);
        start = transform.position;
        height = 2f * cam.orthographicSize;
        width = height * cam.aspect;
        GameManager.Instance.width = width;

    }

    public void Move()
    {
        float cal = width / 2 + (player.transform.position.x - transform.position.x) - GameManager.Instance.startPointSize / 2;
        StartCoroutine(CRMoving(GameManager.Instance.smoothTime, cal));
    }

    IEnumerator CRMoving(float moveTime, float cal)
    {
        isMoving = true;
        float startTime = Time.time;
        while (Time.time - startTime < moveTime)
        {
            isMoving = true;
            float f = (Time.time - startTime) / moveTime;
            transform.position = Vector3.Lerp(start, start + new Vector3(cal, 0, 0), f);
            yield return null;
        }
        Vector3 target = holeFlag.transform.position;
        target.x = transform.position.x;
        target.y = groundManager.GetCenterY(groundManager.currentGround) + GameManager.Instance.distanceBottoCam;
        target.z = transform.position.z;
        while (transform.position.y != target.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 30 * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
        start = transform.position;
        player.GetComponent<PlayerController>().canHit = true;
        GameManager.Instance.niceHit = false;
    }
}
