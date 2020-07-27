using UnityEngine;

namespace Commands
{
    public abstract class Command : MonoBehaviour
    {
        public abstract void BeforeConsecutiveCommands();
        public abstract void AfterConsecutiveCommands();

        public abstract bool CanPerformCommand();
        
        public abstract void Execute(float timeDelta);
        public abstract void Undo();
        public abstract bool IsFinished();
    }
}