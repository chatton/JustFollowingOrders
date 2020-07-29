using System;
using Core;
using Movement;
using UnityEngine;

namespace Commands
{
    public class MoveCommand : ICommand
    {
        [SerializeField] public MoveDirection direction;
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
            this.direction = direction;
            _mover = mover;
            _animator = animator;
            _health = health;
            _targetPosition = null;
        }


        public override string ToString()
        {
            return direction.ToString();
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
                _doable = _mover.CanMoveInDirection(direction);
            }

            return _doable.Value;
        }

        public void Execute(float deltaTime)
        {
            // compute relative position when execute is called
            if (_targetPosition == null)
            {
                _targetPosition = GetTargetPosition(direction);
                Transform t = _mover.transform;
                _initialPosition = t.position;
                _initialRotation = t.rotation;
            }

            // float speedPerSec = Vector3.Distance(_oldPos, transform.parent.position) / deltaTime;
            // float speed = Vector3.Distance(_oldPos, transform.parent.position) / deltaTime;
            // Debug.Log(speed);
            // _oldPos = transform.parent.position;

            // if (_targetPosition == transform.position)
            // {
            //     Debug.Log("HERE!");
            //     _animator.SetFloat(Speed, 0f);
            // }
            // else
            // {
            _animator.SetFloat(Speed, 1f);
            // }


            // if (IsAtTargetPosition)
            // {
            //     _animator.SetFloat(Speed, 0f);
            // }

            _mover.MoveTowards(_targetPosition.Value, deltaTime);
        }

        public void Undo()
        {
            Transform t = _mover.transform;
            t.position = _initialPosition;
            t.rotation = _initialRotation;
        }

        public bool IsFinished()
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