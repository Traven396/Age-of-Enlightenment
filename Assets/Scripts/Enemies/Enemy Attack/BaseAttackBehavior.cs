namespace AgeOfEnlightenment.Enemies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public abstract class BaseAttackBehavior : MonoBehaviour
    {
        //Not sure what is necessary
        [SerializeField] protected string NameOfAttack;
        [SerializeField] protected float RangeOfAttack;
        [SerializeField] protected AnimationClip AttackAnimation;
        public abstract void Attack(PlayerSingleton TargetPlayer, Animator EnemyAnimator);

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


        protected enum AttackType
        {
            Ranged,
            Melee,
            Summon,
            Status
        }
    }

}