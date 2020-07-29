using System.Collections.Generic;
using System.Linq;
using Systems;
using Commands;
using Core;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IProgrammable
    {
        [SerializeField] private bool loopCommands = false;

        private List<Command> _commands;
        private int _commandIndex;
        private Health _health;


        // private KillBox _killBox;

        private void Start()
        {
            _commands = GetComponentsInChildren<Command>().ToList();
            // _killBox = GetComponentInChildren<KillBox>();
            _health = GetComponent<Health>();

            // as soon as this unit gets hit, unregister all commands

            _health.OnHit += SkipAll;
            // _health.OnHit += () => _killBox.gameObject.SetActive(false);
        }


        private void SkipAll()
        {
            foreach (Command c in _commands)
            {
                c.Skip();
            }
        }

        public void MoveOntoNextCommand()
        {
            _commandIndex++;
            if (_commandIndex == _commands.Count && loopCommands)
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
            if (loopCommands)
            {
                return _commands.Count > 0;
            }

            return _commandIndex < _commands.Count;
        }

        public bool OnLastCommand()
        {
            if (!loopCommands)
            {
                return false;
            }

            return _commandIndex == _commands.Count - 1;
        }


        public bool HasCompletedAllCommands()
        {
            // we have never finished all commands if we have any, we want to cycle through them
            if (loopCommands)
            {
                return _commands.Count == 0;
            }

            return _commands.All(c => c.IsFinished() || c.WasSkipped());
        }
    }
}