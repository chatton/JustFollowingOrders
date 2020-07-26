using System;
using UnityEngine;
using Util;

namespace Commands
{
    public class CommandFactory : Singleton<CommandFactory>
    {
        [SerializeField] private MoveCommand moveCommandPrefab;
        [SerializeField] private RotationCommand rotateCommandPrefab;

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
    }
}