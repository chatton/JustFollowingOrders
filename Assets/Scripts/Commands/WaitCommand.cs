using System.Collections;
using UnityEngine;

namespace Commands
{
    public class WaitCommand : Command
    {
        private bool _isFinished;

        public override bool IsFinished()
        {
            return true;
            // return _isFinished;
        }

        public override void Execute(float _)
        {
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

        public override bool CanPerformCommand()
        {
            return true;
        }


        public override void Undo()
        {
        }

        #endregion
    }
}