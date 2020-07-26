using System;
using System.Collections.Generic;
using Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Commands
{
    public class MoveCommand : Command
    {
        [SerializeField] public MoveDirection direction;
        private Mover _mover;

        private Vector3? _targetPosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _mover = GetComponentInParent<Mover>();
            _targetPosition = null;
        }


        public override void Execute(float deltaTime)
        {
            // compute relative position when execute is called
            if (_targetPosition == null)
            {
                _targetPosition = GetTargetPosition(direction);
                Transform t = _mover.transform;
                _initialPosition = t.position;
                _initialRotation = t.rotation;
            }

            _mover.MoveTowards(_targetPosition.Value, deltaTime);
        }

        public override void Undo()
        {
            Transform t = _mover.transform;
            t.position = _initialPosition;
            t.rotation = _initialRotation;
        }

        public override bool IsFinished()
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