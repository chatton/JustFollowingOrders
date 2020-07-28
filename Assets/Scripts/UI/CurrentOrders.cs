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
        }

        private string BuildCommandString(Command[] commands)
        {
            List<List<Command>> commandsByType = new List<List<Command>>();

            List<Command> tmp = new List<Command>();
            commandsByType.Add(tmp);
            Command prev = null;
            foreach (Command c in commands)
            {
                if (prev != null)
                {
                    if (prev.ToString() == c.ToString())
                    {
                        tmp.Add(c);
                    }
                    else
                    {
                        tmp = new List<Command> {c};
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

            foreach (List<Command> commandList in commandsByType)
            {
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