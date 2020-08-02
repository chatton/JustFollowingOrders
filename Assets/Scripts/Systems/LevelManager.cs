using System;
using Commands;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Systems
{
    public class LevelManager : Singleton<LevelManager>
    {
        public bool HasKey { get; set; }
        private Key _key;

        public event Action<CommandBuffer> OnCommandBufferChanged;

        private CommandBuffer _selectedCommandBuffer;

        private void Awake()
        {
            _key = FindObjectOfType<Key>();
        }

        public CommandBuffer SelectedCommandBuffer
        {
            get => _selectedCommandBuffer;
            set
            {
                if (value == null)
                {
                    Debug.Log("Trying to set _selectedCommandBuffer to null!");
                    return;
                }

                _selectedCommandBuffer = value;
                OnCommandBufferChanged?.Invoke(_selectedCommandBuffer);
            }
        }

        public void AddCommand(ICommand command)
        {
            if (SelectedCommandBuffer == null)
            {
                Debug.LogError("Tried to add command to null command buffer!");
                return;
            }

            SelectedCommandBuffer.AddCommand(command);
            OnCommandBufferChanged?.Invoke(SelectedCommandBuffer);
        }

        public void RemoveCommand()
        {
            SelectedCommandBuffer.RemoveCommand();
            OnCommandBufferChanged?.Invoke(SelectedCommandBuffer);
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
    }
}