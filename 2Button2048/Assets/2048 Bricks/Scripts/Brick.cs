using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Brick : MonoBehaviour
{
    [SerializeField]
    Text label;
    [SerializeField]
    Image sprite;

    [SerializeField]
    Color[] labelColors;
    [SerializeField]
    Color[] spriteColors;

    [SerializeField]
    float moveDuration;

    int number;

    public int Number
    {
        get { return number; }
        set
        {
            number = value;
            label.text = number.ToString();

            int colorIndex = (int) Mathf.Log(number, 2) - 1;
            label.color = labelColors[Mathf.Clamp(colorIndex, 0, labelColors.Length - 1)];
            sprite.color = spriteColors[Mathf.Clamp(colorIndex, 0, spriteColors.Length - 1)];
        }
    }

    public void DoLocalMove(Vector2 position, Action onComplete)
    {
        StartCoroutine(LocalMove(position, onComplete));
    }

    public void DoLandingAnimation(Action onComplete)
    {
        GetComponent<Animator>().SetTrigger("Landing");
        StartCoroutine(DelayedCall(onComplete, 0.25f));
    }

    public void DoMergingAnimation(Action onComplete)
    {
        GetComponent<Animator>().SetTrigger("Merging");
        StartCoroutine(DelayedCall(onComplete, 0.25f));
    }

    IEnumerator LocalMove(Vector2 position, Action onComplete)
    {
        Vector2 startPosition = GetComponent<RectTransform>().anchoredPosition;
        float t = Time.deltaTime;
        while (t < moveDuration)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, position, t / moveDuration);
            yield return null;
            t += Time.deltaTime;
        }

        GetComponent<RectTransform>().anchoredPosition = position;

        if (onComplete != null)
            onComplete.Invoke();
    }

    IEnumerator DelayedCall(Action onComplete, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (onComplete != null)
            onComplete.Invoke();
    }
}