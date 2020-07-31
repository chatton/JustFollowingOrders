using System.Collections.Generic;
using System.Text;
using Systems;
using Commands;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CurrentOrders : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Awake()
        {
            text.text = "No Orders";
            LevelManager.Instance.OnCommandBufferChanged += UpdateVisuals;
            MonoCommandProcessor.Instance.OnSkipCommand += HandleSkippedCommand;
        }


        private void HandleSkippedCommand(ICommand skippedCommand)
        {
            Debug.Log("Skipped the command: " + skippedCommand);
        }

        private string BuildCommandString(ICommand[] commands)
        {
            List<List<ICommand>> commandsByType = new List<List<ICommand>>();

            List<ICommand> tmp = new List<ICommand>();
            commandsByType.Add(tmp);
            ICommand prev = null;
            foreach (ICommand c in commands)
            {
                if (prev != null)
                {
                    if (prev.ToString() == c.ToString())
                    {
                        tmp.Add(c);
                    }
                    else
                    {
                        tmp = new List<ICommand> {c};
                        commandsByType.Add(tmp);
                    }
                }
                else
                {
                    tmp.Add(c);
                }

                prev = c;
            }

            StringBuilder sb = new StringBuilder();
            if (commandsByType.Count == 0)
            {
                return "";
            }

            foreach (List<ICommand> commandList in commandsByType)
            {
                if (commandList.Count == 0)
                {
                    continue;
                }

                if (commandList.Count == 1)
                {
                    sb.AppendLine(commandList[0].ToString());
                }
                else
                {
                    sb.AppendLine(commandList[0] + " x" + commandList.Count);
                }
            }


            return sb.ToString();
        }

        private void UpdateVisuals(CommandBuffer selectedCommandBuffer)
        {
            text.text = BuildCommandString(selectedCommandBuffer.Commands.ToArray());
        }
    }
}