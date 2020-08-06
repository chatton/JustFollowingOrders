using Systems;
using Core;
using Movement;
using UnityEngine;
using UnityEngine.Assertions;

namespace Commands
{
    public static class CommandFactory
    {
        public static ICommand CreateMovementCommand(MoveDirection direction, GameObject buffer)
        {
            Mover mover = buffer.GetComponent<Mover>();
            Animator animator = buffer.GetComponentInChildren<Animator>();
            Health health = buffer.GetComponent<Health>();
            Assert.IsNotNull(mover);
            Assert.IsNotNull(animator);
            Assert.IsNotNull(health);
            return new MoveCommand(direction, mover, animator, health);
        }

        public static ICommand CreateRotationCommand(RotationDirection direction, GameObject buffer)
        {
            Mover mover = buffer.GetComponent<Mover>();
            Health health = buffer.GetComponent<Health>();
            Assert.IsNotNull(mover);
            Assert.IsNotNull(health);
            return new RotationCommand(direction, mover, health);
        }

        public static ICommand CreateAttackCommand(GameObject buffer)
        {
            Animator animator = buffer.GetComponentInChildren<Animator>();
            Attacker attacker = buffer.GetComponent<Attacker>();
            Assert.IsNotNull(animator);
            Assert.IsNotNull(attacker);
            return new AttackCommand(animator, attacker);
        }

        public static ICommand CreateWaitCommand(GameObject buffer)
        {
            Health health = buffer.GetComponent<Health>();
            Assert.IsNotNull(health);
            return new WaitCommand(health);
        }
    }
}