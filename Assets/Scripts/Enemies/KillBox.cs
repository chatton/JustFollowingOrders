using Systems;
using Commands;
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
            CommandProcessor.Instance.ExecutePriorityCommand(
                CommandFactory.Instance.CreateAttackCommand(transform.parent), 0.5f);
        }
    }
}