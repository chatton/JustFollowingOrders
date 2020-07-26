using UnityEngine;

namespace Commands
{
    public abstract class Command : MonoBehaviour
    {
        public abstract void Execute(float timeDelta);
        public abstract void Undo();
        public abstract bool IsFinished();
    }
}