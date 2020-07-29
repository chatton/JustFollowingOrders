using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour, IProgrammable
    {
        private List<ICommand> _commands;
        private int _commandIndex;

        [SerializeField] private bool _hasCompletedAllCommands;

        private void Start()
        {
            _commands = GetComponentsInChildren<ICommand>().ToList();
        }

        public ICommand ImmediateCommand { get; set; }
        public event Action OnReset;

        public void MoveOntoNextCommand()
        {
            _commandIndex++;
        }

        public ICommand CurrentCommand()
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


        public IEnumerable<ICommand> Commands()
        {
            return _commands;
        }

        public void AssignCommands(List<ICommand> commands)
        {
            _commands = new List<ICommand>(commands);
        }
    }
}