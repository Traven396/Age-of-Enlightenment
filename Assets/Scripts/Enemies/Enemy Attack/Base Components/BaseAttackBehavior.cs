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
        public string NameOfAttack;
        public AnimationClip AttackAnimation;
        public float EventFireTime;
        public float Cooldown;

        public EnemyRange AttackRange;
        [Space(10)]
        public AttackTiming TimingOfAttack;
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

        
    }

    public enum EnemyRange
    {
        Close,
        Medium,
        Long
    }
    public enum AttackTiming
    {
        Proactive,
        Reactive
    }

    public struct EnemyPositions
    {

        public EnemyPositions(Transform leftHand, Transform rightHand, Transform enemy)
        {
            EnemyRightHand = rightHand;
            EnemyLeftHand = leftHand;
            EnemyMainBody = enemy;
        }

        public Transform EnemyLeftHand { get; }
        public Transform EnemyRightHand { get; }
        public Transform EnemyMainBody { get; }
    }

}