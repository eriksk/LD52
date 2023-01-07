using UnityEngine;

namespace Scripts.Behaviours
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float Time = 1f;

        void Start()
        {
            Destroy(gameObject, Time);
        }
    }
}