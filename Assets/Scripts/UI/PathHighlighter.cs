﻿using System;
using System.Collections.Generic;
using Systems;
using Commands;
using UnityEngine;
using Util;
using World;

namespace UI
{
    public struct PathNode
    {
        public Vector3 position;
        public int turnNo;
    }

    public class PathHighlighter : Singleton<PathHighlighter>
    {
        private List<PathNode> _path;
        private Tile[] _allTiles;

        private void Awake()
        {
            _path = new List<PathNode>();
            _allTiles = FindObjectsOfType<Tile>();
            LevelManager.Instance.OnCommandChanged += HighlightTiles;
            MonoCommandProcessor.Instance.OnBeginCommandProcessing += UnhighlightTiles;
        }

        private void UnhighlightTiles()
        {
            Array.ForEach(_allTiles, t => t.Unlight());
        }

        private void OnDrawGizmos()
        {
            if (_path == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            foreach (PathNode n in _path)
            {
                Debug.Log(n.position);
                Gizmos.DrawWireSphere(n.position, 0.5f);
            }
        }

        private Vector3 GetVector3ToDrawAt(MoveCommand moveCommand, Transform bufferTransform)
        {
            switch (moveCommand.Direction)
            {
                case MoveDirection.Forward:
                    return bufferTransform.position + bufferTransform.forward.normalized;
                case MoveDirection.Back:
                    return bufferTransform.position - bufferTransform.forward.normalized;
                case MoveDirection.Right:
                    return bufferTransform.position + bufferTransform.right.normalized;
                case MoveDirection.Left:
                    return bufferTransform.position - bufferTransform.right.normalized;
                default:
                    throw new Exception("Unknown direction");
            }
        }


        private void ClearPath()
        {
            _path.Clear();
        }

        private void HighlightTiles(GameObject currentUnit, List<ICommand> commands)
        {
            UnhighlightTiles();
            GameObject go = new GameObject();
            Transform bufferTransform = currentUnit.transform;
            go.transform.position = bufferTransform.position;
            go.transform.rotation = bufferTransform.rotation;
            ClearPath();

            int turnNo = 1;
            foreach (ICommand selectedCommand in commands)
            {
                switch (selectedCommand)
                {
                    case MoveCommand moveCommand:
                        go.transform.position = GetVector3ToDrawAt(moveCommand, go.transform);
                        break;
                    case RotationCommand rotationCommand:
                        go.transform.Rotate(Vector3.up,
                            RotationCommand.GetYAxisRotationAngle(rotationCommand.Direction));
                        break;
                    case AttackCommand _:
                    case WaitCommand _:
                        break;
                    default:
                        throw new Exception("Unknown command type! " + selectedCommand);
                }

                _path.Add(new PathNode
                {
                    turnNo = turnNo,
                    position = go.transform.position + Vector3.up
                });
                turnNo++;
            }

            Destroy(go);

            foreach (PathNode n in _path)
            {
                if (Physics.Raycast(n.position, Vector3.down, out RaycastHit hit, 10f,
                    LayerMask.GetMask("Tile")))
                {
                    Tile t = hit.collider.GetComponent<Tile>();
                    t.LightUp(n.turnNo);
                }
            }
        }
    }
}