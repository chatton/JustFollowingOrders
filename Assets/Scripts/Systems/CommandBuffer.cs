using System;
using System.Collections.Generic;
using Commands;
using Core;
using Movement;
using UnityEngine;
using Util;

namespace Systems
{
    public class CommandBuffer : Singleton<CommandBuffer>
    {
        [SerializeField] private Unit unit;

        public event Action OnAssignCommands;
        private List<Command> _commands;

        private void Awake()
        {
            _commands = new List<Command>();
        }

        public void AddCommand(Command command)
        {
            _commands.Add(command);
        }

        public void AssignCommands()
        {
            unit.AssignCommands(_commands);
            OnAssignCommands?.Invoke();
        }
    }
}