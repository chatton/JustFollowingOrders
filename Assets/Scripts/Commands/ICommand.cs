namespace Commands
{
    public interface ICommand
    {
        void Execute(float timeDelta);
        void Undo();
        bool IsFinished();
    }
}