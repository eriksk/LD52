using System;
using UnityEngine;

namespace Scripts
{
    public class RayBullet : MonoBehaviour
    {
        public float MaxRange = 50f;
        public float Damage = 1f;
        public float ImpactForce = 10f;
        public LayerMask CollisionLayer;

        void Start()
        {
            if (Physics.Raycast(
                transform.position,
                transform.forward,
                out var hit,
                MaxRange,
                CollisionLayer,
                QueryTriggerInteraction.Ignore
            ))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                if (HandleHit(hit))
                {
                    // Hit something with health
                }
                else
                {
                    // Hit something without health
                }
            }

            // TODO: some trail or shit?
            Destroy(gameObject);
        }

        private bool HandleHit(RaycastHit hit)
        {
            var health = hit.collider.gameObject.GetComponent<Health>();
            if (health == null) return false;
            health.Deal(Damage, hit.point, transform.forward * ImpactForce);
            return true;
        }
    }
}