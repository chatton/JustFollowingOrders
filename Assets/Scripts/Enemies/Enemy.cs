using System.Collections.Generic;
using System.Linq;
using Commands;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IProgrammable
    {
        private List<Command> _commands;
        private int _commandIndex;

        private void Start()
        {
            _commands = GetComponentsInChildren<Command>().ToList();
        }


        public void MoveOntoNextCommand()
        {
            _commandIndex++;
            if (_commandIndex == _commands.Count)
            {
                _commandIndex = 0;
            }
        }

        public Command CurrentCommand()
        {
            if (_commandIndex >= _commands.Count)
            {
                return null;
            }

            return _commands[_commandIndex];
        }

        public bool HasNextCommand()
        {
            return _commands.Count > 0;
        }

        public bool OnLastCommand()
        {
            return false;
        }


        public bool HasCompletedAllCommands()
        {
            // we have never finished all commands if we have any, we want to cycle through them
            return _commands.Count == 0;
        }
    }
}