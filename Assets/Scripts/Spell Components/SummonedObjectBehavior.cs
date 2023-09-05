using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectBehavior : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5;
    [SerializeField] private bool _destroyAfterLastAnimation = true;

    [Space(15f)]

    [SerializeField] private AnimationClip _spawnAnimation;
    [SerializeField] private AnimationClip _destroyAnimation;

    private Animation selfAnimator;

    private void OnEnable()
    {
        selfAnimator = GetComponent<Animation>();
    }

    public void BeginLifeCycle() 
    {
        if (selfAnimator != null && _spawnAnimation != null)
        {
            selfAnimator.clip = _spawnAnimation;
            selfAnimator.Play();

            Invoke(nameof(MiddleLifeCycle), _spawnAnimation.length);
        }
        else
            MiddleLifeCycle();
    }

    private void MiddleLifeCycle()
    {
        Invoke(nameof(EndCycle), _lifeTime);
    }

    private void EndCycle()
    {
        if (selfAnimator != null && _destroyAnimation != null)
        {

            selfAnimator.clip = _destroyAnimation;
            selfAnimator.Play();

            Destroy(gameObject, _destroyAnimation.length);
        }
        else
            Destroy(gameObject);
    }
}
