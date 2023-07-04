using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectBehavior : MonoBehaviour
{
    [SerializeField] private float lifeExpectancy = 5;
    [SerializeField] private float postMortemTime = 2;
    [SerializeField] private AnimationClip destroyAnimation;
    public bool waitToStartCountdown = false;

    private Animation selfAnimation;

    private void Start()
    {
        selfAnimation = GetComponent<Animation>();
        if (!waitToStartCountdown)
        {
            if (!destroyAnimation)
            {
                Debug.Log("No death animation. Just killing with no animation effects");
                Destroy(gameObject, lifeExpectancy + postMortemTime);
                return;
            }
            if (selfAnimation.clip == null)
            {
                Debug.Log("No opening clip. Killing after lifetime");
                StartDeathCountdown();
            } 
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
        else
        {
            Destroy(gameObject, lifeExpectancy + postMortemTime);
            Debug.Log("Destroying");
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
