using System;
using Movement;
using UnityEngine;

namespace Commands
{
    public class RotationCommand : ICommand
    {
        private readonly Mover _mover;
        private readonly RotationDirection _direction;
        private Quaternion? _startingRotation;
        private Quaternion _expectedQuaternion;
        private float _expectedY;

        public RotationCommand(Mover mover, RotationDirection direction)
        {
            _mover = mover;
            _direction = direction;
        }

        public void Execute(float deltaTime)
        {
            if (_startingRotation == null)
            {
                Quaternion rotation = _mover.transform.rotation;
                _startingRotation = rotation;
                _expectedY = (rotation.eulerAngles.y + GetYAxisRotationAngle(_direction)) % 360;
            }

            if (_startingRotation != null)
            {
                // we need to account for our current angle. We add the rotation angle on top of where we currently are
                _mover.Rotate(_startingRotation.Value.eulerAngles.y + GetYAxisRotationAngle(_direction), deltaTime);
            }
        }


        public void Undo()
        {
            _mover.transform.Rotate(Vector3.up, -GetYAxisRotationAngle(_direction));
        }

        public bool IsFinished()
        {
            if (_startingRotation == null)
            {
                return false;
            }

            // Debug.Log("ExpectedY" + _expectedY);
            // Debug.Log(_mover.transform.rotation.eulerAngles.y);

            // if the expected rotation is 360 degrees
            // it's possible that we are on the opposite end at ~0
            if (Math.Abs(_expectedY - 360f) < 0.01 || Math.Abs(_expectedY) < 0.01)
            {
                // if we are expecting 360, close to 0 is good enough!
                return Mathf.FloorToInt(_mover.transform.rotation.eulerAngles.y) == 0;
            }


            return Mathf.Approximately(_mover.transform.rotation.eulerAngles.y, _expectedY);
        }

        private float GetYAxisRotationAngle(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                return 90f;
            }

            return 270f;
        }
    }
}