using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDetector : MonoBehaviour
{
    public CheckPoint TheCheckPoint;

    void Awake()
    {
        TheCheckPoint = FindObjectOfType<CheckPoint>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            print("checkpoint detected");
            //other.gameObject.transform
            TheCheckPoint.selected = true;
        }
        if (other.gameObject.tag == "Player")
        {
            return;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            TheCheckPoint.selected = false;
        }
    }
}