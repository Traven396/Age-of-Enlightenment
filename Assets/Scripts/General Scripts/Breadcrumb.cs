using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AgeOfEnlightenment.Enemies
{
    public class Breadcrumb : MonoBehaviour
    {
        public float MaxLifetime;

        private float lifeTimeTimer = 0;

        private ObjectPool<Breadcrumb> parentPool;

        private void Update()
        {
            //Increment a timer for how long this crumb should stay around
            lifeTimeTimer += Time.deltaTime;

            //Once the timer is reached then we just release it back into the pool for further use
            if(lifeTimeTimer >= MaxLifetime)
            {
                parentPool.Release(this);
            }
        }
        private void OnEnable()
        { 
            lifeTimeTimer = 0;
        }
        private void OnDisable()
        {
            lifeTimeTimer = 0;
        }
        public void SetPool(ObjectPool<Breadcrumb> pool)
        {
            parentPool = pool;
        }
    } 
}
