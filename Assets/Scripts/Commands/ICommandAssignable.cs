using System.Collections.Generic;

namespace Commands
{
    public interface ICommandAssignable
    {
        void AssignCommands(IEnumerable<ICommand> commands);
    }
}