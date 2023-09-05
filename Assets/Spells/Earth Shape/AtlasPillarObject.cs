using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasPillarObject : MonoBehaviour, IDamageable
{
    public float health;

    private SkinnedMeshRenderer rend;
    private MeshCollider col;
    private float blendAmount = 0;
    private bool isRaising = false;
    public ParticleSystem ps;

    private void Start()
    {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        col = GetComponentInChildren<MeshCollider>();
    }
    public void TakeDamage(float DamageAmount, DamageType elementalDamage)
    {
        health -= DamageAmount;
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (isRaising)
        {
            rend.SetBlendShapeWeight(0, blendAmount);
            Mesh bakeMesh = new Mesh();
            rend.BakeMesh(bakeMesh);
            col.sharedMesh = bakeMesh;
        }
    }

    public void SetBlendAmount(float newAmount)
    {
        blendAmount = newAmount;
    }
    public void SetRaising(bool YN)
    {
        isRaising = YN;
        if (!YN)
        {
            ps.Stop();
        }
    }
}
