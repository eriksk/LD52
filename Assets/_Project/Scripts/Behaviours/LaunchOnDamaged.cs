using UnityEngine;

namespace Scripts.Behaviours
{
    [RequireComponent(typeof(Scripts.Characters.CharacterController))]
    [RequireComponent(typeof(Health))]
    public class LaunchOnDamaged : MonoBehaviour
    {
        public float Multiplier = 1f;

        private Health _health;
        private Scripts.Characters.CharacterController _character;

        void Start()
        {
            _health = GetComponent<Health>();
            _character = GetComponent<Scripts.Characters.CharacterController>();
            _health.OnDamaged += OnDamaged;
        }

        private void OnDamaged(float damage, Vector3 position, Vector3 force)
        {
            var updog = Vector3.up * force.magnitude * 0.5f;
            _character.Launch((force + updog) * Multiplier);
        }
    }
}