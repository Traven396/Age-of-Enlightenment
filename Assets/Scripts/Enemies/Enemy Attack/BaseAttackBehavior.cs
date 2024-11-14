namespace AgeOfEnlightenment.Enemies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    
    public abstract class BaseAttackBehavior : ScriptableObject
    {
        //Not sure what is necessary
        [SerializeField] public string NameOfAttack { get; }
        [SerializeField] public AnimationClip AttackAnimation;
        [SerializeField] public float EventFireTime;
        [SerializeField] public float Cooldown;

        public EnemyRange AttackRange;
        [HideInInspector] public float cooldownTimer;
        public abstract void Attack(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions);

        //Maybe some base Attack method
        //A range requirement for this attakc to be poissble
        //An animation to play for the attack
        //A chance or number that this attack will be chosen often

        //Children of this behavior will add the special characteristics to each one
        //Things like:
        //Homing
        //Mulit shot
        //Summoning entities
        //Melee
        //Create magic constructs
        //Buffind the caster
        //Debuffing the player

        public enum EnemyRange
        {
            Close,
            Medium,
            Long
        }
    }

    public struct EnemyPositions
    {

        public EnemyPositions(Transform leftHand, Transform rightHand)
        {
            EnemyRightHand = rightHand;
            EnemyLeftHand = leftHand;
        }

        public Transform EnemyLeftHand { get; }
        public Transform EnemyRightHand { get; }
    }

}