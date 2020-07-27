using System.Collections.Generic;
using System.Linq;
using Commands;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IProgrammable
    {
        public Command ImmediateCommand { get; set; }
        private List<Command> _commands;
        private int _commandIndex;

        private void Start()
        {
            _commands = GetComponentsInChildren<Command>().ToList();
        }


        public void MoveOntoNextCommand()
        {
            _commandIndex++;
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
            return _commandIndex < _commands.Count;
        }

        public bool OnLastCommand()
        {
            return _commandIndex == _commands.Count - 1;
        }


        public bool HasCompletedAllCommands()
        {
            return OnLastCommand() && CurrentCommand().IsFinished();
        }
    }
}