using UnityEngine;
using System.Runtime.InteropServices;

namespace WebGLIntegration
{
    public static class JSLib
    {
        [DllImport("__Internal")] public static extern void Redirect(string str_location);
        [DllImport("__Internal")] public static extern void RedirectWithParams(string str_location, string str_paramsJson);
        [DllImport("__Internal")] public static extern void Refresh();
        [DllImport("__Internal")] public static extern string GetParams();
        [DllImport("__Internal")] public static extern void SetParams(string str_paramsJson);
    }
}
