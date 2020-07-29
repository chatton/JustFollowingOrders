using System.Collections.Generic;

namespace Commands
{
    public interface IProgrammable
    {
        void MoveOntoNextCommand();
        ICommand CurrentCommand();
        bool HasNextCommand();

        void Reset();
        IEnumerable<ICommand> Commands();

        void AssignCommands(List<ICommand> commands);
    }
}