using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private float _direction;

        public void SetDirection(float direction)
        {
            _direction = direction;
        }

        void Update()
        {
            if (_direction != 0)
            {
                var delta = _direction * _speed * Time.deltaTime;
                var newXPosition = transform.position.x + delta;
                transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
            }
        }

        public void SaySomething()
        {
            Debug.Log("Something!");
        }
    }
}
