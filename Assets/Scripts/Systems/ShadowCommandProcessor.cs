using Commands;
using UnityEngine;
using Util;

namespace Systems
{
    public class ShadowCommandProcessor : Singleton<ShadowCommandProcessor>
    {
        public void ProcessShadowCommand(Command command)
        {
            while (!command.IsFinished())
            {
                command.Execute(Time.deltaTime);
            }
        }
    }
}