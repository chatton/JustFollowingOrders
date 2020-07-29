using System;
using Core;
using UnityEngine;

namespace Commands
{
    public class AttackCommand : ICommand
    {
        private static readonly int Attack = Animator.StringToHash("Attacking");

        private Animator _animator;
        private Health _health;
        private Attacker _attacker;

        private bool _enemyDead;
        private bool _hasAttacked;
        private float _clipLength;
        private float _elapsedTime;
        private bool _finishedAttackAnimation;
        private bool _attackerDead;

        public AttackCommand(Animator animator, Health health, Attacker attacker)
        {
            _animator = animator;
            // _health = health;
            _attacker = attacker;
        }

        public override string ToString()
        {
            return "Attack!";
        }

        public bool CanBeExecuted()
        {
            if (_attacker.GetComponent<Health>().IsDead)
            {
                return false;
            }

            if (_hasAttacked)
            {
                return true;
            }
            return _attacker.GetTargetInRange() != null;
        }

        public void Execute(float timeDelta)
        {
            _elapsedTime += timeDelta;
            if (!_finishedAttackAnimation && _elapsedTime >= 0.5f)
            {
                _finishedAttackAnimation = true;
                Debug.Log("Setting attack animation to false!");
                _animator.SetBool(Attack, false);
                return;
            }

            if (_hasAttacked)
            {
                return;
            }

            if (_attacker.GetComponent<Health>().IsDead)
            {
                _attackerDead = true;
                return;
            }

            Debug.Log(_attacker.name + " is performing the AttackCommand");
            _hasAttacked = true;
            _animator.SetBool(Attack, true);
            _health = _attacker.GetTargetInRange();
            _health.OnDeath += OnDeath;
            _health.Hit();
        }


        private void OnDeath()
        {
            Debug.Log("OnDeath");
            _enemyDead = true;
        }

        public void Undo()
        {
            Debug.Log("Setting " + _health.name + " to active!");
            _health.gameObject.SetActive(true);
        }

        public bool IsFinished()
        {
            bool isFinished = _attackerDead || (_enemyDead && _finishedAttackAnimation);
            Debug.Log("ATTACK FINISHED: " + isFinished);
            return isFinished;
        }
    }
}