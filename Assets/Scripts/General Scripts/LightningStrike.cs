using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LightningStrike : MonoBehaviour
{
    public VisualEffect lightningPrefab;
    public GameObject impactPrefab;
    private Ray strikeRay;

    private RaycastHit hitInfo;
    public LayerMask hittableLayers;

    public int damageAmount;
    private void Start()
    {
        strikeRay = new Ray(transform.position, Vector3.down);
        EntireLifetime();
    }
    private void EntireLifetime()
    {
        var yOffset = 4f - Random.Range(0, .3f);
        if (Physics.Raycast(transform.position + new Vector3(0, .1f, 0), Vector3.up, out RaycastHit smallHit, 4))
        {
            yOffset = smallHit.point.y - transform.position.y;
        }

        iTween.MoveBy(gameObject, iTween.Hash("y", yOffset,
                                                "oncompletetarget", gameObject,
                                                "oncomplete", "Strike",
                                                "time", .7f));
        iTween.ScaleTo(gameObject, Vector3.one * 5f, 1f);

    }
    public void Strike()
    {
        if(Physics.Raycast(strikeRay, out hitInfo))
        {
            var number = transform.position.y - hitInfo.point.y;
            lightningPrefab.transform.localScale = new Vector3(1, 1, number / 5);
            lightningPrefab.Play();

            Invoke(nameof(ImpactFX), .1f);

            Collider[] hits = Physics.OverlapSphere(hitInfo.point, 2, hittableLayers);
            foreach(Collider col in hits)
            {
                if(col.gameObject.TryGetComponent(out IDamageable entity))
                {
                    entity.TakeDamage(damageAmount, DamageType.Lightning);
                }
                if(col.gameObject.TryGetComponent(out IEntity rbEntity))
                {
                    rbEntity.ApplyMotion((col.transform.position - hitInfo.point).normalized * 4, ForceMode.VelocityChange);
                }
                else if(col.attachedRigidbody != null)
                {
                    col.attachedRigidbody.AddExplosionForce(10, hitInfo.point, 2, 4, ForceMode.VelocityChange);
                }
            }
        }
        Destroy(gameObject, 3);
    }
    private void ImpactFX()
    {
        var thingy = Instantiate(impactPrefab, hitInfo.point, Quaternion.identity, null);
        Destroy(thingy, 3);
    }
}
