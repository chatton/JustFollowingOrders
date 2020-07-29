using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Core
{
    public class Attacker : MonoBehaviour
    {
        [SerializeField] private Transform attackPointOrigin;
        [SerializeField] private int attackRange = 1;

        public Health GetTargetInRange()
        {
            Vector3 forward = attackPointOrigin.TransformDirection(Vector3.forward) * attackRange;
            if (Physics.Raycast(attackPointOrigin.position, forward, out RaycastHit hit, attackRange,
                LayerMask.GetMask("Attackable")))
            {
                return hit.collider.GetComponent<Health>();
            }

            return null;
        }


        private void Update()
        {
            Vector3 forward = attackPointOrigin.TransformDirection(Vector3.forward) * attackRange;
            Debug.DrawRay(attackPointOrigin.position, forward, Color.red);
        }
    }
}