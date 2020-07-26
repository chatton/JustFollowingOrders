namespace Commands
{
    public interface IProgrammable
    {
        void MoveOntoNextCommand();
        ICommand CurrentCommand();
        bool HasNextCommand();
    }
}