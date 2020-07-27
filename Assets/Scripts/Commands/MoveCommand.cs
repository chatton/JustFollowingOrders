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
        private Animator _animator;

        private Vector3? _targetPosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        private static readonly int Walking = Animator.StringToHash("Walking");

        private bool IsAtTargetPosition => _mover.transform.position == _targetPosition;

        private void Awake()
        {
            _mover = GetComponentInParent<Mover>();
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _targetPosition = null;
        }

        private void PlayMovementAnimation()
        {
            _animator.SetBool(Walking, true);
        }

        private void StopMovementAnimation()
        {
            _animator.SetBool(Walking, false);
        }

        public override void BeforeConsecutiveCommands()
        {
            PlayMovementAnimation();
        }

        public override void AfterConsecutiveCommands()
        {
            StopMovementAnimation();
        }

        public override bool CanPerformCommand()
        {
            return true; // TODO: check if tile is movable!
        }

        public override void Execute(float deltaTime)
        {
            // compute relative position when execute is called
            if (_targetPosition == null)
            {
                // PlayMovementAnimation();
                _targetPosition = GetTargetPosition(direction);
                Transform t = _mover.transform;
                _initialPosition = t.position;
                _initialRotation = t.rotation;
            }

            _mover.MoveTowards(_targetPosition.Value, deltaTime);
            // if (IsAtTargetPosition)
            // {
            //     StopMovementAnimation();
            // }
        }

        public override void Undo()
        {
            Transform t = _mover.transform;
            t.position = _initialPosition;
            t.rotation = _initialRotation;
        }

        public override bool IsFinished()
        {
            return IsAtTargetPosition;
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