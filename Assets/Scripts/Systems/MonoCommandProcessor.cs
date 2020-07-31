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
    public class MonoCommandProcessor : Singleton<MonoCommandProcessor>
    {
        [SerializeField] private float commandSpeed = 5f;
        public bool IsProcessingCommands => false;
        public event Action OnBeginCommandProcessing;
        public event Action<ICommand> OnSkipCommand;

        private CommandProcessor _commandProcessor;
        private bool _shouldProcessCommands;

        private void Awake()
        {
            IProgrammable[] programmables = FindObjectsOfType<MonoBehaviour>().OfType<IProgrammable>().ToArray();
            _commandProcessor = new CommandProcessor(programmables);
        }


        public void StartProcessingCommands()
        {
            _shouldProcessCommands = true;
        }

        private void Update()
        {
            if (!_shouldProcessCommands || !_commandProcessor.ThereAreCommandsToProcess())
            {
                Debug.Log("Not processing commands");
                return;
            }

            _commandProcessor.ProcessCommands(Time.deltaTime);
        }

        public void Undo()
        {
            _commandProcessor.Undo();
        }

        public void ExecutePriorityCommand(ICommand command)
        {
            _commandProcessor.AddPriorityCommand(command);
        }

        public void UndoAll()
        {
            _shouldProcessCommands = false;
            _commandProcessor.UndoAll();
        }
    }
}