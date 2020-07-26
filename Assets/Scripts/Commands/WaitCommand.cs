namespace Commands
{
    public class WaitCommand : Command
    {
        public override void Execute(float _)
        {
        }

        public override void Undo()
        {
        }

        public override bool IsFinished()
        {
            return true;
        }
    }
}