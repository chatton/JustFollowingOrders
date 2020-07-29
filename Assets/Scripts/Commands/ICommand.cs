using UnityEngine;

namespace Commands
{
    public interface ICommand
    {
        bool CanBeExecuted();
        void Execute(float timeDelta);
        void Undo();
        bool IsFinished();
    }
}