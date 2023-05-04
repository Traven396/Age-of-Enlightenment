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
        if (!destroyAnimation)
        {
            Debug.Log("No death animation. Just killing");
            Destroy(gameObject, lifeExpectancy + postMortemTime);
        }
        if(selfAnimation.clip == null)
        {
            Debug.Log("No opening clip. Killing after life");
            StartDeathCountdown();
        }
    }
    public void StartDeathCountdown()
    {
        Invoke(nameof(PlayDeathAnimation), lifeExpectancy);

        if (destroyAnimation)
        {
            Destroy(this.gameObject, lifeExpectancy + destroyAnimation.length + postMortemTime);
            Debug.Log("Killing after " + (lifeExpectancy + destroyAnimation.length + postMortemTime) + " seconds");
        }

    }
    private void PlayDeathAnimation()
    {
        if (selfAnimation != null)
        {
            selfAnimation.clip = destroyAnimation;
            selfAnimation.Play(); 
        }
    }
}
