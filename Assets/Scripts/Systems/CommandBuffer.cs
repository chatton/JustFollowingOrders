using System;
using System.Collections.Generic;
using Commands;
using Core;
using Movement;
using UnityEngine;
using Util;

namespace Systems
{
    public class CommandBuffer : MonoBehaviour
    {
        private Unit _unit;

        private List<Command> _commands;

        public List<Command> Commands => _commands;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _commands = new List<Command>();
        }

        public void AddCommand(Command command)
        {
            _commands.Add(command);
            AssignCommands();
        }

        public void AssignCommands()
        {
            _unit.AssignCommands(_commands);
        }

        public void RemoveCommand()
        {
            _commands.RemoveAt(_commands.Count - 1);
            AssignCommands();
        }
    }
}