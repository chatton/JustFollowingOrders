using System;
using System.Collections;
using System.Collections.Generic;
using Commands;
using UnityEngine;
using Util;
using System.Linq;
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


        private bool AllProgrammablesHaveCompletedAllCommands => _programmables.All(p => p.HasCompletedAllCommands());
        private bool AllPriorityCommandsAreFinished => _priorityCommands.All(p => p.IsFinished());

        private bool AllCommandsAreFinished =>
            _programmables.All(p =>
                p.CurrentCommand() == null || p.CurrentCommand().IsFinished() || p.CurrentCommand().WasSkipped());

        void Awake()
        {
            // _previousCommands = new Dictionary<IProgrammable, Command>();
            _undoStack = new Stack<IEnumerable<Command>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            _priorityCommands = new List<Command>();
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
                if (command.CanBePerformed())
                {
                    command.Execute(Time.deltaTime);
                }
                else
                {
                    command.Skip();
                    OnSkipCommand?.Invoke(command);
                }
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

        public void ExecutePriorityCommand(Command command, float afterSeconds)
        {
            _priorityCommands.Add(command);
        }
    }
}