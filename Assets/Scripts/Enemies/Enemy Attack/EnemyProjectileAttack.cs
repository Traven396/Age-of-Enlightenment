namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(menuName = "MyAssets/Enemy Attacks/Projectile Attack")]
    public class EnemyProjectileAttack : BaseAttackBehavior
    {
        public GameObject ProjectilePrefab;
        public override void Attack(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions)
        {
            //Spawn projectile
            //Get shooting direction
            //Launch it and tell it where the player is
        }
    }

}