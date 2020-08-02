using System;
using Systems;
using UnityEngine;

namespace Core
{
    public class Key : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 100f;

        void Update()
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            LevelManager.Instance.HasKey = true;
            gameObject.SetActive(false);
        }
    }
}