using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Health : MonoBehaviour
    {
        private static readonly int Dead = Animator.StringToHash("Dead");

        // for now, everything dies in one hit
        public event Action OnDeath;

        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void Hit()
        {
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            // wait a little bit so we don't die as soon as the attack animation starts
            yield return new WaitForSeconds(0.5f);
            _animator.SetBool(Dead, true);
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            OnDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }
}