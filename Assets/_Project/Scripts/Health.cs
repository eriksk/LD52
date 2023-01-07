using UnityEngine;

namespace Scripts
{
    public delegate void Damaged(float damage, Vector3 position, Vector3 force);
    public delegate void Killed(float damage, Vector3 position, Vector3 force);

    public class Health : MonoBehaviour
    {
        public float Initial = 100f;

        public event Damaged OnDamaged;
        public event Killed OnKilled;

        private float _health;

        public bool Alive => _health > 0f;
        public bool Dead => !Alive;

        void Start()
        {
            _health = Initial;
        }

        public void Deal(float damage, Vector3 position, Vector3 force)
        {
            if (Dead) return;

            _health -= damage;
            if (_health <= 0f)
            {
                // Killed
                _health = 0f;
                OnKilled?.Invoke(damage, position, force);
                return;
            }

            OnDamaged?.Invoke(damage, position, force);
        }
    }
}