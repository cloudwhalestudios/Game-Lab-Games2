using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct BackgroundType
    {
        public string name;
        public Sprite backgroundImage;
        public Color backgroundColor;
        public Color groundColor;
        public Color effectFallingColor;
        public Color skyColor;
        public Color skyColor2;
        public Color ballColor;
        public Color trailColor;
    };
    [Header("Reference Objects")]
    public SpriteRenderer emptyBackground0;
    public SpriteRenderer emptyBackground1;
    public SpriteRenderer emptyBackground2;

    public SpriteRenderer bottomBackGround0;
    public SpriteRenderer bottomBackGround1;
    public SpriteRenderer bottomBackGround2;

    public Material ground;
    public Material skybox;

    public SpriteRenderer player;
    public TrailRenderer trailRender;
    public ParticleSystem fallingEffect;

    [Header("Config")]
    public BackgroundType[] backgr;

    [HideInInspector]
    public int rand;

    void Start()
    {
        rand = Random.Range(0, backgr.Length);
        emptyBackground0.sprite = backgr[rand].backgroundImage;
        emptyBackground1.sprite = backgr[rand].backgroundImage;
        emptyBackground2.sprite = backgr[rand].backgroundImage;
        bottomBackGround0.color = backgr[rand].backgroundColor;
        bottomBackGround1.color = backgr[rand].backgroundColor;
        bottomBackGround2.color = backgr[rand].backgroundColor;
        ground.SetColor("_Color", backgr[rand].groundColor);
        skybox.SetColor("_Color", backgr[rand].skyColor);
        skybox.SetColor("_Color2", backgr[rand].skyColor2);
        var main = fallingEffect.main;
        main.startColor = backgr[rand].effectFallingColor;
        player.color = backgr[rand].ballColor;
        trailRender.startColor = backgr[rand].trailColor;
        trailRender.endColor = backgr[rand].trailColor;
    }
}
