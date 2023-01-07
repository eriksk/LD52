using UnityEngine;

namespace Scripts.Behaviours
{
    [RequireComponent(typeof(Health))]
    public class SpawnPrefabOnDamaged : MonoBehaviour
    {
        public GameObject Prefab;
        private Health _health;

        void Start()
        {
            _health = GetComponent<Health>();
            _health.OnDamaged += OnDamaged;
        }

        private void OnDamaged(float damage, Vector3 position, Vector3 force)
        {
            Instantiate(Prefab, position, Quaternion.identity);
        }
    }
}