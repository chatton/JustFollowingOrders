using System;
using System.Collections.Generic;
using Systems;
using Commands;
using Movement;
using UnityEngine;

namespace UI
{
    public class ActionButton : MonoBehaviour
    {
        private Dictionary<string, Func<Transform, Command>> commandFuncs;

        private void Awake()
        {
            commandFuncs = new Dictionary<string, Func<Transform, Command>>
            {
                {
                    "MoveForward",
                    t => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Forward, t)
                },
                {"MoveBack", t => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Back, t)},
                {"MoveLeft", t => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Left, t)},
                {
                    "MoveRight",
                    t => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Right, t)
                },
                {
                    "RotateRight",
                    t => CommandFactory.Instance.CreateRotationCommand(RotationDirection.Right, t)
                },
                {
                    "RotateLeft",
                    t => CommandFactory.Instance.CreateRotationCommand(RotationDirection.Left, t)
                },
                {
                    "Attack", t => CommandFactory.Instance.CreateAttackCommand(t)
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

            lm.AddCommand(commandFuncs[actionName].Invoke(lm.SelectedCommandBuffer.transform));
        }
        
    }
}