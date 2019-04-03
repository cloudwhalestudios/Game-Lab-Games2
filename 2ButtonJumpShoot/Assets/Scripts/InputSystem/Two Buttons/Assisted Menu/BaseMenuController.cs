using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public abstract class BaseMenuController : MonoBehaviour
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

            [Header("Selection Behaviour")]
            public int startingIndex;
            public int buttonsPerRow;
            public int buttonsPerColumn;

            [Header("Offset Tweaking")]
            public Vector2 itemIndicatorOffset;
            public Vector2 columnIndicatorOffset;
            public Vector2 rowIndicatorOffset;
        }
    }
}