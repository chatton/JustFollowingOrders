
using Systems;
using Commands;
using Core;
using Movement;
using UnityEngine;
using UnityEngine.Assertions;

public class RotatorTile : MonoBehaviour
{
    [SerializeField] private RotationDirection Direction;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Mover mover = other.gameObject.GetComponent<Mover>();
        Health health = other.gameObject.GetComponent<Health>();
        Assert.IsNotNull(mover);
        Assert.IsNotNull(health);
        MonoCommandProcessor.Instance.ExecutePriorityCommand(new RotationCommand(Direction, mover, health));
    }
}