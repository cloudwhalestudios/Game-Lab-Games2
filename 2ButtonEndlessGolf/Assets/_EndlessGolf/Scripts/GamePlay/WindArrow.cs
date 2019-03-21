using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArrow : MonoBehaviour
{
    [Header("Reference Object")]
    public GameObject body;
    public GameObject head;

    protected Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        /*body.transform.localScale = new Vector3(GameManager.Instance.bodyXscale * Mathf.Abs(GameManager.Instance.windForce), 1, 1);
        if (GameManager.Instance.windForce != 0)
        {
            head.transform.localScale = Vector3.one;
            if (GameManager.Instance.windForce > 0)
            {
                anim.Play("WindLeft");
            }
            else
            {
                anim.Play("WindRight");
            }
        }
        else
        {
            head.transform.localScale = Vector3.zero;
        }*/
    }
}
