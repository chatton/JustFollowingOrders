using System;
using System.Collections;
using Systems;
using Core;
using Movement;
using UnityEngine;
using World;

namespace Commands
{
    public class MoveCommand : ICommand
    {
        public readonly MoveDirection Direction;
        private Mover _mover;
        private Animator _animator;
        private Health _health;

        private Vector3? _targetPosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private bool? _doable;
        private Vector3 _oldPos;

        // private static readonly int Walking = Animator.StringToHash("Walking");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private bool IsAtTargetPosition => _mover.transform.position == _targetPosition;

        public MoveCommand(MoveDirection direction, Mover mover, Animator animator, Health health)
        {
            Direction = direction;
            _mover = mover;
            _animator = animator;
            _health = health;
            _targetPosition = null;
        }


        public override string ToString()
        {
            return Direction.ToString();
        }

        public bool CanBeExecuted()
        {
            if (_health.IsDead)
            {
                return false;
            }

            if (_doable == null)
            {
                // whether or not the move is valid is determined at the first execution.
                _doable = _mover.CanMoveInDirection(Direction);
            }

            return _doable.Value;
        }

        public void Execute(float deltaTime)
        {
            // compute relative position when execute is called
            if (_targetPosition == null)
            {
                _targetPosition = GetTargetPosition();
                Transform t = _mover.transform;
                _initialPosition = t.position;
                _initialRotation = t.rotation;
            }

            _animator.SetFloat(Speed, 1f);

            _mover.MoveTowards(_targetPosition.Value, deltaTime);
        }

        public void Undo()
        {
            Transform t = _mover.transform;
            t.position = _initialPosition;
            t.rotation = _initialRotation;
            _targetPosition = null;
            _doable = null;
        }

        public bool IsFinished()
        {
            return IsAtTargetPosition;
        }

        private Vector3 GetTargetPosition()
        {
            Transform moverTransform = _mover.transform;
            Vector3 pos = moverTransform.position;

            switch (Direction)
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

            throw new Exception("Unexpected move direction: " + Direction);
        }
    }
}