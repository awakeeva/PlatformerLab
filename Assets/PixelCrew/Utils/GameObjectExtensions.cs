using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Utils
{
    public static class GameObjectExtensions
    {
        public static bool IsInLayer(this GameObject go, LayerMask layer)
        {
            return layer == (layer | 1 << go.layer);
        }

        public static List<GameObject> getChildren(this GameObject go)
        {
            List<GameObject> listOfChildren = new List<GameObject>();

            foreach (Transform child in go.transform)
            {
                if (null == child)
                    continue;
                listOfChildren.Add(child.gameObject);
            }

            return listOfChildren;
        }
    }
}
