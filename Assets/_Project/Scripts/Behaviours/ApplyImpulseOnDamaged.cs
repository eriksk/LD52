using UnityEngine;

namespace Scripts.Behaviours
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Health))]
    public class ApplyImpulseOnDamaged : MonoBehaviour
    {
        public float Multiplier = 1f;

        private Rigidbody _rigidbody;
        private Health _health;

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _health = GetComponent<Health>();

            _health.OnDamaged += OnDamaged;
        }

        private void OnDamaged(float damage, Vector3 position, Vector3 force)
        {
            var f = force * Multiplier * _rigidbody.mass;
            _rigidbody.AddForceAtPosition(
                f,
                position,
                ForceMode.Impulse);
        }
    }
}