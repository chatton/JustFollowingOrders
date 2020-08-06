using System;
using Commands;
using Core;
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

        public event Action OnRotationStart;
        public event Action OnRotationEnd;

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

        public bool HasReachedPosition(Vector3 position)
        {
            return transform.position == position;
        }

        public void Rotate(float yAxisAngle, float deltaTime)
        {
            Quaternion targetRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, yAxisAngle, 0),
                rotateSpeed * deltaTime);

            transform.rotation = targetRotation;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(StartPosition(MoveDirection.Right) + Vector3.up * 2f,
                StartPosition(MoveDirection.Right) - Vector3.down);
            Gizmos.DrawLine(StartPosition(MoveDirection.Left) + Vector3.up * 2f,
                StartPosition(MoveDirection.Left) - Vector3.down);
            Gizmos.DrawLine(StartPosition(MoveDirection.Back) + Vector3.up * 2f,
                StartPosition(MoveDirection.Back) - Vector3.down);
            Gizmos.DrawLine(StartPosition(MoveDirection.Forward) + Vector3.up * 2f,
                StartPosition(MoveDirection.Forward) - Vector3.down);
        }

        private Vector3 StartPosition(MoveDirection direction)
        {
            switch (direction)
            {
                case MoveDirection.Forward:
                    return transform.position + transform.forward;
                case MoveDirection.Left:
                    return transform.position + -transform.right;
                case MoveDirection.Right:
                    return transform.position + transform.right;
                case MoveDirection.Back:
                    return transform.position + -transform.forward;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction!");
            }
        }


        private void Update()
        {
            Vector3 startingPosition = StartPosition(MoveDirection.Forward);
            Debug.DrawRay(startingPosition + transform.up * 4, -transform.up * 4, Color.red);
        }

        public bool CanMoveInDirection(MoveDirection direction)
        {
            Vector3 startingPosition = StartPosition(direction);

            // shoot a raycast down to see if there is a tile that we can walk on
            if (Physics.Raycast(startingPosition + transform.up * 2, -transform.up, out RaycastHit hit, 2f,
                LayerMask.GetMask("Tile")))
            {
                Tile t = hit.collider.gameObject.GetComponent<Tile>();
                Debug.Log("CanMoveInDirection:" + t.name + " IsEmpty: " + t.IsEmpty);
                return t.IsEmpty;
            }

            return false;
        }
    }
}