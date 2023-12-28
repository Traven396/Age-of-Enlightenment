using UnityEngine;

[RequireComponent(typeof(SpellProjectile))]
public class ExplosiveProjectile : MonoBehaviour
{
    private SpellProjectile _ParentProjectile;

    [SerializeField] private float ExplosionRadius = 0;
    [SerializeField] private bool AppliesForce = false;
    [SerializeField] private float AmountOfForce = 0;

    private LayerMask _nonCollidableLayers;
    private DamageType _damageType;
    private float _damageAmount;
    private Collider[] hits = new Collider[40];

    private void Awake()
    {
        _ParentProjectile = GetComponent<SpellProjectile>();
    }
    private void Start()
    {
        _nonCollidableLayers = _ParentProjectile.GetShooter().GetNonCollidableLayers();
        _damageType = _ParentProjectile.GetShooter().GetDamageAndTyping().returnedTyping;
        _damageAmount = _ParentProjectile.GetShooter().GetDamageAndTyping().returnedAmount;
    }
    private void OnCollisionEnter(Collision collision)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(collision.GetContact(0).point, ExplosionRadius, hits, ~_nonCollidableLayers);

        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                //Debug.Log(hits[i].name);

                if (hits[i].TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(AmountOfForce, collision.GetContact(0).point, ExplosionRadius, 0, ForceMode.Force);
                }
                if (hits[i].TryGetComponent(out IDamageable target))
                {
                    Debug.Log(hits[i].gameObject + " took " + _damageAmount + " damage");
                    target.TakeDamage(_damageAmount, _damageType);
                }
            }

        }
    }


}