using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Outline))]
public class TelekinesisTargetable : MonoBehaviour, ITargetable
{

    private Rigidbody rb;

    private Outline selfOutline;

    [HideInInspector] public bool isFloating = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        selfOutline = GetComponent<Outline>();
    }
    public void CatapultStartBehavior()
    {
        OnDeselect();
        StartCoroutine(CatapultVisuals());
    }

    public void CatapultLaunch(Vector3 direction)
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        StopCoroutine(CatapultVisuals());
        isFloating = false;
        rb.AddForce(direction, ForceMode.Impulse);
    }
    IEnumerator CatapultVisuals()
    {
        OnDeselect();
        if (!isFloating)
        {
            isFloating = true;
            Vector3 previousPosition = rb.transform.position;
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.MovePosition(previousPosition + new Vector3(0, .2f, 0));
            

            yield return new WaitForSeconds(10f);

            rb.isKinematic = false;
            rb.useGravity = true;
            isFloating = false;            
        }
    }

    public void OnSelect()
    {
        selfOutline.OutlineWidth = 3f;
    }

    public void OnDeselect()
    {
        selfOutline.OutlineWidth = 0;
    }
}
