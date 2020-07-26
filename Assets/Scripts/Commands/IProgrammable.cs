namespace Commands
{
    public interface IProgrammable
    {
        void MoveOntoNextCommand();
        Command CurrentCommand();
        bool HasNextCommand();


        bool HasCompletedAllCommands();
        // void AddCommand(Command command);
    }
}