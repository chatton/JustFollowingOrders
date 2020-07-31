using System;
using Systems;
using UnityEngine;
using Util;

namespace UI
{
    public class UiEnabler : Singleton<UiEnabler>
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private GameObject issueOrdersButton;

        private void Awake()
        {
            LevelManager.Instance.OnCommandBufferChanged += EnableUi;
            MonoCommandProcessor.Instance.OnBeginCommandProcessing += DisableUi;
        }

        private void DisableUi()
        {
            ui.SetActive(false);
        }

        private void EnableUi(CommandBuffer buffer)
        {
            ui.SetActive(true);
            issueOrdersButton.SetActive(buffer.Commands.Count > 0);
        }
    }
}