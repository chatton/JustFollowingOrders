namespace Commands
{
    public class WaitCommand : ICommand
    {
        public void Execute(float _)
        {
        }

        public void Undo()
        {
        }

        public bool IsFinished()
        {
            return true;
        }
    }
}