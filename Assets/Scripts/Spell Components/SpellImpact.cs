using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpellImpact : MonoBehaviour
{
    public enum ImpactType
    {
        Animation,
        ParticleSystem,
        Tween
    }
    public enum TweenType
    {
        Shrink,
        Grow,
        FadeOut,
        FadeIn,
        Sink,
        Rise
    }
    [SerializeField] public bool AutoCalculateLifetime = true;
    [SerializeField] public ImpactType TypeOfImpact;

    [SerializeField] public TweenType StartTween, EndTween;
    [SerializeField] public float StartTweenSpeed = 0, EndTweenSpeed = 0;


    private ObjectPool<SpellImpact> _pool;
    private ParticleSystem[] _particleSystems;
    private AudioSource _audioSource;

    private float _scale = 1;
    [SerializeField] public float _maxLifetime = 0;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _particleSystems = GetComponentsInChildren<ParticleSystem>();

        if (AutoCalculateLifetime)
        {
            if (TypeOfImpact == ImpactType.ParticleSystem)
            {
                _maxLifetime = _particleSystems.Max(ps => ps.main.duration + ps.main.startLifetime.constantMax);  
            }
            if(TypeOfImpact == ImpactType.Tween)
            {
                _maxLifetime = StartTweenSpeed + EndTweenSpeed;
            }
        }
        else
        {
            if(TypeOfImpact == ImpactType.Tween)
            {
                _maxLifetime += StartTweenSpeed + EndTweenSpeed;
            }
        }
    }
    public void FinishedLoading()
    {
        transform.localScale = Vector3.one * _scale;

        if (_audioSource)
            _audioSource.PlayOneShot(_audioSource.clip);
        switch (TypeOfImpact)
        {
            case ImpactType.ParticleSystem:
                    Invoke(nameof(DestroyEffect), _maxLifetime);
                break;
            case ImpactType.Animation:
                    Invoke(nameof(DestroyEffect), _maxLifetime);
                break;
            case ImpactType.Tween:
                    StartingTweenMethod();
                break;
        }
        
    }

    private void StartingTweenMethod()
    {
        HandleTween(StartTween, StartTweenSpeed, nameof(EndingTweenMethod));
    }
    private void EndingTweenMethod()
    {
        HandleTween(EndTween, EndTweenSpeed, nameof(DestroyEffect));
    }
    private void HandleTween(TweenType WhichTween, float Timing, string EndingMethod)
    {
        switch (WhichTween)
        {
            case TweenType.Shrink:
                iTween.ScaleTo(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "scale", Vector3.zero));
                break;
            case TweenType.Grow:
                iTween.ScaleFrom(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "scale", Vector3.zero));
                break;
            case TweenType.FadeOut:
                iTween.FadeTo(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "alpha", 0));
                break;
            case TweenType.FadeIn:
                iTween.FadeFrom(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "alpha", 0));
                break;
            case TweenType.Sink:
                Debug.Log("Sinking: " + transform.TransformPoint(0, -1, 0));
                iTween.MoveTo(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "islocal", true, "position", transform.TransformPoint(0, -1, 0)));
                break;
            case TweenType.Rise:
                Debug.Log("Rising: " + transform.TransformPoint(0, -1, 0));
                iTween.MoveFrom(gameObject, iTween.Hash("time", Timing, "oncomplete", EndingMethod, "oncompletetarget", gameObject, "islocal", true, "position", transform.TransformPoint(0, -1, 0)));
                break;
        }
    }

    public void SetPool(ObjectPool<SpellImpact> newPool)
    {
        _pool = newPool;
    }
    public void SetScale(float newScale)
    {
        _scale = newScale;
    }
    
    private void DestroyEffect()
    {
        _pool.Release(this);
    }
}
