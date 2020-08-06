using System;
using System.Collections.Generic;
using Commands;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Util;

namespace Systems
{
    public class LevelManager : Singleton<LevelManager>
    {
        public bool HasKey { get; set; }
        private Key _key;

        public event Action<GameObject, List<ICommand>> OnCommandChanged;

        private Dictionary<string, GameObject> _unitNameMap;
        private CommandBuffer _selectedCommandBuffer;


        public GameObject _currentUnit;
        private Dictionary<string, List<ICommand>> _commandMap;


        private void Awake()
        {
            _commandMap = new Dictionary<string, List<ICommand>>();
            _unitNameMap = new Dictionary<string, GameObject>();
            _key = FindObjectOfType<Key>();
        }


        private List<ICommand> CurrentCommandList
        {
            get
            {
                if (_commandMap.ContainsKey(CurrentUnit.name))
                {
                    return _commandMap[CurrentUnit.name];
                }

                List<ICommand> commands = new List<ICommand>();
                _commandMap[CurrentUnit.name] = commands;
                return commands;
            }
        }


        // public GameObject CurrentGameObject { get; set; }

        public GameObject CurrentUnit
        {
            get => _currentUnit;
            set
            {
                if (value == null)
                {
                    Debug.Log("Trying to set _currentCommands to null!");
                    return;
                }

                _currentUnit = value;
                if (!_commandMap.ContainsKey(_currentUnit.name))
                {
                    _commandMap[value.name] = new List<ICommand>();
                    _unitNameMap[value.name] = value;
                }

                OnCommandChanged?.Invoke(_currentUnit, _commandMap[_currentUnit.name]);
            }
        }


        public void AddCommand(ICommand command)
        {
            if (_currentUnit == null)
            {
                Debug.LogError("Tried to add command to null list!");
                return;
            }

            CurrentCommandList.Add(command);
            _currentUnit.GetComponent<IProgrammable>().AssignCommands(CurrentCommandList);
            OnCommandChanged?.Invoke(CurrentUnit, CurrentCommandList);
        }

        public void RemoveCommand()
        {
            List<ICommand> currentList = CurrentCommandList;
            if (currentList.Count > 0)
            {
                currentList.RemoveAt(currentList.Count - 1);
                _currentUnit.GetComponent<IProgrammable>().AssignCommands(CurrentCommandList);
                OnCommandChanged?.Invoke(_currentUnit, CurrentCommandList);
            }
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void ResetKey()
        {
            HasKey = false;
            _key.gameObject.SetActive(true);
        }

        // ResetLevel will undo every command that has been executed effectively resetting the state
        // of the level
        public void ResetLevel()
        {
            // CommandProcessor.Instance.
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(0);
        }
    }
}