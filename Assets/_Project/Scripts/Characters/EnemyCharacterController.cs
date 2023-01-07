using UnityEngine;

namespace Scripts.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyCharacterController : MonoBehaviour
    {
        public float MovementSpeed = 10f;
        public float MovementDamping = 0.2f;
        public float JumpForce = 15f;
        public float ExtraGravity = 15f;
        public float RotationSpeed = 5f;

        public SphereCollider GroundChecker;
        public Collider BodyCollider;
        public LayerMask GroundCheckLayer;
        private Rigidbody _rigidbody;

        public Vector3 InputMovement;
        public bool InputJump;

        private bool _grounded;
        private float _jumpCooldown;
        private float _timeSinceGrounded;
        public float _forwardMovementSpeed;

        private const float JumpCoolDownTime = 0.3f;

        public Rigidbody Rigidbody => _rigidbody;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            ResetController();
        }

        public void ResetController()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            InputMovement = Vector3.zero;
            InputJump = false;
            _grounded = false;
            _jumpCooldown = 0f;
            _timeSinceGrounded = 0f;
            _forwardMovementSpeed = 0f;
        }

        public void Launch(Vector3 force)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public bool IsMoving => _rigidbody.velocity.magnitude > 0.1f;

        void Update()
        {
            _grounded = CheckGround();
            _timeSinceGrounded += Time.deltaTime;

            if (_grounded) _timeSinceGrounded = 0f;

            if (InputMovement.magnitude > 0.01f)
            {
                var magnitude = Mathf.Clamp01(InputMovement.magnitude);
                InputMovement = InputMovement.normalized * magnitude;
                InputMovement.y = 0f;
            }
            else
            {
                InputMovement = Vector3.zero;
            }

            _jumpCooldown -= Time.deltaTime;

            if (InputJump)
            {
                Jump();
            }

            var moving = InputMovement.magnitude > 0.001f;

            if (moving)
            {
                var targetRotation = Quaternion.LookRotation(InputMovement.normalized, Vector3.up);
                _rigidbody.MoveRotation(
                    Quaternion.Slerp(
                        _rigidbody.rotation,
                        targetRotation,
                        RotationSpeed * Time.deltaTime
                    )
                );
            }
        }

        private bool CheckGround()
        {
            if (Physics.CheckSphere(GroundChecker.transform.position, GroundChecker.radius, GroundCheckLayer, QueryTriggerInteraction.Ignore))
            {
                return true;
            }
            return false;
        }

        void Jump()
        {
            if (!_grounded && _timeSinceGrounded >= 0.2f)
            {
                return;
            }

            if (_jumpCooldown > 0f)
            {
                return;
            }
            var v = _rigidbody.velocity;
            v.y = 0f;
            _rigidbody.velocity = v;
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            _jumpCooldown = JumpCoolDownTime;
        }

        void FixedUpdate()
        {
            if (!_grounded)
            {
                _rigidbody.AddForce(Vector3.down * _rigidbody.mass * ExtraGravity);
            }

            var velocity = _rigidbody.velocity;
            velocity.y = 0f;
            var damping = MovementDamping;

            if (!_grounded)
            {
                damping *= 0.2f;
            }

            _rigidbody.AddForce(-velocity * _rigidbody.mass * damping);

            if (InputMovement.magnitude > 0.01f)
            {
                var speed = MovementSpeed;
                if (!_grounded)
                {
                    speed *= 0.2f;
                }
                _rigidbody.AddForce(InputMovement * _rigidbody.mass * speed);
            }
            else if (_rigidbody.velocity.magnitude < 1f)
            {
                if (_grounded)
                {
                    _rigidbody.velocity = Vector3.zero;
                }
            }
        }
    }
}