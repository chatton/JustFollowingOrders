using System;
using System.Collections.Generic;
using Movement;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Mover _mover;
        private readonly MoveDirection _direction;

        private Vector3? _targetPosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        public MoveCommand(Mover mover, MoveDirection direction)
        {
            _mover = mover;
            _direction = direction;
            _targetPosition = null;
        }

        public void Execute(float deltaTime)
        {
            // compute relative position when execute is called
            if (_targetPosition == null)
            {
                _targetPosition = GetTargetPosition(_direction);
                Transform transform = _mover.transform;
                _initialPosition = transform.position;
                _initialRotation = transform.rotation;
            }

            _mover.MoveTowards(_targetPosition.Value, deltaTime);
        }

        public void Undo()
        {
            Transform transform = _mover.transform;
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        public bool IsFinished()
        {
            return _mover.transform.position == _targetPosition;
        }

        private Vector3 GetTargetPosition(MoveDirection moveDirection)
        {
            Transform moverTransform = _mover.transform;
            Vector3 pos = moverTransform.position;

            switch (moveDirection)
            {
                case MoveDirection.Forward:
                    return pos + moverTransform.forward.normalized;
                case MoveDirection.Back:
                    return pos - moverTransform.forward.normalized;
                case MoveDirection.Right:
                    return pos + moverTransform.right.normalized;
                case MoveDirection.Left:
                    return pos - moverTransform.right.normalized;
            }

            throw new Exception("Unexpected move direction: " + moveDirection);
        }
    }
}