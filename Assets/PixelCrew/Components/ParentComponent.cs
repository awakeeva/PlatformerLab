using UnityEngine;

namespace PixelCrew.Components
{
    public class ParentComponent : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;

        public void SetParent(GameObject target)
        {
            target.transform.SetParent(_parent.transform);
        }

        public void ResetParent(GameObject target)
        {
            target.transform.SetParent(null);
        }
    }
}

