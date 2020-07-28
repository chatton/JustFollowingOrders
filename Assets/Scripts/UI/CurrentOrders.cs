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

        private void UpdateVisuals(CommandBuffer selectedCommandBuffer)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Command command in selectedCommandBuffer.Commands)
            {
                sb.AppendLine(command.ToString());
            }

            text.text = sb.ToString();
        }
    }
}