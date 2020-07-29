using Systems;
using Core;
using Movement;
using UnityEngine;

namespace Commands
{
    public static class CommandFactory
    {
        public static ICommand CreateMovementCommand(MoveDirection direction, CommandBuffer buffer)
        {
            Mover mover = buffer.GetComponent<Mover>();
            Animator animator = buffer.GetComponentInChildren<Animator>();
            Health health = buffer.GetComponent<Health>();
            return new MoveCommand(direction, mover, animator, health);
        }

        public static ICommand CreateRotationCommand(RotationDirection direction, CommandBuffer buffer)
        {
            Mover mover = buffer.GetComponent<Mover>();
            Health health = buffer.GetComponent<Health>();
            return new RotationCommand(direction, mover, health);
        }

        public static ICommand CreateAttackCommand(CommandBuffer buffer)
        {
            Animator animator = buffer.GetComponentInChildren<Animator>();
            Health health = buffer.GetComponent<Health>();
            Attacker attacker = buffer.GetComponent<Attacker>();
            return new AttackCommand(animator, health, attacker);
        }

        public static ICommand CreateWaitCommand(CommandBuffer buffer)
        {
            Health health = buffer.GetComponent<Health>();
            return new WaitCommand(health);
        }
    }
}