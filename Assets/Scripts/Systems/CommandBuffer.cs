using System.Collections.Generic;
using Commands;
using UnityEngine;

namespace Systems
{
    public class CommandBuffer : MonoBehaviour
    {
        private IProgrammable _programmable;

        private List<ICommand> _commands;

        public List<ICommand> Commands => _commands;

        private void Start()
        {
            _programmable = GetComponent<IProgrammable>();
            // _unit.OnReset += AssignCommands;
            _commands = new List<ICommand>();
        }

        public void AddCommand(ICommand command)
        {
            _commands.Add(command);
            AssignCommands();
        }

        public void AssignCommands()
        {
            _programmable.AssignCommands(_commands);
        }

        public void RemoveCommand()
        {
            if (_commands.Count > 0)
            {
                _commands.RemoveAt(_commands.Count - 1);
                AssignCommands();
            }
        }
    }
}