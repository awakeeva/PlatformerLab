using System.Collections.Generic;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class ModularTrapItemAI : MonoBehaviour
    {
        [SerializeField] private ColliderCheck _vision;
        public bool VisionIsTouchingLayer => _vision.IsTouchingLayer;

        [Header("Range")]
        [SerializeField] private SpawnComponent _rangeAttack;

        private static readonly int RangeKey = Animator.StringToHash("range");
        private static readonly int HitKey = Animator.StringToHash("hit");

        private Animator _animator;

        private GameObject _parentGO;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _parentGO = this.gameObject.transform.parent.gameObject;
        }

        public void OnHitModule()
        {
            List<GameObject> listOfChildren = new List<GameObject>();
            listOfChildren = _parentGO.getChildren();

            foreach (GameObject child in listOfChildren)
            {
                var animator = child.GetComponent<Animator>();
                if (null == animator)
                    continue;

                animator.SetTrigger(HitKey);
            }
        }

        public void RangeAttack()
        {
            if (_animator != null)
                _animator.SetTrigger(RangeKey);
        }

        private void OnRangeAttack()
        {
            _rangeAttack.Spawn();
        }
    }
}

