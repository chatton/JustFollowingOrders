using System.Collections.Generic;

namespace Commands
{
    public interface IProgrammable
    {
        void MoveOntoNextCommand();
        Command CurrentCommand();
        bool HasNextCommand();

        bool OnLastCommand();

        bool HasCompletedAllCommands();
        // void AddCommand(Command command);
        IEnumerable<Command> Commands();
    }
}