using UnityEngine;

namespace PixelCrew
{
    public class MovablePlatform : MonoBehaviour
    {
        [SerializeField] private float _speedX;
        [SerializeField] private float _speedY;

        [SerializeField, Range(0, 15)] private float _xOffsetRight;
        [SerializeField, Range(-15, 0)] private float _xOffsetLeft;

        [SerializeField, Range(0, 15)] private float _yOffsetUp;
        [SerializeField, Range(-15, 0)] private float _yOffsetDown;

        private const float DirectionForward = 1.0f;
        private const float DirectionBack = -1.0f;

        private float _directionX = DirectionForward;
        private float _directionY = DirectionForward;

        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        private void Awake()
        {
            _minX = transform.position.x + _xOffsetLeft;
            _maxX = transform.position.x + _xOffsetRight;

            _minY = transform.position.y + _yOffsetDown;
            _maxY = transform.position.y + _yOffsetUp;
        }

        private void Update()
        {
            var deltaXPosition = _directionX * _speedX * Time.deltaTime;
            var newXPosition = transform.position.x + deltaXPosition;

            var deltaYPosition = _directionY * _speedY * Time.deltaTime;
            var newYPosition = transform.position.y + deltaYPosition;

            transform.position = new Vector3(newXPosition, newYPosition, transform.position.z);
            
            _directionX = TryChangeDirection(_directionX, _minX, _maxX, transform.position.x);

            _directionY = TryChangeDirection(_directionY, _minY, _maxY, transform.position.y);

        }

        private float TryChangeDirection(float direction, float minPos, float maxPos, float currentPos)
        {
            if (currentPos >= maxPos)
            {
                return DirectionBack;
            }
            else if (currentPos <= minPos)
            {
                return DirectionForward;
            }

            return direction;
        }
    }
}
