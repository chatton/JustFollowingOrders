using System;
using UnityEngine;

namespace Movement
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateSpeed = 100f;

        public event Action OnMove;
        public event Action OnStop;


        public void MoveTowards(Vector3 targetPosition, float deltaTme)
        {
            Vector3 currentPosition = transform.position;

            if (currentPosition == targetPosition)
            {
                OnStop?.Invoke();
                return;
            }
            
            OnMove?.Invoke();

            Vector3 newPosition = Vector3.MoveTowards(currentPosition, targetPosition, deltaTme * moveSpeed);
            transform.position = newPosition;

            bool closeEnough = Vector3.Distance(targetPosition, currentPosition) <= 0.01f;
            if (closeEnough)
            {
                transform.position = targetPosition;
                OnStop?.Invoke();
            }
        }

        public void Rotate(float yAxisAngle, float deltaTime)
        {
            Quaternion targetRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, yAxisAngle, 0),
                rotateSpeed * deltaTime);
            transform.rotation = targetRotation;
        }
    }
}