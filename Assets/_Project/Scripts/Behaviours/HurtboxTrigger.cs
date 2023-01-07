using System;
using UnityEngine;

namespace Scripts.Behaviours
{
    public class HurtboxTrigger : MonoBehaviour
    {
        public LayerMask HurtsMask;
        public float Damage = 1f;

        void OnTriggerEnter(Collider collider)
        {
            OnInsideHurtbox(collider.gameObject);
        }

        private void OnInsideHurtbox(GameObject gameObject)
        {
            if ((HurtsMask.value & (1 << gameObject.layer)) == 0)
                return;

            var health = gameObject.GetComponent<Health>() ?? gameObject.GetComponentInParent<Health>();
            health.Deal(Damage, transform.position, transform.forward);
        }
    }
}