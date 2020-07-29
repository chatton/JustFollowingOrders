using System;
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
        [SerializeField] private bool _hasCompletedAllCommands;
        [SerializeField] private CommandType[] commandTypes;

        private List<ICommand> _commands;
        private int _commandIndex;
        private Health _health;
        private CommandBuffer _buffer;


        // private KillBox _killBox;

        private void Start()
        {
            _buffer = GetComponent<CommandBuffer>();
            _commands = BuildCommands();
            _health = GetComponent<Health>();
            _health.OnHit += SkipAll;
        }


        private List<ICommand> BuildCommands()
        {
            List<ICommand> allCommands = new List<ICommand>();
            foreach (CommandType commandType in commandTypes)
            {
                switch (commandType)
                {
                    case CommandType.MoveForward:
                        allCommands.Add(CommandFactory.CreateMovementCommand(MoveDirection.Forward, _buffer));
                        break;
                    case CommandType.MoveBack:
                        allCommands.Add(CommandFactory.CreateMovementCommand(MoveDirection.Back, _buffer));
                        break;
                    case CommandType.MoveLeft:
                        allCommands.Add(CommandFactory.CreateMovementCommand(MoveDirection.Left, _buffer));
                        break;
                    case CommandType.MoveRight:
                        allCommands.Add(CommandFactory.CreateMovementCommand(MoveDirection.Right, _buffer));
                        break;
                    case CommandType.Attack:
                        allCommands.Add(CommandFactory.CreateAttackCommand(_buffer));
                        break;
                    case CommandType.Wait:
                        allCommands.Add(CommandFactory.CreateWaitCommand(_buffer));
                        break;
                    case CommandType.RotateLeft:
                        allCommands.Add(CommandFactory.CreateRotationCommand(RotationDirection.Left, _buffer));
                        break;
                    case CommandType.RotateRight:
                        allCommands.Add(CommandFactory.CreateRotationCommand(RotationDirection.Right, _buffer));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return allCommands;
        }

        private void SkipAll()
        {
            CommandProcessor.Instance.SkipAll(this);
        }

        public void MoveOntoNextCommand()
        {
            _commandIndex++;
            if (_commandIndex == _commands.Count && loopCommands)
            {
                _commandIndex = 0;
            }
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
            if (loopCommands)
            {
                return _commands.Count > 0;
            }

            return _commandIndex < _commands.Count;
        }

        public void Reset()
        {
            Debug.Log("Enemy::Reset");
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