using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGen3 : MonoBehaviour
{
    Vector3[] vertices;
    PlaneGen1 g1;
    PlaneGen2 g2;
    GroundManager gManager;
    Mesh mesh;
    PolygonCollider2D pCollider;

    void Start()
    {
        vertices = new Vector3[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
        g1 = GameObject.FindGameObjectWithTag("Ground1").GetComponent<PlaneGen1>();
        g2 = GameObject.FindGameObjectWithTag("Ground2").GetComponent<PlaneGen2>();
        gManager = GameObject.FindGameObjectWithTag("GroundGenerator").GetComponent<GroundManager>();

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        mesh = GetComponent<MeshFilter>().mesh;
        pCollider = gameObject.AddComponent<PolygonCollider2D>();
    }
    
    public void Generate()
    {
        if (gManager.currentGround == 1)
        {
            transform.position = new Vector3(g2.gameObject.transform.position.x, g2.gameObject.transform.position.y, 10);
            vertices = g2.vertices;

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

            Vector2[] vertices2 = new Vector2[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
            for (int j = 0; j < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2; j++)
                vertices2[j] = new Vector2(vertices[j].x, vertices[j].y);

            mesh.vertices = vertices;
            mesh.triangles = tri;

            pCollider.points = vertices2;

            if (GameManager.Instance.spriteMaterial)
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.spriteMaterial;
            }

            mesh.RecalculateBounds();
        }
        else
        {
            transform.position = new Vector3(g1.gameObject.transform.position.x, g1.gameObject.transform.position.y, 10);
            vertices = g1.vertices;

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

            Vector2[] vertices2 = new Vector2[GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2];
            for (int j = 0; j < GameManager.Instance.levelComplicated + GameManager.Instance.complicatedOfHole * 2 + 1 + 2; j++)
                vertices2[j] = new Vector2(vertices[j].x, vertices[j].y);

            mesh.vertices = vertices;
            mesh.triangles = tri;

            pCollider.points = vertices2;

            if (GameManager.Instance.spriteMaterial)
            {
                gameObject.GetComponent<MeshRenderer>().material = GameManager.Instance.spriteMaterial;
            }
            mesh.RecalculateBounds();
        }
    }
}
