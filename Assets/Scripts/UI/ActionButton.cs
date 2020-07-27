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
        private Dictionary<string, Func<Command>> commandFuncs;

        private void Awake()
        {
            commandFuncs = new Dictionary<string, Func<Command>>
            {
                {
                    "MoveForward",
                    () => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Forward, _mover.transform)
                },
                {"MoveBack", () => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Back, _mover.transform)},
                {"MoveLeft", () => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Left, _mover.transform)},
                {
                    "MoveRight",
                    () => CommandFactory.Instance.CreateMovementCommand(MoveDirection.Right, _mover.transform)
                },
                {
                    "RotateRight",
                    () => CommandFactory.Instance.CreateRotationCommand(RotationDirection.Right, _mover.transform)
                },
                // {"RotateLeft", () => CommandFactory.Instance.CreateRotationCommand(RotationDirection.Left, _mover.transform)}
                {"RotateLeft", () => CommandFactory.Instance.CreateAttackCommand(_mover.transform)}
            };
        }

        [SerializeField] private Mover _mover;


        public void EnqueueAction(string actionName)
        {
            Debug.Log(actionName);
            CommandBuffer.Instance.AddCommand(commandFuncs[actionName].Invoke());
        }
    }
}