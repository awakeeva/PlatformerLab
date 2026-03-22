using System.Linq;
using UnityEngine;

namespace PixelCrew.Utils
{
    public static class WindowUtils
    {
        public static void CreateWindow(string resourcePath)
        {
            var window = Resources.Load<GameObject>(resourcePath);

            //var canvas = Object.FindObjectOfType<Canvas>();
            var canvases = Object.FindObjectsOfType<Canvas>();
            var canvas = canvases.FirstOrDefault(x => x.renderMode == RenderMode.ScreenSpaceOverlay);

            Object.Instantiate(window, canvas.transform);
        }
    }
}

