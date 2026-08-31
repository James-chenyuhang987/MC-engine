using UnityEngine;

namespace MCEngine.Player
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float smoothing = 10f;
        private Transform target;

        public void Initialize(Transform followTarget)
        {
            target = followTarget;
            SnapToTarget();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var targetPosition = new Vector3(target.position.x, target.position.y + 1f, -10f);
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                1f - Mathf.Exp(-smoothing * Time.deltaTime));
        }

        private void SnapToTarget()
        {
            transform.position = new Vector3(target.position.x, target.position.y + 1f, -10f);
        }
    }
}

