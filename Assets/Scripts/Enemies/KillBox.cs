using System;
using System.Collections;
using System.Linq;
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

            Attack();
        }

        private void Attack()
        {
            CommandProcessor.Instance.ExecutePriorityCommand(
                CommandFactory.Instance.CreateAttackCommand(transform.parent), 0.5f);
        }
    }
}