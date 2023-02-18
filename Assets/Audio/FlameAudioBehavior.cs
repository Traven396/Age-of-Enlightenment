using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlameAudioBehavior : MonoBehaviour
{
    private AudioSource audioSource;
    

    public float sizeInfluence = .05f;
    public float speedInfluence = .2f;
    public float minimumPitch = 0.8f;
    
    
    private Vector3 previousPos;
    private float startingPitch;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startingPitch = audioSource.pitch;
        previousPos = transform.position;
    }
    private void Update()
    {
        var currVel = (transform.position - previousPos) / Time.deltaTime;
        //Debug.Log(currVel);
        audioSource.pitch = startingPitch - (currVel.magnitude / 10 * speedInfluence);
        if (audioSource.pitch < minimumPitch)
            audioSource.pitch = minimumPitch;



        //if (transform.localScale.sqrMagnitude > previousSize.sqrMagnitude)
        //{
        //    //Debug.Log(Mathf.RoundToInt(transform.localScale.sqrMagnitude - previousSize.sqrMagnitude));
        //    audioSource.volume += (transform.localScale.sqrMagnitude - previousSize.sqrMagnitude * sizeInfluence) / 100;
        //}
        //else if (transform.localScale.sqrMagnitude < previousSize.sqrMagnitude)
        //{
        //    audioSource.volume -= (transform.localScale.sqrMagnitude - previousSize.sqrMagnitude * sizeInfluence) / 100;
        //}

    }
    private void LateUpdate()
    {
        previousPos = transform.position;
    }

}
