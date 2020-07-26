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

        void Awake()
        {
            _undoStack = new Stack<IEnumerable<Command>>();
            _programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            CommandBuffer.Instance.OnAssignCommands +=
                () => StartCoroutine(CommandProcessor.Instance.ProcessCommands());
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
                if (command != null)
                {
                    command.Execute(Time.deltaTime);
                    yield return new WaitForSeconds(Time.deltaTime / commandSpeed);
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