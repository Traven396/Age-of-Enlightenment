using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgeOfEnlightenment.StabbingPhysics;

public class RepeaterBlade : MonoBehaviour
{
    public float RecallSpeed = 5;
    public float LaunchSpeed = 5f;
    public float SnapDistance = 1;


    StabbingObject _selfStabber;
    DontGoThroughThings _selfPhaser;
    List<Collider> _allSelfColliders;
    public Rigidbody rb { get; private set; }

    [SerializeField] public bool recalling  = false;
    [SerializeField] public bool launched  = false;
    [HideInInspector] public Transform recallPoint;

    private void Awake()
    {
        _allSelfColliders = new List<Collider>();

        _selfStabber = GetComponent<StabbingObject>();

        _selfPhaser = GetComponent<DontGoThroughThings>();
        _selfPhaser.ChangeEnabled(false);

        _allSelfColliders.AddRange(GetComponents<Collider>());
        _allSelfColliders.AddRange(GetComponentsInChildren<Collider>());

        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //Debug.Log(recallPoint.position);
    }

    private void FixedUpdate()
    {
        if (recalling)
        {
            Vector3 recallVector = (recallPoint.position - transform.position);
            rb.AddForce(recallVector.normalized * RecallSpeed * (Vector3.Distance(recallPoint.position, transform.position) / 2), ForceMode.Acceleration);

            if (recallVector.sqrMagnitude <= SnapDistance * SnapDistance)
            {
                StopRecallBlade();

                launched = false;

                rb.isKinematic = true;

                _selfPhaser.ChangeEnabled(false);

                transform.parent = recallPoint;
                transform.SetPositionAndRotation(recallPoint.position, recallPoint.rotation);
            }
        }
    }

    public void StartRecallBlade()
    {
        if (!recalling)
        {
            _allSelfColliders.ForEach(col => col.enabled = false);

            _selfStabber.ForceUnstab();

            rb.useGravity = false;
        }

        recalling = true;
    }

    public void StopRecallBlade()
    {
        if (recalling)
        {
            _allSelfColliders.ForEach(col => col.enabled = true);
            rb.useGravity = true;
        }

        recalling = false;
    }

    public void Launch()
    {
        transform.parent = null;
        StopRecallBlade();

        rb.isKinematic = false;



        rb.AddForce(transform.forward * LaunchSpeed, ForceMode.VelocityChange);
        _selfPhaser.ChangeEnabled(true);
        launched = true;
    }
}
