using UnityEngine;

namespace Scripts.Behaviours
{
    [RequireComponent(typeof(Health))]
    public class SpawnPrefabOnDeath : MonoBehaviour
    {
        public GameObject Prefab;
        private Health _health;

        void Start()
        {
            _health = GetComponent<Health>();
            _health.OnKilled += OnKilled;
        }

        private void OnKilled(float damage, Vector3 position, Vector3 force)
        {
            _health.OnKilled -= OnKilled;
            Instantiate(Prefab, transform.position, Quaternion.identity);
        }
    }
}