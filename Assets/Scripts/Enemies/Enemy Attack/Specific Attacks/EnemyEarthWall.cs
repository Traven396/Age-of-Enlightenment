namespace AgeOfEnlightenment.Enemies
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemyEarthWall : EnemySummonBehavior
    {
        [Range(0, 1)]
        public float PercentageOfSelfWalls;
        public float WallSpawnDistanceMax;
        public float WallSpawnDistanceMin;
        public LayerMask GroundLayers;

        private Vector3 DirectionVectorToPlayer;
        private Transform EnemyTransform;
        private Transform PlayerTransformChild;
        public override void StartOfLife(PlayerSingleton TargetPlayer, EnemyPositions SpawnPositions)
        {
            EnemyTransform = SpawnPositions.EnemyMainBody;
            PlayerTransformChild = TargetPlayer.transform;

            DirectionVectorToPlayer = (PlayerTransformChild.position - EnemyTransform.position);
            DirectionVectorToPlayer.y = 0;
            DirectionVectorToPlayer.Normalize();


            var randomNum = Random.Range(0f, 1f);

            if (randomNum <= PercentageOfSelfWalls)
            {

                DefenseiveWall();

            }
            else
            {
                //OffensiveWall();
                DefenseiveWall();
            }

        }
        
        private void DefenseiveWall()
        {
            float randomAngle = Random.Range(-10, 10);
            Vector3 rotatedDirection = Quaternion.Euler(0, randomAngle, 0) * DirectionVectorToPlayer;
            float randomDistance = Random.Range(WallSpawnDistanceMin, WallSpawnDistanceMax);

            Vector3 preHeightDirection = rotatedDirection * randomDistance;

            Vector3 alteredSpawningDirection = EnemyTransform.position + preHeightDirection + (EnemyTransform.up * 2);
            //Take this position and transfer it to world space based on the Enemy position
            // Try and find the ground height in that location, maybe by doing a raycast down but only hitting the ground layer?
            //Take that position and normal as the direction the wall will rise


            if (Physics.Raycast(alteredSpawningDirection, -EnemyTransform.up, out RaycastHit hit, 3, GroundLayers))
            {
                FinalizeLocation(hit.point, true);
            }
            else
            {
                Debug.Log("We didnt find a proper location");     
            }



        }
        private void OffensiveWall() 
        { 

        }

        private void FinalizeLocation(Vector3 worldPosition, bool defenseWall)
        {
            transform.position = worldPosition;
            if (defenseWall)
            {
                transform.LookAt(new Vector3(EnemyTransform.position.x, transform.position.y, EnemyTransform.position.z));
                transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            { 
                transform.LookAt(new Vector3(DirectionVectorToPlayer.x, transform.position.y, DirectionVectorToPlayer.z));
                transform.rotation *= Quaternion.Euler(0, 180, 0);
            }

            transform.GetChild(0).gameObject.SetActive(true);
        }

    }

}