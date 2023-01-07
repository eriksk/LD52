using System;
using Scripts.Characters;
using UnityEngine;

namespace Scripts.AI
{
    [RequireComponent(typeof(EnemyCharacterController))]
    public class EnemyAI : MonoBehaviour
    {
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
            // _controller.InputJump= ??
        }

        private void Default()
        {
            _controller.InputMovement = Vector3.zero;
            _controller.InputJump = false;
        }
    }
}