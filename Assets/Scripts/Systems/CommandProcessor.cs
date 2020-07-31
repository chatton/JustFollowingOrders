using System;
using System.Collections.Generic;
using Commands;
using System.Linq;
using UnityEngine;

namespace Systems
{
    public class CommandProcessor
    {
        private readonly IProgrammable[] _programmables;
        private readonly Stack<IEnumerable<ICommand>> _undoStack;
        private readonly List<ICommand> _priorityCommands;

        // finished commands keeps track of all of the commands that have successfully finished.
        // they are saved here as the result of command.IsFinished() can no longer be trusted after the
        // command has finished executing
        private readonly HashSet<ICommand> _finishedCommands;
        private readonly HashSet<ICommand> _skippedCommands;


        public CommandProcessor(IProgrammable[] programmables)
        {
            _programmables = programmables;
            _undoStack = new Stack<IEnumerable<ICommand>>();
            _priorityCommands = new List<ICommand>();
            _finishedCommands = new HashSet<ICommand>();
            _skippedCommands = new HashSet<ICommand>();
        }

        private bool AllProgrammablesHaveCompletedAllCommands => _programmables.All(HasCompletedAllCommands);

        private bool AllPriorityCommandsAreFinished =>
            _finishedCommands.Intersect(_priorityCommands).Count() == _priorityCommands.Count;


        private bool AllCommandsInCurrentRoundAreFinishedOrSkipped =>
            _programmables.All(p =>
                p.CurrentCommand() == null || _finishedCommands.Contains(p.CurrentCommand()) ||
                _skippedCommands.Contains(p.CurrentCommand()));

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

        public bool ThereAreCommandsToProcess()
        {
            bool haveRegularCommands = _programmables.Any(p => !HasCompletedAllCommands(p));
            bool havePriorityCommands =
                _priorityCommands.Any(c => !_finishedCommands.Contains(c) || !_skippedCommands.Contains(c));
            return haveRegularCommands || havePriorityCommands;
        }

        public void ProcessCommands(float deltaTime)
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
                    command.Execute(deltaTime);
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
                    // OnSkipCommand?.Invoke(command);
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

                    command.Execute(deltaTime);
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

        public void AddPriorityCommand(ICommand command)
        {
            _priorityCommands.Add(command);
        }

        public void UndoAll()
        {
            while (_undoStack.Count > 0)
            {
                Undo();
            }

            Array.ForEach(_programmables, p => p.Reset());
            _finishedCommands.Clear();
            _skippedCommands.Clear();
        }
    }
}