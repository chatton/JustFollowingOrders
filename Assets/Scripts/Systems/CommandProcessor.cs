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

        private IProgrammable[] _programmables;
        private Stack<IEnumerable<Command>> _undoStack;
        private Dictionary<IProgrammable, Command> _previousCommands;

        void Awake()
        {
            _previousCommands = new Dictionary<IProgrammable, Command>();
            _undoStack = new Stack<IEnumerable<Command>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            CommandBuffer.Instance.OnAssignCommands +=
                () => StartCoroutine(Instance.ProcessCommands());
        }

        public IEnumerator ProcessCommands()
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

                    command.Execute(Time.deltaTime);
                    _previousCommands[programmable] = command;

                    yield return new WaitForSeconds(Time.deltaTime / commandSpeed);

                    if (programmable.OnLastCommand())
                    {
                        command.AfterConsecutiveCommands();
                    }
                }
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
    }
}