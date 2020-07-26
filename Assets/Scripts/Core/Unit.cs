using System.Collections.Generic;
using Systems;
using Commands;
using Movement;
using UnityEngine;

namespace Core
{
    public class Unit : MonoBehaviour, IProgrammable, ICommandAssignable
    {
        private List<ICommand> _commands;
        private int _commandIndex;
        private Mover _mover;

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _commands = new List<ICommand>();
        }


        public void MoveOntoNextCommand()
        {
            _commandIndex++;
        }

        public ICommand CurrentCommand()
        {
            return _commands[_commandIndex];
        }

        public bool HasNextCommand()
        {
            return _commandIndex < _commands.Count;
        }

        public void AssignCommands(IEnumerable<ICommand> commands)
        {
            _commands = new List<ICommand>(commands);
        }
    }
}