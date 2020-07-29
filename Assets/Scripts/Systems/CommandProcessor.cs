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
        public event Action<Command> OnSkipCommand;

        private IProgrammable[] _programmables;
        private Stack<IEnumerable<Command>> _undoStack;
        private Command _priorityCommand;
        private float _afterSeconds;
        private Coroutine _coroutine;
        private List<Command> _priorityCommands;


        private HashSet<Command> _finishedCommands;
        private HashSet<Command> _skippedCommands;

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
            _undoStack = new Stack<IEnumerable<Command>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            _priorityCommands = new List<Command>();
            _finishedCommands = new HashSet<Command>();
            _skippedCommands = new HashSet<Command>();
        }

        private bool _shouldProcessCommands;

        private bool HasCompletedAllCommands(IProgrammable programmable)
        {
            foreach (Command command in programmable.Commands())
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
            Array.ForEach(_programmables, p => p.Reset());
        }

        public void StartProcessingShadowCommands()
        {
            // _processShadowCommands = true;
        }


        private bool ThereAreCommandsToProcess()
        {
            return _shouldProcessCommands && _programmables.Any(p => !HasCompletedAllCommands(p));
        }


        private void Update()
        {
            // if (_processShadowCommands)
            // {
            //     ProcessShadowCommands();
            //     return;
            // }

            if (!ThereAreCommandsToProcess())
            {
                // _skippedCommands.Clear();
                // _finishedCommands.Clear();
                Debug.Log("There are no commands to process");
                return;
            }

            ProcessRealCommands();
        }

        private void ProcessShadowCommands()
        {
        }

        private void ProcessShadowCommand(Command command)
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

                Command command = programmable.CurrentCommand();
                bool isValidCommandToExecute =
                    !(_finishedCommands.Contains(command) || _skippedCommands.Contains(command)) &&
                    command.CanBePerformed();

                if (isValidCommandToExecute)
                {
                    Debug.Log("Executing: " + command);
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
                foreach (Command command in _priorityCommands)
                {
                    if (_finishedCommands.Contains(command))
                    {
                        continue;
                    }

                    command.Execute(Time.deltaTime);
                    if (command.IsFinished())
                    {
                        _finishedCommands.Add(command);
                        // _undoStack.Push(new List<Command> {command});
                    }
                }

                if (AllPriorityCommandsAreFinished)
                {
                    Debug.Log("All Priority Commands have finished");

                    ProgressToNextCommands();
                    _priorityCommands.Clear();
                }
            }

            if (AllProgrammablesHaveCompletedAllCommands)
            {
                Debug.Log("Stopping processing commands");
                _shouldProcessCommands = false;
            }
        }


        private void ProgressToNextCommands()
        {
            List<Command> undoList = new List<Command>();
            foreach (IProgrammable programmable in _programmables)
            {
                Command command = programmable.CurrentCommand();
                if (command != null && !_skippedCommands.Contains(command) && _finishedCommands.Contains(command))
                {
                    Debug.Log("Adding command: " + command + " to the undo stack");
                    undoList.Add(command);
                }

                programmable.MoveOntoNextCommand();
            }


            List<Command> priorityList = new List<Command>();
            foreach (Command c in _priorityCommands)
            {
                if (c != null && !_skippedCommands.Contains(c) && _finishedCommands.Contains(c))
                {
                    Debug.Log("Adding command: " + c + " to the undo stack");
                    priorityList.Add(c);
                }
            }

            if (undoList.Count > 0)
            {
                _undoStack.Push(undoList);
            }

            if (priorityList.Count > 0)
            {
                _undoStack.Push(priorityList);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                foreach (Command command in _undoStack.Pop())
                {
                    Debug.Log("UNDO: " + command);
                    command.Undo();
                }
            }
        }

        public void ExecutePriorityCommand(Command command, float afterSeconds)
        {
            _priorityCommands.Add(command);
        }
    }
}