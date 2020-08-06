using Core;
using World;

namespace Commands
{
    public class WaitCommand : ICommand
    {
        private bool _isFinished;
        private float _elapsedTime;
        private float _finishedAfter = 0.1f;
        private Health _health;


        public WaitCommand(Health health)
        {
            _health = health;
        }

        public bool IsFinished()
        {
            if (_isFinished)
            {
                _isFinished = false;
                return true;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "Wait";
        }

        public bool CanBeExecuted()
        {
            return !_health.IsDead;
        }

        public void Execute(float deltaTime)
        {
            if (_isFinished)
            {
                return;
            }

            _elapsedTime += deltaTime;
            if (_elapsedTime >= _finishedAfter)
            {
                _isFinished = true;
                _elapsedTime = 0;
            }
        }


        public void Undo()
        {
        }
    }
}