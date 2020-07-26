using UnityEngine;
using Util;

namespace Systems
{
    public class MouseController : Singleton<MouseController>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U) && !CommandProcessor.Instance.IsProcessingCommands)
            {
                CommandProcessor.Instance.Undo();
            }
        }
    }
}