using UnityEngine;
using UnityEngine.InputSystem;

namespace MCEngine.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 7f;
        [SerializeField] private float jumpSpeed = 10f;

        private BoxCollider2D bodyCollider;
        private Rigidbody2D body;
        private float movement;
        private bool jumpRequested;

        public BoxCollider2D BodyCollider => bodyCollider;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                movement = 0f;
                return;
            }

            movement = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                movement -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                movement += 1f;
            }

            jumpRequested |= keyboard.spaceKey.wasPressedThisFrame;
        }

        private void FixedUpdate()
        {
            body.linearVelocity = new Vector2(movement * movementSpeed, body.linearVelocity.y);

            if (jumpRequested && IsGrounded())
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
            }

            jumpRequested = false;
        }

        private bool IsGrounded()
        {
            var bounds = bodyCollider.bounds;
            var checkCenter = new Vector2(bounds.center.x, bounds.min.y - 0.06f);
            var hit = Physics2D.OverlapBox(checkCenter, new Vector2(bounds.size.x * 0.8f, 0.08f), 0f);
            return hit != null && hit != bodyCollider;
        }
    }
}

