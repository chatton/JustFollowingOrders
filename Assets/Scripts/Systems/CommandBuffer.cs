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
        private List<ICommand> _commands;

        private void Awake()
        {
            _commands = new List<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public void AssignCommands()
        {
            unit.AssignCommands(_commands);
        }
    }
}