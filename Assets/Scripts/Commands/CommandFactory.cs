using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEngine;
using Util;

namespace Commands
{
    public class CommandFactory : Singleton<CommandFactory>
    {
        [SerializeField] private MoveCommand moveCommandPrefab;
        [SerializeField] private RotationCommand rotateCommandPrefab;
        [SerializeField] private AttackCommand attackCommandPrefab;
        [SerializeField] private WaitCommand waitCommandPrefab;

        public Command CreateMovementCommand(MoveDirection direction, Transform parent)
        {
            MoveCommand command =
                Instantiate(moveCommandPrefab, parent.transform.position, Quaternion.identity, parent);
            command.direction = direction;
            return command;
        }

        public Command CreateRotationCommand(RotationDirection direction, Transform parent)
        {
            RotationCommand command =
                Instantiate(rotateCommandPrefab, parent.transform.position, Quaternion.identity, parent);
            command.direction = direction;
            return command;
        }

        public Command CreateAttackCommand(Transform parent)
        {
            AttackCommand command =
                Instantiate(attackCommandPrefab, parent.transform.position, Quaternion.identity, parent);
            return command;
        }

        public Command CreateWaitCommand(Transform parent)
        {
            WaitCommand command =
                Instantiate(waitCommandPrefab, parent.transform.position, Quaternion.identity, parent);
            return command;
        }
    }
}