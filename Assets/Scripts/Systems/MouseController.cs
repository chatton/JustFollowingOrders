using System;
using UnityEngine;
using Util;

namespace Systems
{
    public class MouseController : Singleton<MouseController>
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U) && !MonoCommandProcessor.Instance.IsProcessingCommands)
            {
                MonoCommandProcessor.Instance.Undo();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                MonoCommandProcessor.Instance.UndoAll();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        return;
                    }

                    LevelManager.Instance.SelectedCommandBuffer = hit.collider.GetComponent<CommandBuffer>();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (LevelManager.Instance.SelectedCommandBuffer != null)
                {
                    LevelManager.Instance.RemoveCommand();
                }
            }
        }
    }
}