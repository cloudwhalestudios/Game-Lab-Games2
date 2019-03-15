using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SgLib;

public class PlaneGen2 : MonoBehaviour
{
    public static event System.Action ResetPositionPlayer;
    float ratio;

    Vector3 playerStartPos;

    [HideInInspector]
    public bool isOutOfScreen = false;
    [HideInInspector]
    public bool win = false;
    [HideInInspector]
    public bool isCheck = false;

    int middlePoint = 0;

    GameObject player;
    PolygonCollider2D pCollider;

    Mesh mesh;
    [HideInInspector]
    public Vector3[] vertices;
    [HideInInspector]
    public int startHolePoint;
    [HideInInspector]
    public int endHolePoint;
    [HideInInspector]
    public int endPoint;

    GroundManager gManager;

    GameObject ground1;

    PlaneGen1 g1;
    PlaneGen3 g3;

    [HideInInspector]
    public Vector3 a;
    [HideInInspector]
    public Vector3 b;
    [HideInInspector]
    public RaycastHit2D hit;

    Vector3 w;
    Vector3 e;

    public int curStroke = 5;

    void Start()
    {
        ground1 = GameObject.FindGameObjectWithTag("Ground1");
        g1 = ground1.GetComponent<PlaneGen1>();
        g3 = GameObject.FindGameObjectWithTag("Ground3").GetComponent<PlaneGen3>();
        ratio = (GameManager.Instance.holeHeight / 2) / ((GameManager.Instance.holeWidth / 2) * (GameManager.Instance.holeWidth / 2));
        vertices = new Vector3[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        pCollider = gameObject.AddComponent<PolygonCollider2D>();
        gManager = GameObject.FindGameObjectWithTag("GroundGenerator").GetComponent<GroundManager>();

        a = new Vector3(-GameManager.Instance.width / 2, GameManager.Instance.len / 2, 0);
        b = a + new Vector3(GameManager.Instance.startPointSize, 0, 0);

        GenerateFirst(a, b);
        GetPlayerPosition(a, b);
    }


    // Update is called once per frame
    void Update()
    {
        if (gManager.currentGround == 2)
        {
            Vector3 test = Camera.main.WorldToViewportPoint(player.transform.position);
            if (test.x >= 1 || test.x <= 0)
            {
                isOutOfScreen = true;
                if (ResetPositionPlayer != null)
                    ResetPositionPlayer();
                player.transform.position = playerStartPos;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            else isOutOfScreen = false;
        }
        if (gManager.currentGround == 2)
        {
            w = new Vector3(vertices[middlePoint - 2].x + transform.position.x, vertices[middlePoint - 2].y + transform.position.y, 5);
            e = new Vector3(vertices[middlePoint + 2].x + transform.position.x, vertices[middlePoint + 2].y + transform.position.y, 5);
        }
        else
        {
            w = new Vector3(0, 0, 0);
            e = new Vector3(0, 0, 0);
        }

        hit = Physics2D.Linecast(w, e, 1 << LayerMask.NameToLayer("Player"));
        if (hit)
        {
            player.GetComponent<PlayerController>().canHit = false;
        }

        if (hit && player.GetComponent<PlayerController>().canDrag && !isCheck)
        {
            Camera.main.GetComponent<CameraFollow>().isMoving = true;
            isCheck = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.win);
            ScoreManager.Instance.AddScore(1);
            win = true;
            ground1.transform.position = new Vector3(transform.position.x + GameManager.Instance.width / (GameManager.Instance.levelComplicated - 1) * (startHolePoint - 1) - GameManager.Instance.width / 2, transform.position.y, 5);
            g1.Generate(new Vector2(g1.a.x, vertices[startHolePoint].y), new Vector2(g1.a.x, vertices[startHolePoint].y) + new Vector2(GameManager.Instance.holeWidth, 0));
            Camera.main.GetComponent<CameraFollow>().Move();
            ChangeVertex();
            g1.win = false;
            if (player.GetComponent<PlayerController>().hitcount == curStroke - 1)
                GameManager.Instance.niceHit = true;
            g1.isCheck = false;
            gManager.currentGround = 1;
            player.GetComponent<PlayerController>().hitcount += GameManager.Instance.strokeAdd;
            Invoke("GenerateG3", GameManager.Instance.smoothTime / 2);
            g1.curStroke = player.GetComponent<PlayerController>().hitcount;
        }
    }

    void GenerateG3()
    {
        g3.Generate();
    }

    public void GetPlayerPosition(Vector3 start, Vector3 end)
    {
        Vector3 test0 = transform.TransformPoint(start);
        Vector3 test1 = transform.TransformPoint(end);
        playerStartPos = new Vector3((test0.x + test1.x) / 2, test0.y, 5);
        player.transform.position = playerStartPos;
    }

    void GenerateFirst(Vector3 vertices0, Vector3 vertices1)
    {
        int checkWind = Random.Range(0, 2);
        int checkNoWind = Random.Range(0, 2);
        if (checkNoWind == 0)
        {

            if (checkWind == 0)
                GameManager.Instance.windForce = Random.Range(GameManager.Instance.minWindForce, GameManager.Instance.maxWindForce);
            else
                GameManager.Instance.windForce = -Random.Range(GameManager.Instance.minWindForce, GameManager.Instance.maxWindForce);

        }
        else
        {
            GameManager.Instance.windForce = 0;
        }
        startHolePoint = Random.Range(GameManager.Instance.levelComplicated / 2 + 1, GameManager.Instance.levelComplicated);
        endHolePoint = startHolePoint + GameManager.Instance.complicatedOfHole * 2;
        endPoint = GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2;

        vertices[0] = vertices0;
        vertices[1] = vertices1;

        for (int i = 2; i < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 - 1; i++)
        {
            if (i <= startHolePoint)
                vertices[i] = vertices[0] + new Vector3(GameManager.Instance.width / (GameManager.Instance.levelComplicated - 1) * (i - 1), Random.Range(GameManager.Instance.minSlope, GameManager.Instance.maxSlope), 0);
            else if (i >= (GameManager.Instance.complicatedOfHole * 2 + startHolePoint + 1))
            {
                vertices[i] = vertices[0] + new Vector3(GameManager.Instance.width / (GameManager.Instance.levelComplicated - 1) * (i - GameManager.Instance.complicatedOfHole * 2 - 1), Random.Range(GameManager.Instance.minSlope, GameManager.Instance.maxSlope), 0);
            }
            else
            {
                vertices[i] = vertices[startHolePoint];
            }

        }

        middlePoint = startHolePoint + GameManager.Instance.complicatedOfHole;
        vertices[middlePoint] = vertices[startHolePoint] + new Vector3(GameManager.Instance.holeWidth / 2, -GameManager.Instance.holeHeight);

        float complex = GameManager.Instance.holeWidth / (GameManager.Instance.complicatedOfHole * 2);

        vertices[startHolePoint + 1] = vertices[startHolePoint] + new Vector3(0, -GameManager.Instance.holeHeight / 2, 0);
        for (int i = startHolePoint + 2; i < middlePoint; i++)
        {
            vertices[i] = vertices[middlePoint] + new Vector3(-(complex * (middlePoint - i + 1)), ratio * (complex * (middlePoint - i + 1)) * (complex * (middlePoint - i + 1)), 0);
        }

        for (int i = middlePoint + 1; i < (GameManager.Instance.complicatedOfHole * 2 + startHolePoint + 1); i++)
        {
            vertices[i] = vertices[middlePoint] + new Vector3(-(complex * (middlePoint - i - 1)), ratio * (complex * (middlePoint - i - 1)) * (complex * (middlePoint - i - 1)), 0);
        }

        vertices[endHolePoint] = vertices[startHolePoint] + new Vector3(GameManager.Instance.holeWidth, 0, 0);

        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 - 1] = vertices0 + new Vector3(GameManager.Instance.width, 0, 0);
        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1] = vertices0 + new Vector3(GameManager.Instance.width, -GameManager.Instance.len, 0);
        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 1] = vertices0 + new Vector3(0, -GameManager.Instance.len, 0);
        Vector2[] vertices2 = new Vector2[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
        for (int j = 0; j < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2; j++)
            vertices2[j] = new Vector2(vertices[j].x, vertices[j].y);

        int[] tri = new int[(GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1) * 3];

        for (int y = 0; y < (GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1) * 3; y++)
        {
            int c = y % 3;
            int b = y / 3;
            if (c != 2)
                tri[y] = c + b;
            else
                tri[y] = GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.RecalculateBounds();
        pCollider.points = vertices2;

        Vector3 x = transform.TransformPoint(vertices[middlePoint]);
        GameManager.Instance.holeCheckPoint.transform.position = new Vector3(x.x, x.y - GameManager.Instance.checkPointBelow, 5);

        if (GameManager.Instance.spriteMaterial)
        {
            gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.spriteMaterial;
        }

    }
    public void Generate(Vector3 vertices0, Vector3 vertices1)
    {
        int checkWind = Random.Range(0, 2);
        int checkNoWind = Random.Range(0, 2);
        if (checkNoWind == 0)
        {
            if (checkWind == 0)
                GameManager.Instance.windForce = Random.Range(GameManager.Instance.minWindForce, GameManager.Instance.maxWindForce);
            else
                GameManager.Instance.windForce = -Random.Range(GameManager.Instance.minWindForce, GameManager.Instance.maxWindForce);
        }
        else
        {
            GameManager.Instance.windForce = 0;
        }

        startHolePoint = Random.Range(GameManager.Instance.levelComplicated / 2 + 1, GameManager.Instance.levelComplicated);
        endHolePoint = startHolePoint + GameManager.Instance.complicatedOfHole * 2;
        endPoint = GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2;

        vertices[0] = vertices0;
        vertices[1] = vertices1;

        for (int i = 2; i < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2; i++)
        {
            if (i <= startHolePoint)
            {

                vertices[i] = vertices[0] + new Vector3(GameManager.Instance.width / (GameManager.Instance.levelComplicated - 1) * (i - 1), Random.Range(GameManager.Instance.minSlope, GameManager.Instance.maxSlope), 0);
                if (i <= (g1.endPoint - g1.endHolePoint + 1))
                {
                    vertices[i] = new Vector3(vertices[i].x, g1.vertices[g1.endHolePoint + i - 1].y, 0);
                }
            }
            else if (i >= (GameManager.Instance.complicatedOfHole * 2 + startHolePoint + 1))
            {
                vertices[i] = vertices[0] + new Vector3(GameManager.Instance.width / (GameManager.Instance.levelComplicated - 1) * (i - GameManager.Instance.complicatedOfHole * 2 - 1), Random.Range(GameManager.Instance.minSlope, GameManager.Instance.maxSlope), 0);
            }
            else
            {
                vertices[i] = vertices[startHolePoint];
            }
        }

        middlePoint = startHolePoint + GameManager.Instance.complicatedOfHole;
        vertices[middlePoint] = vertices[startHolePoint] + new Vector3(GameManager.Instance.holeWidth / 2, -GameManager.Instance.holeHeight);

        float complex = GameManager.Instance.holeWidth / (GameManager.Instance.complicatedOfHole * 2);

        vertices[startHolePoint + 1] = vertices[startHolePoint] + new Vector3(0, -GameManager.Instance.holeHeight / 2, 0);
        for (int i = startHolePoint + 2; i < middlePoint; i++)
        {
            vertices[i] = vertices[middlePoint] + new Vector3(-(complex * (middlePoint - i + 1)), ratio * (complex * (middlePoint - i + 1)) * (complex * (middlePoint - i + 1)), 0);
        }

        for (int i = middlePoint + 1; i < (GameManager.Instance.complicatedOfHole * 2 + startHolePoint + 1); i++)
        {
            vertices[i] = vertices[middlePoint] + new Vector3(-(complex * (middlePoint - i - 1)), ratio * (complex * (middlePoint - i - 1)) * (complex * (middlePoint - i - 1)), 0);
        }
        vertices[endHolePoint] = vertices[startHolePoint] + new Vector3(GameManager.Instance.holeWidth, 0, 0);

        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 - 1] = vertices0 + new Vector3(GameManager.Instance.width, 0, 0);
        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1] = vertices0 + new Vector3(GameManager.Instance.width, -GameManager.Instance.len, 0);
        vertices[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 1] = vertices0 + new Vector3(0, -GameManager.Instance.len, 0);
        Vector2[] vertices2 = new Vector2[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
        for (int j = 0; j < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2; j++)
            vertices2[j] = new Vector2(vertices[j].x, vertices[j].y);

        int[] tri = new int[(GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1) * 3];

        for (int y = 0; y < (GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1) * 3; y++)
        {
            int c = y % 3;
            int b = y / 3;
            if (c != 2)
                tri[y] = c + b;
            else
                tri[y] = GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.RecalculateBounds();
        pCollider.points = vertices2;

        Vector3 x = transform.TransformPoint(vertices[middlePoint]);
        GameManager.Instance.holeCheckPoint.transform.position = new Vector3(x.x, x.y - GameManager.Instance.checkPointBelow, 5);
        GetPlayerPosition(vertices0, vertices1);

        if (GameManager.Instance.spriteMaterial)
        {
            gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.spriteMaterial;
        }
    }


    void ChangeVertex()
    {
        for (int i = startHolePoint; i <= (startHolePoint + GameManager.Instance.complicatedOfHole * 2); i++)
        {
            vertices[i] = new Vector3(vertices[i].x, vertices[startHolePoint].y, 0);

        }
        mesh.vertices = vertices;
    }

}
