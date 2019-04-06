using UnityEngine;
using System.Runtime.InteropServices;

namespace WebGLIntegration
{
    public static class JSLib
    {
        [DllImport("__Internal")] public static extern void Redirect(string location);
        [DllImport("__Internal")] public static extern void Refresh();
    }
}
