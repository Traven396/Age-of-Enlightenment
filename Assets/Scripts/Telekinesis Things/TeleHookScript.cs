using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleHookScript : MonoBehaviour
{
    [SerializeField] public float followSpeed = 30f;
    [SerializeField] public float rotationSpeed = 100f;
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
    public Rigidbody _body;

    private void Update()
    {
        if (_body != null)
        {
            var positionWithOffset = transform.position + positionOffset;
            var distance = Vector3.Distance(positionWithOffset, _body.transform.position);
            _body.velocity = (positionWithOffset - _body.transform.position).normalized * followSpeed * distance;

            var rotationWithOffset = transform.rotation * Quaternion.Euler(rotationOffset);
            var q = rotationWithOffset * Quaternion.Inverse(_body.transform.rotation);
            q.ToAngleAxis(out float angle, out Vector3 axis);
            _body.angularVelocity = angle * axis * Mathf.Deg2Rad * rotationSpeed;
        }

    }
}
