namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(menuName = "MyAssets/Enemy Attacks/Summon Attack")]
    public class EnemySummonAttack : BaseAttackBehavior
    {
        public GameObject SummonObject;

        protected EnemySummonBehavior lastSummonedObject; 
        public override void Attack(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions)
        {
            //This will need to have something to summon, and then just create it and tell it to be destroyed after a while?
            lastSummonedObject = Instantiate(SummonObject, SpawnPositions.EnemyMainBody.position, Quaternion.identity).GetComponent<EnemySummonBehavior>();
            lastSummonedObject.StartOfLife(TargetPlayer, SpawnPositions);
        }

    }

}