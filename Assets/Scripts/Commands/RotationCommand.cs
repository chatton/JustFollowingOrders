using System;
using Core;
using Movement;
using UnityEngine;
using World;

namespace Commands
{
    public class RotationCommand : ICommand
    {
        public RotationDirection Direction;
        private Mover _mover;
        private Health _health;

        private Quaternion? _startingRotation;
        private Quaternion _expectedQuaternion;
        private float _expectedY;


        public RotationCommand(RotationDirection direction, Mover mover, Health health)
        {
            Direction = direction;
            _mover = mover;
            _health = health;
        }

        public override string ToString()
        {
            return "Rotate " + Direction;
        }

        public bool CanBeExecuted()
        {
            return !_health.IsDead;
        }

        public void Execute(float deltaTime)
        {
            if (_startingRotation == null)
            {
                Quaternion rotation = _mover.transform.rotation;
                _startingRotation = rotation;
                _expectedY = (rotation.eulerAngles.y + GetYAxisRotationAngle(Direction)) % 360;
            }

            if (_startingRotation != null)
            {
                // we need to account for our current angle. We add the rotation angle on top of where we currently are
                _mover.Rotate(_startingRotation.Value.eulerAngles.y + GetYAxisRotationAngle(Direction), deltaTime);
            }
        }


        public void Undo()
        {
            _mover.transform.Rotate(Vector3.up, -GetYAxisRotationAngle(Direction));
        }

        public bool IsFinished()
        {
            if (_startingRotation == null)
            {
                return false;
            }


            // if the expected rotation is 360 degrees
            // it's possible that we are on the opposite end at ~0
            if (Math.Abs(_expectedY - 360f) < 0.01 || Math.Abs(_expectedY) < 0.01)
            {
                // if we are expecting 360, close to 0 is good enough!
                return Mathf.FloorToInt(_mover.transform.rotation.eulerAngles.y) == 0;
            }


            return Mathf.Approximately(_mover.transform.rotation.eulerAngles.y, _expectedY);
        }

        public Tile GetEndTile()
        {
            return null;
        }

        public static float GetYAxisRotationAngle(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                return 90f;
            }

            return 270f;
        }
    }
}