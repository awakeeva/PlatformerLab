using Cinemachine;
using UnityEngine;

namespace PixelCrew.Components
{
    public class CameraStateController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CinemachineVirtualCamera _camera;

        private static readonly int ShowTargetKey = Animator.StringToHash("ShowTarget");

        public void SetPosition(Vector3 pos)
        {
            pos.z = _camera.transform.position.z;
            _camera.transform.position = pos;
        }

        public void SetState(bool state)
        {
            _animator.SetBool(ShowTargetKey, state);
        }
    }
}

