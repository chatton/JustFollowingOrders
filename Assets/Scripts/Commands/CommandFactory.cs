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

        // public List<Command> CondenseMoveCommands(Command[] commands)
        // {
        //     List<Command> condensed = new List<Command>();
        //     List<MoveCommand> moveCommands = new List<MoveCommand>();
        //     foreach (Command command in commands)
        //     {
        //         if (!(command is MoveCommand))
        //         {
        //             if (moveCommands.Count != 0)
        //             {
        //                 // create our condensed move command!
        //
        //                 moveCommands.Clear();
        //             }
        //
        //             // regular command, nothing to condense!
        //             condensed.Add(command);
        //             continue;
        //         }
        //
        //         MoveCommand moveCommand = command as MoveCommand;
        //         moveCommands.Add(moveCommand);
        //     }
        //
        //     if (moveCommands.Count != 0)
        //     {
        //         // create our condensed move command!
        //
        //         moveCommands.Clear();
        //     }
        //
        //     return condensed;
        // }
    }
}