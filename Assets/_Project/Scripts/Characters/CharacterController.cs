using System;
using System.Collections;
using UnityEngine;

namespace Scripts.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterController : MonoBehaviour
    {
        public float MouseSensitivity = 1f;
        public float Height = 1.8f;
        public float MovementSpeed = 10f;
        public float MovementDamping = 0.2f;
        public float JumpForce = 15f;
        public float ExtraGravity = 15f;

        public SphereCollider GroundChecker;
        public Collider BodyCollider, HeadCollider;

        public LayerMask GroundCheckLayer;

        public Transform CameraAnchor;
        public Transform GunModel;
        public Transform GunPosition;
        public Transform GunAdsPosition;
        public Weapon Weapon;

        private Rigidbody _rigidbody;
        private Camera _camera;

        private float _yaw, _pitch;
        private Vector3 _movement;
        private bool _jump;
        private bool _sprint;
        private bool _grounded;
        private bool _crouching;
        private float _jumpCooldown;
        private float _timeSinceGrounded;
        public float _forwardMovementSpeed;

        private const float JumpCoolDownTime = 0.3f;

        public Vector3 CharacterForward => Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;
        public Vector3 CharacterLeft => Quaternion.Euler(0f, _yaw - 90f, 0f) * Vector3.forward;
        public Vector3 CharacterRight => Quaternion.Euler(0f, _yaw + 90f, 0f) * Vector3.forward;
        public Quaternion CharacterForwardRotation => Quaternion.Euler(0f, _yaw, 0f);

        public Vector3 EyePosition => _camera.transform.position;
        public Quaternion EyeRotation => _camera.transform.rotation;
        public Vector3 EyeForward => _camera.transform.forward;

        public Rigidbody Rigidbody => _rigidbody;

        private Vector3 _gunTargetPosition;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _rigidbody = GetComponent<Rigidbody>();
            _camera = Camera.main;
            _camera.transform.SetParent(CameraAnchor);
            _camera.transform.localPosition = Vector3.zero;
            _camera.transform.localRotation = Quaternion.identity;
            _camera.transform.localScale = Vector3.one;

            Weapon.OnFired += () =>
            {
                _gunTargetPosition += Vector3.forward * 0.2f; // ????
            };

            GunModel.SetParent(CameraAnchor);
            ResetController();
            StartCoroutine(WaitAndCenterMousePosition());
        }

        private IEnumerator WaitAndCenterMousePosition()
        {
            yield return new WaitForSeconds(0.2f);
            _yaw = 0f;
            _pitch = 0f;
        }

        public void ResetController()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _movement = Vector3.zero;
            _jump = false;
            _grounded = false;
            _jumpCooldown = 0f;
            _timeSinceGrounded = 0f;
            _yaw = 0;
            _pitch = 0;
            _sprint = false;
            _forwardMovementSpeed = 0f;
        }

        public void Launch(Vector3 force)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        private float _targetFov = 80f;
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                // Time.timeScale = .3f;
                _targetFov = 50f;
                _gunTargetPosition = GunAdsPosition.localPosition;
            }
            else
            {
                Time.timeScale = 1f;
                _targetFov = 80f;
                _gunTargetPosition = GunPosition.localPosition;
            }

            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFov, 25f * Time.unscaledDeltaTime);

            var mouseDelta = new Vector2(
                Input.GetAxis("Mouse X"),
                -Input.GetAxis("Mouse Y")
            );
            _movement = new Vector3(
                Input.GetAxis("Horizontal"),
                0f,
                Input.GetAxis("Vertical")
            );
            _jump = Input.GetButtonDown("Jump");
            _sprint = Input.GetKey(KeyCode.LeftShift);
            _crouching = Input.GetKey(KeyCode.LeftControl);
            _grounded = CheckGround();
            _timeSinceGrounded += Time.deltaTime;

            if (_grounded) _timeSinceGrounded = 0f;

            if (_movement.magnitude > 0.01f)
            {
                var magnitude = Mathf.Clamp01(_movement.magnitude);
                _movement = CharacterForwardRotation * _movement.normalized * magnitude;
                _movement.y = 0f;
            }
            else
            {
                _movement = Vector3.zero;
            }

            _yaw += mouseDelta.x * MouseSensitivity * Time.deltaTime;
            _pitch += mouseDelta.y * MouseSensitivity * Time.deltaTime;

            _pitch = Mathf.Clamp(_pitch, -90f, 90f);

            CameraAnchor.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            _rigidbody.MoveRotation(CharacterForwardRotation);

            var height = Height;
            if (_crouching)
            {
                height = Height * 0.5f;
                // HeadCollider.enabled = false;
            }
            else
            {
                // HeadCollider.enabled = true;
            }
            CameraAnchor.localPosition = Vector3.up * height;
            GunModel.transform.localPosition = Vector3.Lerp(GunModel.transform.localPosition, _gunTargetPosition, 15f * Time.deltaTime);

            _jumpCooldown -= Time.deltaTime;

            var velocity = _rigidbody.velocity;
            var forwardDot = Vector3.Dot(CharacterForward, velocity.normalized);

            _forwardMovementSpeed = _rigidbody.velocity.magnitude * forwardDot;

            if (_jump)
            {
                Jump();
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

            if (_movement.magnitude > 0.01f)
            {
                var speed = MovementSpeed;
                if (_grounded)
                {
                    if (_sprint)
                    {
                        speed *= 1.5f;
                    }
                }
                else
                {
                    speed *= 0.2f;
                }
                _rigidbody.AddForce(_movement * _rigidbody.mass * speed);
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