using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Health : MonoBehaviour
    {
        // for now, everything dies in one hit
        public event Action OnDeath;

        public void Hit()
        {
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            yield return new WaitForSeconds(1.5f);
            OnDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }
}