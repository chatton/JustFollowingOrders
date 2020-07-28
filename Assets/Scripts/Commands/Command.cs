using UnityEngine;

namespace Commands
{
    public abstract class Command : MonoBehaviour
    {
        public abstract void BeforeConsecutiveCommands();
        public abstract void AfterConsecutiveCommands();

        protected abstract bool DoCanPerformCommand();

        public bool CanBePerformed()
        {
            return !_skip && DoCanPerformCommand();
        }

        public abstract void Execute(float timeDelta);
        public abstract void Undo();
        public abstract bool IsFinished();

        private bool _skip = false;

        public void Skip()
        {
            _skip = true;
        }

        public bool WasSkipped()
        {
            return _skip;
        }
    }
}