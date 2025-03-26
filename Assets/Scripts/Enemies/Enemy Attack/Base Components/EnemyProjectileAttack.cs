namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(menuName = "MyAssets/Enemy Attacks/Projectile Attack")]
    public class EnemyProjectileAttack : BaseAttackBehavior
    {
        public GameObject ProjectilePrefab;
        public float LaunchSpeed;
        public override void Attack(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions)
        {
            EnemyProjectileBehavior projectileInstance = Instantiate(ProjectilePrefab, SpawnPositions.EnemyRightHand.position, Quaternion.identity).GetComponent<EnemyProjectileBehavior>();
            Vector3 shootingDirection = TargetPlayer.transform.position - SpawnPositions.EnemyRightHand.position;

            projectileInstance.Launch(TargetPlayer.transform, shootingDirection, LaunchSpeed);
            //Spawn projectile
            //Get shooting direction
            //Launch it and tell it where the player is
        }
    }

}