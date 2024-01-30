using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Enemies
{
    public static class AiManager
    {
        //Lists of objects that can be targetted
        private static List<Breadcrumb> _BreadcrumbList;

        static public string PlayerTagString = "Player",
							 EnemyTagString = "Enemy";
        private static RaycastHit hit;

        private static Transform MainPlayer;
        private static LeaveBreadcrumbs BreadcrumbMaker;


		public static void AI_Layer_Setup(this NewEnemyAI ai)
		{
			ai.PlayerLayer = (1 << LayerMask.NameToLayer(PlayerTagString));
			ai.EnemyLayer = (1 << LayerMask.NameToLayer(EnemyTagString));
			//ai.breadcrumbLayer = (1 << LayerMask.NameToLayer(breadcrumbString));
		}
        public static void General_AI_Setup(this NewEnemyAI ai)
        {
            if (!MainPlayer)
            {
                MainPlayer = Object.FindObjectOfType<FrankensteinCharacterController>().transform; 
            }
            if (!BreadcrumbMaker)
            {
                BreadcrumbMaker = MainPlayer.GetComponent<LeaveBreadcrumbs>();
            }
            ai.PlayerBreadcrumbs = BreadcrumbMaker;
        }
        public static void BreadcrumbSearch()
        {
            _BreadcrumbList = BreadcrumbMaker.ActiveCrumbs;
        }
		public static Transform FindPlayer(this NewEnemyAI ai) 
		{
            Debug.DrawLine(ai.transform.position, MainPlayer.position, Color.green);

            if (InVisionRange(ai, MainPlayer.position))
            {
                //If I decide to implement breadcrumbs and such, I need to put their layers here. Use ~(layer1 | layer2)
                //This just sees if there is anything in the way to the player. If not, we dont change anything
                if (Physics.Linecast(MainPlayer.position, ai.transform.position, out hit, ~ (ai.EnemyLayer | ai.PlayerLayer)))
                {
                    //So with the new method. If we have hit something, that means there is something in the way between the player and the enemy.
                    //So we set the target to null
                    ai._Player = null;
                }
                else
                {
                    //If the Linecast returns false on the entire path from the enemy to the player, ignoring the enemy and the player, then that means we have line of sight
                    ai._Player = MainPlayer;
                    ai._PreviousPlayer = MainPlayer;
                }
            } else {
                //If the current target is outside of the vision range, then clear it
                ai._Player = null;
            }

            return ai._Player;
        }
        public static Transform FindBreadcrumb(this NewEnemyAI ai)
        {
            //Update the list with all current breadcrumbs.
            BreadcrumbSearch();

            //If we already have a breadcrumb found, then we just verify its still valid
            if (ai._TargetBreadcrumb) { 
                //Check to see that the enemy still has line of sight on the current breadcrumb
                if(InVisionRange(ai, ai._TargetBreadcrumb.position))
                {
                    //Send a line from the enemy to the target breadcrumb. This is to get information on what we are looking at. It should be impossible for this to return false.
                    //but if somehow it does, we set the target to null
                    if(Physics.Linecast(ai.transform.position, ai._TargetBreadcrumb.transform.position, out hit, ~(ai.EnemyLayer), QueryTriggerInteraction.Collide))
                    {
                        //Did we hit an actual breadcrumb with the linecast? IE, a different one in the way
                        if (hit.collider.TryGetComponent<Breadcrumb>(out _))
                        {
                            //Is the Breadcrumb that we hit the same as our target?
                            if (hit.collider.gameObject == ai._TargetBreadcrumb)
                            {
                                //Have we already reached the Breadcrumb we are trying to?
                                if (Vector3.Distance(ai.transform.position, ai._TargetBreadcrumb.position) > 0.5f)
                                {
                                    //If this spot is reached then all of these statements are true
                                    //1. We have a target breadcrumb
                                    //2. The target is within our vision range
                                    //3. The object we are looking at has the Breadcrumb component
                                    //4. The object we hit is the same as our current target
                                    //5. We are not within 0.5 units of the target already
                                    return ai._TargetBreadcrumb;
                                }
                            }
                            else
                            {
                                //This is the case for if somehow another breadcrumb is now in the way of the enemy. One that is closer to them

                                //We set the new target, then return out of here
                                ai._TargetBreadcrumb = hit.transform;
                                return ai._TargetBreadcrumb;
                            }
                        }
                    }
                }
                //This is the catch all statement. If any of these tests failed in a way that matters, then that means we have lost sight of the target
                ai._TargetBreadcrumb = null;
            }

            float distance = Mathf.Infinity;
            if(_BreadcrumbList != null)
            {
                bool CrumbWithinRange = false;
                foreach(Breadcrumb crumb in _BreadcrumbList)
                {
                    //Check for each crumb we know of, seeing if it is within range and sight of the enemy
                    if(crumb && InVisionRange(ai, crumb.transform.position))
                    {
                        if(Physics.Linecast(ai.transform.position, crumb.transform.position, out hit)){
                            if (hit.collider.TryGetComponent<Breadcrumb>(out _))
                            {
                                Vector3 diff = crumb.transform.position - ai._PreviousPlayer.position;
                                float miniDistance = diff.sqrMagnitude;
                                if(miniDistance < distance)
                                {
                                    ai._TargetBreadcrumb = crumb.transform;
                                    distance = miniDistance;
                                    CrumbWithinRange = true;
                                }
                            }
                        }
                    }
                }
                if(!CrumbWithinRange)
                {
                    ai._PreviousPlayer = null;
                }
            }
            return ai._TargetBreadcrumb;
        }
        public static Transform FindOtherAI(this NewEnemyAI ai)
        {
            //Do we already have a target we are following?
            if (ai._TargetAI)
            {
                //Are they within our vision range currently?
                if(InVisionRange(ai, ai._TargetAI.position))
                {
                    //Can we still see the target? This shouldnt return false ever. But programming be like
                    if(Physics.Linecast(ai.transform.position, ai._TargetAI.position, out hit)) 
                    { 
                        //Is the thing we are hitting an enemy?
                        if(hit.collider.TryGetComponent<NewEnemyAI>(out var hitEnemy))
                        {
                            //Is our target still tracking the player?
                            if(hitEnemy.visionState == NewEnemyAI.VISION_STATE.CanSeePlayer || hitEnemy.visionState == NewEnemyAI.VISION_STATE.CanSeeBreadcrumb)
                            {
                                Debug.Log("My friend can see them!");
                                return ai._TargetAI;
                            }
                        }
                    }
                }
                ai._TargetAI = null;
            } else
            {
                float distance = Mathf.Infinity;
                float radius = ai.VisionRange;

                //If the enemy doesnt use sight for some reason, then just check everything within 500 units
                if (!ai.HasVision)
                {
                    radius = 500;
                }
                //We do an overlap sphere around the enemy, only searching for other enemies
                Collider[] hitColliders = Physics.OverlapSphere(ai.transform.position, radius, ai.EnemyLayer);
                
                //Now we cycle through all our hit targets
                foreach (Collider collider in hitColliders)
                {
                    //Does our target have the enemy script?
                    if(collider.TryGetComponent<NewEnemyAI>(out var hitEnemy))
                    {
                        //Do they see the player?
                        if(hitEnemy.visionState == NewEnemyAI.VISION_STATE.CanSeePlayer || hitEnemy.visionState == NewEnemyAI.VISION_STATE.CanSeeBreadcrumb)
                        {
                            //Can we actually see this enemy we are checking?
                            if(!Physics.Linecast(ai.transform.position, collider.transform.position, out hit, ~(ai.EnemyLayer)))
                            {
                                //Do a basic algorithm to check if the current target is closer than the currently set one
                                float diff = (collider.transform.position - ai.transform.position).sqrMagnitude;
                                if(diff < distance)
                                {
                                    ai._TargetAI = collider.transform;
                                    distance = diff;
                                }
                            }
                        }
                    }
                }
            }

            return ai._TargetAI;
        }
		public static bool InVisionRange(NewEnemyAI AI, Vector3 position)
        {
			//If the enemy doesnt use vision, just return true
            if (AI.HasVision) {
				//Checks if the point is less than the vision range distance
				if(Vector3.Distance(AI.transform.position, position) < AI.VisionRange) {
                    //Does the AI only see forward? If not, then return true
                    if (AI.OnlyFrontVision) {
                        //Calculates if the direction from the AI to the Target is within the provided range.
                        //If it is directly in front of the AI it will return 1, perpendicular is 0
                        float visionAngle = Vector3.Dot((position - AI.transform.position).normalized, AI.transform.forward);
                        if(visionAngle > AI.FrontVisionRange) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        return true;
                    }
                }
                else {
					return false;
                }
            }
            else {
				return true;
            }
        }
	}
}
