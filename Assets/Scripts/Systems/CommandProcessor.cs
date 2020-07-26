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
        public bool IsProcessingCommands => false;

        private IProgrammable[] _programmables;
        private Stack<IEnumerable<ICommand>> _undoStack;

        void Awake()
        {
            _undoStack = new Stack<IEnumerable<ICommand>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
        }

        void Update()
        {
            if (_programmables.All(p => !p.HasNextCommand()))
            {
                Debug.Log("All commands finished running.");
                return; // no commands left to run!
            }

            ExecuteAllCommands();

            bool allCommandsFinished = _programmables.All(p => p.CurrentCommand().IsFinished());

            if (allCommandsFinished)
            {
                ProgressToNextCommands();
            }
        }

        private void ExecuteAllCommands()
        {
            foreach (IProgrammable programmable in _programmables)
            {
                if (!programmable.HasNextCommand())
                {
                    continue;
                }

                ICommand command = programmable.CurrentCommand();
                command.Execute(Time.deltaTime);
            }
        }

        private void ProgressToNextCommands()
        {
            List<ICommand> undoList = new List<ICommand>();
            foreach (IProgrammable programmable in _programmables)
            {
                undoList.Add(programmable.CurrentCommand());
                programmable.MoveOntoNextCommand();
            }

            _undoStack.Push(undoList);
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                foreach (ICommand command in _undoStack.Pop())
                {
                    command.Undo();
                }
            }
        }
    }
}