using System;
using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using Util;
using System.Linq;

namespace Systems
{
    public class CommandProcessor : Singleton<CommandProcessor>
    {
        [SerializeField] private float commandSpeed = 5f;
        public bool IsProcessingCommands => false;
        public event Action OnBeginCommandProcessing;

        private IProgrammable[] _programmables;
        private Stack<IEnumerable<Command>> _undoStack;
        private Dictionary<IProgrammable, Command> _previousCommands;
        private Command _priorityCommand;
        private float _afterSeconds;
        private Coroutine _coroutine;
        private List<Command> _priorityCommands;


        private bool AllProgrammablesHaveCompletedAllCommands => _programmables.All(p => p.HasCompletedAllCommands());
        private bool AllPriorityCommandsAreFinished => _priorityCommands.All(p => p.IsFinished());

        private bool AllCommandsAreFinished =>
            _programmables.All(p => p.CurrentCommand() == null || p.CurrentCommand().IsFinished());

        void Awake()
        {
            _previousCommands = new Dictionary<IProgrammable, Command>();
            _undoStack = new Stack<IEnumerable<Command>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            _priorityCommands = new List<Command>();
            // CommandBuffer.Instance.OnAssignCommands +=
            // () => StartCoroutine(Instance.ProcessCommands());
        }

        public void DoCommandProcessing()
        {
            if (_coroutine == null)
            {
                OnBeginCommandProcessing?.Invoke();
                _coroutine = StartCoroutine(ProcessCommands());
            }
            else
            {
                Debug.Log("Tried to call DoCommandProcessing when there was already a coroutine running!");
            }
        }

        private bool _shouldProcessCommands;

        public void StartProcessingCommands()
        {
            _shouldProcessCommands = true;
        }

        private bool ThereAreCommandsToProcess()
        {
            return _shouldProcessCommands && _programmables.Any(p => !p.HasCompletedAllCommands());
        }

        private void Update()
        {
            if (!ThereAreCommandsToProcess())
            {
                return;
            }

            foreach (IProgrammable programmable in _programmables)
            {
                if (!programmable.HasNextCommand())
                {
                    continue;
                }

                Command command = programmable.CurrentCommand();
                command.Execute(Time.deltaTime);
            }

            if (AllCommandsAreFinished)
            {
                // we perform all priority commands before we move onto the next round of commands
                foreach (Command command in _priorityCommands)
                {
                    command.Execute(Time.deltaTime);
                }

                if (AllPriorityCommandsAreFinished)
                {
                    _priorityCommands.Clear();
                    ProgressToNextCommands();
                }
            }

            if (AllProgrammablesHaveCompletedAllCommands)
            {
                _shouldProcessCommands = false;
            }
        }


        private IEnumerator ProcessCommands()
        {
            while (!_programmables.All(p => p.HasCompletedAllCommands()))
            {
                if (_programmables.All(p => !p.HasNextCommand()))
                {
                    Debug.Log("All commands finished running.");
                    yield break;
                }

                yield return ExecuteAllCommands();

                bool allCommandsFinished =
                    _programmables.All(p => p.CurrentCommand() == null || p.CurrentCommand().IsFinished());

                if (allCommandsFinished)
                {
                    ProgressToNextCommands();
                }
            }
        }

        private IEnumerator ExecuteAllCommands()
        {
            foreach (IProgrammable programmable in _programmables)
            {
                if (!programmable.HasNextCommand())
                {
                    continue;
                }

                Command command = programmable.CurrentCommand();
                bool commandIsNull = command == null;

                if (!commandIsNull)
                {
                    if (!_previousCommands.ContainsKey(programmable))
                    {
                        // this is the first command
                        _previousCommands[programmable] = command;
                    }

                    // command is not the same type as the previously executed one
                    if (_previousCommands[programmable].GetType() != command.GetType())
                    {
                        // end the previous command consecutive chain
                        _previousCommands[programmable].AfterConsecutiveCommands();
                    }
                    else
                    {
                        _previousCommands[programmable].BeforeConsecutiveCommands();
                    }

                    // TODO: remove command or something when it's not possible to perform it. For now just 
                    // skip over it
                    if (command.CanPerformCommand() || _priorityCommand != null)
                    {
                        bool stillCompletingPreviousCommand =
                            !command.IsFinished() && command == _previousCommands[programmable];

                        // make sure to finish any currently executing commands
                        if (stillCompletingPreviousCommand || _priorityCommand == null)
                        {
                            yield return HandleRegularCommand(command, programmable);
                        }
                        // then jump over to a priority command instead. We should never have more than one.
                        else if (_priorityCommand != null)
                        {
                            yield return HandlePriorityCommand(command);
                        }


                        if (programmable.OnLastCommand())
                        {
                            command.AfterConsecutiveCommands();
                        }
                    }
                    else
                    {
                        command.AfterConsecutiveCommands();
                        Destroy(command.gameObject);
                    }
                }
            }
        }

        private IEnumerator HandleRegularCommand(Command command, IProgrammable programmable)
        {
            command.Execute(Time.deltaTime);
            _previousCommands[programmable] = command;
            yield return new WaitForSeconds(Time.deltaTime / commandSpeed);
        }

        private IEnumerator HandlePriorityCommand(Command currentCommand)
        {
            if (currentCommand != null)
            {
                currentCommand.AfterConsecutiveCommands();
            }

            yield return new WaitForSeconds(_afterSeconds);
            _afterSeconds = 0;
            _priorityCommand.Execute(Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime / commandSpeed);
            if (_priorityCommand.IsFinished())
            {
                _undoStack.Push(new List<Command> {_priorityCommand});
                _priorityCommand = null;
            }
        }

        private void ProgressToNextCommands()
        {
            List<Command> undoList = new List<Command>();
            foreach (IProgrammable programmable in _programmables)
            {
                Command command = programmable.CurrentCommand();
                if (command != null)
                {
                    undoList.Add(programmable.CurrentCommand());
                }

                programmable.MoveOntoNextCommand();
            }

            _undoStack.Push(undoList);
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                foreach (Command command in _undoStack.Pop())
                {
                    command.Undo();
                }
            }
        }

        // public void ExecutePriorityCommand(Command command, float afterSeconds)
        // {
        //     _afterSeconds = afterSeconds;
        //     _priorityCommand = command;
        // }

        public void ExecutePriorityCommand(Command command, float afterSeconds)
        {
            _priorityCommands.Add(command);
        }
    }
}