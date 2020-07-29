using System;
using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using Util;
using System.Linq;
using Core;
using Enemies;

namespace Systems
{
    public class CommandProcessor : Singleton<CommandProcessor>
    {
        [SerializeField] private float commandSpeed = 5f;
        public bool IsProcessingCommands => false;
        public event Action OnBeginCommandProcessing;
        public event Action<ICommand> OnSkipCommand;

        private IProgrammable[] _programmables;
        private Stack<IEnumerable<ICommand>> _undoStack;
        private ICommand _priorityCommand;
        private float _afterSeconds;
        private Coroutine _coroutine;
        private List<ICommand> _priorityCommands;


        private HashSet<ICommand> _finishedCommands;
        private HashSet<ICommand> _skippedCommands;

        // private Unit _shadowUnit;
        // private bool _processShadowCommands;


        private bool AllProgrammablesHaveCompletedAllCommands => _programmables.All(HasCompletedAllCommands);

        private bool AllPriorityCommandsAreFinished =>
            _finishedCommands.Intersect(_priorityCommands).Count() == _priorityCommands.Count;


        private bool AllCommandsInCurrentRoundAreFinishedOrSkipped =>
            _programmables.All(p =>
                p.CurrentCommand() == null || _finishedCommands.Contains(p.CurrentCommand()) ||
                _skippedCommands.Contains(p.CurrentCommand()));


        private void Awake()
        {
            // _previousCommands = new Dictionary<IProgrammable, Command>();
            _undoStack = new Stack<IEnumerable<ICommand>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            _priorityCommands = new List<ICommand>();
            _finishedCommands = new HashSet<ICommand>();
            _skippedCommands = new HashSet<ICommand>();
        }

        private bool _shouldProcessCommands;

        private bool HasCompletedAllCommands(IProgrammable programmable)
        {
            foreach (ICommand command in programmable.Commands())
            {
                bool wasFinished = _finishedCommands.Contains(command);
                bool wasSkipped = _skippedCommands.Contains(command);
                if (wasFinished || wasSkipped)
                {
                    continue;
                }

                Debug.Log("Command: " + command + " was not skipped or finished!");
                return false;
            }

            return true;
        }

        public void StartProcessingCommands()
        {
            _shouldProcessCommands = true;
            // _finishedCommands.Clear();
            // _skippedCommands.Clear();
            // Array.ForEach(_programmables, p => p.Reset());
        }

        public void StartProcessingShadowCommands()
        {
            // _processShadowCommands = true;
        }


        private bool ThereAreCommandsToProcess()
        {
            bool haveRegularCommands = _programmables.Any(p => !HasCompletedAllCommands(p));
            bool havePriorityCommands =
                _priorityCommands.Any(c => !_finishedCommands.Contains(c) || !_skippedCommands.Contains(c));
            return _shouldProcessCommands && (haveRegularCommands || havePriorityCommands);
        }


        private void Update()
        {
            if (!ThereAreCommandsToProcess())
            {
                return;
            }

            ProcessRealCommands();
        }

        private void ProcessShadowCommands()
        {
        }

        private void ProcessShadowCommand(ICommand command)
        {
            while (!command.IsFinished())
            {
                command.Execute(Time.deltaTime);
            }
        }

        private void ProcessRealCommands()
        {
            foreach (IProgrammable programmable in _programmables)
            {
                if (!programmable.HasNextCommand())
                {
                    continue;
                }

                ICommand command = programmable.CurrentCommand();
                bool isValidCommandToExecute =
                    !(_finishedCommands.Contains(command) || _skippedCommands.Contains(command)) &&
                    command.CanBeExecuted();

                if (isValidCommandToExecute)
                {
                    command.Execute(Time.deltaTime);
                    if (command.IsFinished())
                    {
                        Debug.Log("FINISHING COMMAND: " + command);
                        _finishedCommands.Add(command);
                    }
                }
                else if (!_skippedCommands.Contains(command) && !_finishedCommands.Contains(command))
                {
                    Debug.Log("SKIPPING COMMAND: " + command);
                    _skippedCommands.Add(command);
                    OnSkipCommand?.Invoke(command);
                }
            }

            if (AllCommandsInCurrentRoundAreFinishedOrSkipped)
            {
                Debug.Log("All Commands this round have been finished or skipped");
                // we perform all priority commands before we move onto the next round of commands
                foreach (ICommand command in _priorityCommands)
                {
                    if (_finishedCommands.Contains(command))
                    {
                        Debug.Log("Skipping priority command: " + command);
                        continue;
                    }

                    command.Execute(Time.deltaTime);
                    if (command.IsFinished())
                    {
                        Debug.Log("FINISHING PRIORITY COMMAND: " + command);
                        _finishedCommands.Add(command);
                    }
                }

                if (AllPriorityCommandsAreFinished)
                {
                    Debug.Log("All Priority Commands have finished");

                    ProgressToNextCommands();
                    if (_priorityCommands.Any(c => !c.IsFinished()))
                    {
                        Debug.LogError(" Trying to clear priority commands when they are not finished!");
                    }

                    _priorityCommands.Clear();
                }
            }

            if (AllProgrammablesHaveCompletedAllCommands && AllPriorityCommandsAreFinished)
            {
                Debug.Log("Stopping processing commands");
                _shouldProcessCommands = false;
            }
        }


        private void ProgressToNextCommands()
        {
            List<ICommand> undoList = new List<ICommand>();
            foreach (IProgrammable programmable in _programmables)
            {
                ICommand command = programmable.CurrentCommand();
                if (command != null && !_skippedCommands.Contains(command) && _finishedCommands.Contains(command))
                {
                    Debug.Log("Adding command: " + command + " to the undo stack");
                    undoList.Add(command);
                }

                programmable.MoveOntoNextCommand();
            }

            List<ICommand> priorityList = new List<ICommand>();
            foreach (ICommand c in _priorityCommands)
            {
                if (c != null && !_skippedCommands.Contains(c) && _finishedCommands.Contains(c))
                {
                    Debug.Log("Adding priority command: " + c + " to the undo stack");
                    priorityList.Add(c);
                }
            }

            if (undoList.Count > 0)
            {
                _undoStack.Push(undoList);
            }

            if (priorityList.Count > 0)
            {
                Debug.Log("Adding priority command!");
                _undoStack.Push(priorityList);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                foreach (ICommand command in _undoStack.Pop())
                {
                    Debug.Log("UNDO: " + command);
                    command.Undo();
                }
            }
        }

        public void ExecutePriorityCommand(ICommand command, float afterSeconds)
        {
            _priorityCommands.Add(command);
        }

        public void SkipAll(IProgrammable programmable)
        {
            foreach (ICommand command in programmable.Commands())
            {
                _skippedCommands.Add(command);
            }
        }
    }
}