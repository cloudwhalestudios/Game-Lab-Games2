using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlaceholder : MonoBehaviour
{
    public GameObject fx_Land;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0, 3, 0);

    Rigidbody2D rb;
    TrailRenderer trailRenderer;
    BoxCollider2D bc2D;

    bool hasLanded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc2D = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        //playerParentTransform = BasePlayerManager.Instance.playerParent;
        InitPlaceholder();
    }

    public void InitPlaceholder()
    {
        trailRenderer.startWidth = transform.localScale.x;
        trailRenderer.endWidth = transform.localScale.x;

        rb.velocity = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;

        transform.position = spawnPosition;

        hasLanded = false;

        rb.isKinematic = false;
        bc2D.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isActiveAndEnabled && !hasLanded && other.gameObject.tag == "Step" && rb.velocity == Vector2.zero)
        {
            hasLanded = true;
            AudioManager.Instance.PlaySound(AudioManager.Instance.BassCrash);
            Destroy(Instantiate(fx_Land, transform.position, Quaternion.identity), 0.5f);

            rb.velocity = new Vector2(0, 0);
            transform.rotation = Quaternion.identity;

            transform.SetParent(other.gameObject.transform);

            other.gameObject.GetComponent<Step>().StartCoroutine_LandingEffect();
        }
    }
}
