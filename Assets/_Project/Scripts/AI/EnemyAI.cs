using System;
using Scripts.Characters;
using UnityEngine;

namespace Scripts.AI
{
    [RequireComponent(typeof(EnemyCharacterController))]
    public class EnemyAI : MonoBehaviour
    {
        public Animation Animator;
        private EnemyCharacterController _controller;

        void Start()
        {
            _controller = GetComponent<EnemyCharacterController>();
        }

        void Update()
        {
            var player = GameObject.Find("Player");
            if (player == null)
            {
                Default();
                return;
            }

            // Basic follow
            var directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer.y = 0f;

            _controller.InputMovement = directionToPlayer;

            if (_controller.IsMoving)
            {
                PlayAnim("carrot_walk");
            }
            else
            {
                PlayAnim("carrot_idle");
            }
            // _controller.InputJump= ??
        }

        private void Default()
        {
            _controller.InputMovement = Vector3.zero;
            _controller.InputJump = false;
        }

        private string _activeAnimation;
        private void PlayAnim(string name)
        {
            if (_activeAnimation == name) return;

            Animator.CrossFade(name, 0.5f);
            _activeAnimation = name;
        }
    }
}