using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace AgeOfEnlightenment.Enemies
{
    public class LeaveBreadcrumbs : MonoBehaviour
    {
        [SerializeField] Breadcrumb BreadcrumbPrefab;
        [SerializeField] private int MaxPoolSize = 10;
        [SerializeField] private float CrumbSpawnCooldown = 0.8f;
        [SerializeField] private float CrumbLifetime = 1;

        private ObjectPool<Breadcrumb> BreadcrumbPool;
        public List<Breadcrumb> ActiveCrumbs { private set; get; } = new List<Breadcrumb>();

        private float breadcrumbTimer = 0;
        private void Awake()
        {
            //Initialize the pool
            BreadcrumbPool = new ObjectPool<Breadcrumb>(createFunc: OnCreateCrumb, actionOnRelease: OnReturnCrumb, actionOnGet: OnGetCrumb, actionOnDestroy: OnDestroyCrumb, maxSize: MaxPoolSize);
        }
        private Breadcrumb OnCreateCrumb()
        {
            return Instantiate(BreadcrumbPrefab); 
        }
        private void OnReturnCrumb(Breadcrumb crumb)
        {
            ActiveCrumbs.Remove(crumb);
            crumb.gameObject.SetActive(false);
        }
        private void OnGetCrumb(Breadcrumb crumb)
        {
            crumb.MaxLifetime = CrumbLifetime;
            crumb.SetPool(BreadcrumbPool);

            crumb.gameObject.SetActive(true);

            ActiveCrumbs.Insert(0, crumb);
        }
        private void OnDestroyCrumb(Breadcrumb crumb)
        {
            ActiveCrumbs.Remove(crumb);
            Destroy(crumb);
        }

        private void Update()
        {
            breadcrumbTimer += Time.deltaTime;
            if(breadcrumbTimer >= CrumbSpawnCooldown)
            {
                breadcrumbTimer = 0;

                Breadcrumb latestCrumb = BreadcrumbPool.Get();
                latestCrumb.transform.position = transform.position;
            }
        }
    }
}