using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public LayerMask TargetableLayers;
    public float SearchRadius;
    public float HomingStrength;
    public bool RequireLineOfSight = false;

    private SphereCollider _searchCollider;
    private Rigidbody _rb;
    private List<Transform> potentialTargets = new List<Transform>();
    private float sqrSearchDistance;


    private void Awake()
    {
        _searchCollider = gameObject.AddComponent<SphereCollider>();
        _searchCollider.isTrigger = true;
        _searchCollider.radius = SearchRadius;

        sqrSearchDistance = SearchRadius * SearchRadius;

        _rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (_rb != null)
        {
            if (potentialTargets.Count != 0)
            {
                //var homingVector = (potentialTargets[0].position - _rb.position).normalized * (HomingStrength + (sqrSearchDistance - (potentialTargets[0].position - _rb.position).sqrMagnitude));
                //var homingVector = ((potentialTargets[0].position - _rb.position).normalized * HomingStrength) / (potentialTargets[0].position - _rb.position).magnitude;
                var homingVector = (potentialTargets[0].position - _rb.position).normalized * HomingStrength;

                //if ((potentialTargets[0].position - _rb.position).sqrMagnitude <= sqrSearchDistance / 3)
                //{
                //    Debug.Log("Bigger homing");
                //    homingVector *= 5;
                //}

                _rb.AddForce(homingVector, ForceMode.Acceleration);

            } 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((TargetableLayers & (1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.TryGetComponent<IEntity>(out _))
            {
                potentialTargets.Add(other.transform);
                SortTargets();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (potentialTargets.Contains(other.gameObject.transform))
        {
            potentialTargets.Remove(other.gameObject.transform);
            SortTargets();
        }
    }


    private void SortTargets()
    {
        for (int i = potentialTargets.Count - 1; i >= 0; i--)
        {
            if (!potentialTargets[i])
            {
                potentialTargets.RemoveAt(i);
            }
        }
        potentialTargets = potentialTargets.OrderBy((d) => (d.position - transform.position).sqrMagnitude).ToList();
    }
}
