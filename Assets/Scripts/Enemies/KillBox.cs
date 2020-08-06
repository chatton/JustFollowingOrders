using System;
using Systems;
using Commands;
using Core;
using UnityEngine;

namespace Enemies
{
    public class KillBox : MonoBehaviour
    {
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
            if (GetComponentInParent<Health>().IsDead)
            {
                return;
            }

            MonoCommandProcessor.Instance.ExecutePriorityCommand(
                CommandFactory.CreateAttackCommand(transform.parent.gameObject));
        }
    }
}