using System;
using System.Collections;
using Core;
using UnityEngine;

namespace Commands
{
    public class WaitCommand : Command
    {
        private bool _isFinished;
        private float _elapsedTime;
        private float _finishedAfter = 0.5f;
        private Health _health;

        private void Awake()
        {
            _health = GetComponentInParent<Health>();
        }

        public override bool IsFinished()
        {
            if (_isFinished)
            {
                _isFinished = false;
                return true;
            }

            return false;
            // return _isFinished;
        }

        public override string ToString()
        {
            return "Wait";
        }

        public override void Execute(float deltaTime)
        {
            if (_isFinished)
            {
                return;
            }

            _elapsedTime += deltaTime;
            if (_elapsedTime >= _finishedAfter)
            {
                _isFinished = true;
                _elapsedTime = 0;
            }

            // if (!transform.parent.gameObject.activeSelf) return;
            // _isFinished = false;
            // StartCoroutine(Wait(1f));
        }


        #region EmptyMethods

        public override void BeforeConsecutiveCommands()
        {
        }

        public override void AfterConsecutiveCommands()
        {
        }

        protected override bool DoCanPerformCommand()
        {
            return !_health.IsDead;
        }


        public override void Undo()
        {
        }

        #endregion
    }
}