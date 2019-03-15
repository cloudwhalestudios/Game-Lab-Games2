using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public static GroundManager Instance { get; private set; }

    [Header("Reference Object")]
    [SerializeField]
    private GameObject groundHolder1;

    [SerializeField]
    private GameObject groundHolder2;

    // Use this for initialization
    [HideInInspector]
    public int currentGround;

    private PolygonCollider2D holderCollider1;
    private PolygonCollider2D holderCollider2;

    void Start()
    {
        currentGround = 2;
        StartCoroutine(GetPolygonCollder_IEmun());
    }
    
    IEnumerator GetPolygonCollder_IEmun()
    {
        yield return new WaitForSeconds(0.1f);
        if (groundHolder1.GetComponent<PolygonCollider2D>() != null)
            holderCollider1 = groundHolder1.GetComponent<PolygonCollider2D>();
        else
            Debug.LogWarning("missing collider1");

        if (groundHolder2.GetComponent<PolygonCollider2D>() != null)
            holderCollider2 = groundHolder2.GetComponent<PolygonCollider2D>();
        else
            Debug.LogWarning("missing collider2");
    } 

    public float GetCenterY(int groundIndex)
    {
        if (groundIndex > 2 || groundIndex <= 0)
        {
            return -1;
        }
        else
        {
            if (groundIndex == 1)
                return (FindYMax(holderCollider1.points) + FindYMin(holderCollider1.points)) / 2;
            else
                return (FindYMax(holderCollider2.points) + FindYMin(holderCollider2.points)) / 2;
        }
    }

    float FindYMax(Vector2[] vector2Array)
    {
        float max = vector2Array[0].y;
        for (int i = 0; i< vector2Array.Length - 2; i++)
        {
            if (max < vector2Array[i].y)
                max = vector2Array[i].y;
        }
        return max;
    }

    float FindYMin(Vector2[] vector2Array)
    {
        float min = vector2Array[0].y;
        for (int i = 0; i < vector2Array.Length - 2; i++)
        {
            if (min > vector2Array[i].y)
                min = vector2Array[i].y;
        }
        return min;
    }
}
