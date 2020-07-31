using System;
using Systems;
using Commands;
using UnityEngine;

namespace Enemies
{
    public class KillBox : MonoBehaviour
    {
        private CommandBuffer _buffer;

        private void Awake()
        {
            _buffer = GetComponentInParent<CommandBuffer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            // Attack if the player is in range
            Attack();
        }

        private void Attack()
        {
            MonoCommandProcessor.Instance.ExecutePriorityCommand(
                CommandFactory.CreateAttackCommand(_buffer));
        }
    }
}