using System;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class Health : MonoBehaviour
    {
        private static readonly int Dead = Animator.StringToHash("Dead");

        // for now, everything dies in one hit
        public event Action OnDeath;
        public event Action OnHit;

        private Animator _animator;
        public bool IsDead { get; set; }


        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void Hit()
        {
            IsDead = true;
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            OnHit?.Invoke();
            // wait a little bit so we don't die as soon as the attack animation starts
            yield return new WaitForSeconds(0.5f);
            _animator.SetBool(Dead, true);
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
            OnDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }
}