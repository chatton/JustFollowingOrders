using System;
using Core;
using UnityEngine;

namespace Commands
{
    public class AttackCommand : Command
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

        private void Awake()
        {
            _animator = transform.parent.GetComponentInChildren<Animator>();
            _attacker = GetComponentInParent<Attacker>();
        }

        public override string ToString()
        {
            return "Attack!";
        }

        protected override bool DoCanPerformCommand()
        {
            if (_attacker.GetComponent<Health>().IsDead)
            {
                return false;
            }
            Debug.Log(_attacker.GetTargetInRange());

            return _attacker.GetTargetInRange() != null;
        }

        public override void Execute(float timeDelta)
        {
            _elapsedTime += timeDelta;
            // if (!_finishedAttackAnimation && _elapsedTime >= _animator.GetCurrentAnimatorStateInfo(0).length)
            if (!_finishedAttackAnimation && _elapsedTime >= 0.5f)
            {
                _finishedAttackAnimation = true;
                Debug.Log("Setting value to false!");
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

            Debug.Log(gameObject.transform.parent.name + " is performing the AttackCommand");
            _hasAttacked = true;
            _animator.SetBool(Attack, true);
            _health = _attacker.GetTargetInRange();
            _health.OnDeath += OnDeath;
            _health.Hit();
        }


        private void OnDeath()
        {
            _enemyDead = true;
        }

        public override void Undo()
        {
            _health.gameObject.SetActive(true);
        }

        public override bool IsFinished()
        {
            return _attackerDead || (_enemyDead && _finishedAttackAnimation);
        }

        #region EmptyOverrides

        public override void BeforeConsecutiveCommands()
        {
        }

        public override void AfterConsecutiveCommands()
        {
        }

        #endregion
    }
}