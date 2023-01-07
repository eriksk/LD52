using UnityEngine;

namespace Scripts.Behaviours
{
    [RequireComponent(typeof(Health))]
    public class DestroyOnDeath : MonoBehaviour
    {
        private Health _health;

        void Start()
        {
            _health = GetComponent<Health>();
            _health.OnKilled += OnKilled;
        }

        private void OnKilled(float damage, Vector3 position, Vector3 force)
        {
            _health.OnKilled -= OnKilled;
            Destroy(gameObject);
        }
    }
}