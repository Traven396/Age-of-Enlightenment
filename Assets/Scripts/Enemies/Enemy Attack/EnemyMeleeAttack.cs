namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyMeleeAttack : BaseAttackBehavior
    {
        public override void Attack(PlayerSingleton TargetPlayer, Animator EnemyAnimator)
        {
            //The melee attack will just apply damage to the player if a certain collider impacts then during an animation
            
            //Play animation
            //Somehow need to get a message from the colliders to the enemy.
            //Try and find a tutorial on modular melee attacks or something.
        }

        
    } 
}
