using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonedObjectBehavior : MonoBehaviour
{
    [SerializeField] private float lifeExpectancy = 5;
    [SerializeField] private AnimationClip destroyAnimation;

    private Animation selfAnimation;

    private void Start()
    {
        selfAnimation = GetComponent<Animation>();
    }
    public void DeathThroes()
    {
        Invoke(nameof(PlayDeathAnimation), lifeExpectancy);
        Debug.Log("I SHOT YOU, YOU ALL SAW IT");
        Destroy(this.gameObject, lifeExpectancy + destroyAnimation.length);
    }
    private void PlayDeathAnimation()
    {
        Debug.Log("Tis but a flesh wound");
        selfAnimation.clip = destroyAnimation;
        selfAnimation.Play();
    }
}
