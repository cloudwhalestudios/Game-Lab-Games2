using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public abstract class BaseMenuController : MonoBehaviour
        {
            [Serializable]
            public class ButtonAnimationSpot
            {
                public Vector3 localPosition;
                [ReadOnly] public GameObject buttonObject;
            }

            public enum Transition
            {
                Move,
                Animate
            }

            public GameObject menuContainer;
            public GameObject buttonParent;

            [Space]
            public RectTransform itemSelectIndicator;
            public RectTransform itemSelectTimer;

            [Header("Selection Behaviour")]
            public int startingIndex;
            public Transition transitionType;
            public bool updateStartIndex = false;

            [Header("Move Offset Tweaking")]
            public Vector2 itemIndicatorOffset;

            [Header("Animation Config")]
            public float transitionTime;

            [Space]
            public AnimationClip appearClip;
            public AnimationClip growClip;
            public AnimationClip shrinkClip;
            public AnimationClip disappearClip;

            public AnimationClip staticFadedClip;
            public AnimationClip staticSmallClip;
            public AnimationClip staticLargeClip;
            
            [Space]
            public ButtonAnimationSpot firstSpot;
            public ButtonAnimationSpot currentSpot;
            public ButtonAnimationSpot lastSpot;
        }
    }
}