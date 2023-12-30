using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.StabbingPhysics;

public class RepeaterBlade : MonoBehaviour
{
    StabbingObject _selfStabber;
    List<Collider> _allSelfColliders;
    public Rigidbody rb { get; private set; }

    public bool recalling { get; private set; } = false;
    public bool launched { get; private set; } = false;
    [HideInInspector] public Transform recallPoint;

    private void Awake()
    {
        _allSelfColliders = new List<Collider>();

        _selfStabber = GetComponent<StabbingObject>();

        _allSelfColliders.AddRange(GetComponents<Collider>());
        _allSelfColliders.AddRange(GetComponentsInChildren<Collider>());

        rb = GetComponent<Rigidbody>();
    }

    public void RecallBlade()
    {
        _allSelfColliders.ForEach(col => col.enabled = false);
        _selfStabber.ForceUnstab();

        recalling = true;
    }

    public void Launch()
    {

        launched = true;
    }
}
