using System;
using System.Collections.Generic;
using Systems;
using Commands;
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
            LevelManager.Instance.OnCommandChanged += EnableUi;
            MonoCommandProcessor.Instance.OnBeginCommandProcessing += DisableUi;
        }

        private void DisableUi()
        {
            ui.SetActive(false);
        }

        private void EnableUi(GameObject _, List<ICommand> commands)
        {
            ui.SetActive(true);
            issueOrdersButton.SetActive(commands.Count > 0);
        }
    }
}