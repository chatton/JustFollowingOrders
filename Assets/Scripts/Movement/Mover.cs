using System;
using Commands;
using UnityEngine;
using World;

namespace Movement
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateSpeed = 100f;
        [SerializeField] private Transform groundPoint;

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

        public bool CanMoveInDirection(MoveDirection direction)
        {
            Vector3 startingPosition;
            switch (direction)
            {
                case MoveDirection.Forward:
                    startingPosition = transform.position + Vector3.forward;
                    break;
                case MoveDirection.Left:
                    startingPosition = transform.position + Vector3.left;
                    break;
                case MoveDirection.Right:
                    startingPosition = transform.position + Vector3.right;
                    break;
                case MoveDirection.Back:
                    startingPosition = transform.position + Vector3.back;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction!");
            }

            // shoot a raycast down to see if there is a tile that we can walk on
            if (Physics.Raycast(startingPosition + Vector3.up * 2, Vector3.down, out RaycastHit hit, 5f))
            {
                Tile t = hit.collider.gameObject.GetComponent<Tile>();
                return t != null;
            }

            return false;
        }
    }
}