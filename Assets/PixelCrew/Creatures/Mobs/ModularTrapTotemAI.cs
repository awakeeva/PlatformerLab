using PixelCrew.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class ModularTrapTotemAI : MonoBehaviour
    {
        [Header("Range")]
        [SerializeField] private Cooldown _rangeCooldown;

        private GameObject _parentGO;

        private int _lastAttackModuleIndx;

        private void Awake()
        {
            _parentGO = this.gameObject;
        }

        private void Update()
        {
            if (AnyChildVisionIsTouchingLayer())
            {
                List<GameObject> listOfChildren = new List<GameObject>();
                listOfChildren = _parentGO.getChildren();
                var sortedListOfChildren = listOfChildren.OrderByDescending(go => go.transform.position.y).ToList();

                if (sortedListOfChildren.Count < _lastAttackModuleIndx + 1)
                {
                    _lastAttackModuleIndx = 0;
                }

                var itemAIComponent = sortedListOfChildren[_lastAttackModuleIndx].GetComponent<ModularTrapItemAI>();
                if (itemAIComponent != null && _rangeCooldown.IsReady)
                {
                    itemAIComponent.RangeAttack();
                    _rangeCooldown.Reset();
                    _lastAttackModuleIndx++;
                }

            }
        }

        private bool AnyChildVisionIsTouchingLayer()
        {
            List<GameObject> listOfChildren = new List<GameObject>();
            listOfChildren = _parentGO.getChildren();

            if (listOfChildren.Count <= 0)
                return false;

            foreach (GameObject child in listOfChildren)
            {
                var itemAIComponent = child.GetComponent<ModularTrapItemAI>();
                if (null == itemAIComponent)
                    continue;

                if (itemAIComponent.VisionIsTouchingLayer)
                    return true;
            }

            return false;
        }
    }
}
