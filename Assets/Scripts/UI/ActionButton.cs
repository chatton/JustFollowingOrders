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
        private Dictionary<string, Func<ICommand>> commandFuncs;

        [SerializeField] private Mover _mover;

        private void Awake()
        {
            _mover = FindObjectOfType<Mover>();
            commandFuncs = new Dictionary<string, Func<ICommand>>
            {
                {"MoveForward", () => new MoveCommand(_mover, MoveDirection.Forward)},
                {"MoveBack", () => new MoveCommand(_mover, MoveDirection.Back)},
                {"MoveLeft", () => new MoveCommand(_mover, MoveDirection.Left)},
                {"MoveRight", () => new MoveCommand(_mover, MoveDirection.Right)},
                {"RotateRight", () => new RotationCommand(_mover, RotationDirection.Right)},
                {"RotateLeft", () => new RotationCommand(_mover, RotationDirection.Left)}
            };
        }


        public void EnqueueAction(string actionName)
        {
            Debug.Log(actionName);
            CommandBuffer.Instance.AddCommand(commandFuncs[actionName].Invoke());
        }
    }
}