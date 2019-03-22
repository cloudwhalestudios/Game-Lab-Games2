using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuController : MonoBehaviour
{
    public enum IndicatorMode
    {
        Single,
        RowAndSingle,
        ColumnAndSingle,
        RowAndColumn
    }

    public GameObject menuContainer;
    public GameObject buttonParent;

    public IndicatorMode indicatorMode = IndicatorMode.Single;
    public RectTransform rowSelectIndicator;
    public RectTransform columnSelectIndicator;
    public RectTransform itemSelectIndicator;
    public int startingIndex;
    public Vector2 offset;
}
