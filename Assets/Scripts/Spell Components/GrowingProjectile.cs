using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingProjectile : MonoBehaviour
{
    [SerializeField] private float MaxSize;
    [SerializeField] private float ScaleSpeed;

    private float currentScaleAmount = 0;
    private Vector3 startingScale = Vector3.zero;
    private Transform previousParent;

    private void Awake()
    {
        startingScale = transform.localScale;
        previousParent = transform.parent;
    }

    private void OnEnable()
    {
        //startingScale = transform.localScale;
        currentScaleAmount = 0;
    }

    private void Update()
    {
        //if(previousParent != transform.parent)
        //{
        //    startingScale = startingScale * (1 / transform.parent.localScale.magnitude);
        //    Debug.Log("Parent change");
        //}
    }

    private void FixedUpdate()
    {
        if (currentScaleAmount <= MaxSize)
        {
            currentScaleAmount += ScaleSpeed;

            var scaleVector = (currentScaleAmount * startingScale + startingScale);

            var x = 0f;
            var y = 0f;
            var z = 0f;

            if (transform.parent)
            {
                x = scaleVector.x / transform.parent.localScale.x;
                y = scaleVector.y / transform.parent.localScale.y;
                z = scaleVector.z / transform.parent.localScale.z; 
            }
            else
            {
                x = scaleVector.x;
                y = scaleVector.y;
                z = scaleVector.z;
            }

            transform.localScale = new Vector3(x, y, z);

            //Debug.Log(currentScaleAmount);
        }
    }
}
