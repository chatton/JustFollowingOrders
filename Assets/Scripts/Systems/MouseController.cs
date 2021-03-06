﻿using System;
using Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                // MonoCommandProcessor.Instance.Reset();
                return;
                // MonoCommandProcessor.Instance.UndoAll();
                // return;
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

                    LevelManager.Instance.CurrentUnit = hit.collider.gameObject;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (LevelManager.Instance.CurrentUnit != null)
                {
                    Debug.Log("REMOVE");
                    LevelManager.Instance.RemoveCommand();
                }
            }
        }
    }
}