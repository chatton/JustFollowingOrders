using System;
using System.Collections.Generic;
using Systems;
using Commands;
using UnityEngine;

namespace UI
{
    public class ActionButton : MonoBehaviour
    {
        private Dictionary<string, Func<GameObject, ICommand>> _commandFuncs;

        private void Awake()
        {
            _commandFuncs = new Dictionary<string, Func<GameObject, ICommand>>
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
            ICommand cmd = _commandFuncs[actionName].Invoke(lm.CurrentUnit );
            Debug.Log("Adding command: " + cmd);
            lm.AddCommand(cmd);
        }
    }
}