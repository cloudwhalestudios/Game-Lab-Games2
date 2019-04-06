using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace WebGLIntegration
{
    public class WebGLRedirect : MonoBehaviour
    {
        private static void Redirect(string url)
        {
            JSLib.Redirect(Config.BASE_URL + url);
        }

        public void OpenPlatform()
        {
            Redirect(Config.PLATFORM_URL);
        }
    }
}