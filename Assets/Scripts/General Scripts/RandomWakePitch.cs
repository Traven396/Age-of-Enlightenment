using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWakePitch : MonoBehaviour
{
    [SerializeField] private float startingPitch = .1f;
    [SerializeField] private float maxPitchRange = .1f;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        _source.playOnAwake = false;
        _source.pitch = startingPitch + Random.Range(-maxPitchRange, maxPitchRange);
        _source.PlayOneShot(_source.clip);
    }
}
