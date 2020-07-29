using System;
using System.Collections.Generic;
using Systems;
using Commands;
using UnityEngine;

namespace UI
{
    public class ActionButton : MonoBehaviour
    {
        private Dictionary<string, Func<CommandBuffer, ICommand>> _commandFuncs;

        private void Awake()
        {
            _commandFuncs = new Dictionary<string, Func<CommandBuffer, ICommand>>
            {
                {
                    "MoveForward",
                    t => CommandFactory.CreateMovementCommand(MoveDirection.Forward, t)
                },
                {"MoveBack", t => CommandFactory.CreateMovementCommand(MoveDirection.Back, t)},
                {"MoveLeft", t => CommandFactory.CreateMovementCommand(MoveDirection.Left, t)},
                {
                    "MoveRight",
                    t => CommandFactory.CreateMovementCommand(MoveDirection.Right, t)
                },
                {
                    "RotateRight",
                    t => CommandFactory.CreateRotationCommand(RotationDirection.Right, t)
                },
                {
                    "RotateLeft",
                    t => CommandFactory.CreateRotationCommand(RotationDirection.Left, t)
                },
                {
                    "Attack", t => CommandFactory.CreateAttackCommand(t)
                },
                {
                    "Wait", t => CommandFactory.CreateWaitCommand(t)
                }
            };
        }


        public void EnqueueAction(string actionName)
        {
            LevelManager lm = LevelManager.Instance;
            if (lm.SelectedCommandBuffer == null)
            {
                Debug.Log("There was no selected command buffer!");
                return;
            }

            ICommand cmd = _commandFuncs[actionName].Invoke(lm.SelectedCommandBuffer);
            lm.AddCommand(cmd);
            // TODO: implement hinting for commands
            // ShadowCommandProcessor.Instance.ProcessShadowCommand(cmd);
        }
    }
}