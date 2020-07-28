using System.Collections;
using UnityEngine;

namespace Commands
{
    public class WaitCommand : Command
    {
        private bool _isFinished;
        private float _elapsedTime;
        private float _finishedAfter = 0.5f;

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

        private IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _isFinished = true;
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
            return true;
        }


        public override void Undo()
        {
        }

        #endregion
    }
}