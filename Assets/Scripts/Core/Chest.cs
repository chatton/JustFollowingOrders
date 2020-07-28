using System;
using Systems;
using UnityEngine;

namespace Core
{
    public class Chest : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int OpenHash = Animator.StringToHash("Open");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        private void Open()
        {
            _animator.SetBool(OpenHash, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && LevelManager.Instance.HasKey)
            {
                Open();
            }
        }
    }
}