using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool selected;
    public SpriteRenderer selectedVisual;


    void Start()
    {
        selectedVisual = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (selected)
        {
            selectedVisual.enabled = true;
        } else
        {
            selectedVisual.enabled = false;
        }
    }
}