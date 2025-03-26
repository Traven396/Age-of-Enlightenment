using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWavePiece : MonoBehaviour
{
    //When this spawns cause it to raise up
    //Once it hits max height it destroys itself
    
    public float RiseSpeed = 1;
    public float LifeTime = 1;

    private float _riseSpeed;
    private float _lifeTimer;

    private ParticleSystem _SelfParticle;

    private void Awake()
    {
        _SelfParticle = GetComponent<ParticleSystem>();

        if (!_SelfParticle)
            _SelfParticle = GetComponentInChildren<ParticleSystem>();
        _riseSpeed = RiseSpeed / 100;
    }

    private void Update()
    {
        if(_lifeTimer < LifeTime)
        {
            _lifeTimer += Time.deltaTime;
        }
        else
        {
            EndLife();
        }
    }


    private void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        transform.position = new Vector3(currentPos.x, currentPos.y + _riseSpeed, currentPos.z);
    }

    void EndLife()
    {
        if (_SelfParticle)
            _SelfParticle.Stop();
        Destroy(gameObject, 1);
    }
}
