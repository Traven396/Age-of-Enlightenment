using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthFistComponent : MonoBehaviour
{
    [SerializeField]
    private float forceMultipler = 0;
    [SerializeField]
    private float range = 0;
    [SerializeField]
    private int damageAmount = 16;
    public void Punch()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1, transform.up, range);
        foreach (RaycastHit item in hits)
        {
            if (item.rigidbody)
            {
                item.rigidbody.AddExplosionForce(forceMultipler, transform.position + new Vector3(0, .3f), range, 2, ForceMode.VelocityChange);
            }
            if(item.transform.TryGetComponent(out IEntity target))
            {
                Vector3 forceToApply = item.transform.position - transform.position;
                forceToApply = forceToApply.normalized * forceMultipler;
                target.ApplyMotion(forceToApply, ForceMode.VelocityChange);
            }
            if(item.transform.TryGetComponent(out IDamageable damage))
            {
                damage.TakeDamage(damageAmount, DamageType.Earth);
            }
        }
    }
}
