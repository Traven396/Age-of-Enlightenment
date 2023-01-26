using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectBehavior : MonoBehaviour
{
    [SerializeField] private float lifeExpectancy = 5;
    [SerializeField] private float postMortemTime = 2;
    [SerializeField] private AnimationClip destroyAnimation;

    private Animation selfAnimation;

    private void Start()
    {
        selfAnimation = GetComponent<Animation>();
    }
    public void DeathThroes()
    {
        Invoke(nameof(PlayDeathAnimation), lifeExpectancy);
        Destroy(this.gameObject, lifeExpectancy + destroyAnimation.length + postMortemTime);
    }
    private void PlayDeathAnimation()
    {
        selfAnimation.clip = destroyAnimation;
        selfAnimation.Play();
    }
}
