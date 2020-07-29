using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour, IProgrammable
    {
        private List<Command> _commands;
        private int _commandIndex;

        [SerializeField] private bool _hasCompletedAllCommands;

        private void Start()
        {
            _commands = GetComponentsInChildren<Command>().ToList();
        }

        public Command ImmediateCommand { get; set; }
        public event Action OnReset;

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

        public void Reset()
        {
            _commandIndex = 0;
            OnReset?.Invoke();
        }

        public bool OnLastCommand()
        {
            return _commandIndex == _commands.Count - 1;
        }


        public bool HasCompletedAllCommands()
        {
            _hasCompletedAllCommands = _commands.All(c => c.IsFinished() || c.WasSkipped());
            return _hasCompletedAllCommands;
            // return OnLastCommand() && CurrentCommand().IsFinished();
        }

        public IEnumerable<Command> Commands()
        {
            return _commands;
        }

        public void AssignCommands(IEnumerable<Command> commands)
        {
            _commands = new List<Command>(commands);
            Debug.Log(commands);
        }
    }
}