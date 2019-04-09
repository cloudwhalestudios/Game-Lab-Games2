using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WebGLIntegration
{
    public class WebGLParameters : MonoBehaviour
    {
        public static string GetParameterJson()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return JSLib.GetParams();
#endif
            return "";
        }
    }
}