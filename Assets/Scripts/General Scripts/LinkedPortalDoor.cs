using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedPortalDoor : MonoBehaviour
{
    [SerializeField]
    private GameObject DoorObject;
    [SerializeField]
    private float RotationAmount = 90f;
    [SerializeField]
    private float Speed = 1;
    [SerializeField]
    private RotationDirection RotationAxis;

    private enum RotationDirection
    {
        X,
        Y,
        Z
    }

    private bool currentlyOpen;
    private Quaternion startingRotation;
    private Coroutine animationCoroutine;

    private void Start()
    {
        startingRotation = DoorObject.transform.rotation;
    }
    public void OpenDoor()
    {
        if (!currentlyOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = StartCoroutine(DoRotationOpen());

        }
    }

    public void CloseDoor()
    {

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(DoRotationClose());
}

    private IEnumerator DoRotationOpen()
    {
        Quaternion startRotation = DoorObject.transform.rotation;
        Quaternion endRotation = Quaternion.identity;


        switch (RotationAxis)
        {
            case RotationDirection.X:
                endRotation = startingRotation * Quaternion.Euler(RotationAmount, 0, 0);
                break;
            case RotationDirection.Y:
                endRotation = startingRotation * Quaternion.Euler(0, RotationAmount, 0);
                break;
            case RotationDirection.Z:
                endRotation = startingRotation * Quaternion.Euler(0, 0, RotationAmount);
                break;
        }
        
        
        currentlyOpen = true;

        float time = 0;
        while (time < 1)
        {
            DoorObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = DoorObject.transform.rotation;
        Quaternion endRotation = startingRotation;

        currentlyOpen = false;

        float time = 0;
        while (time < 1)
        {
            DoorObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DoorObject)
        {
            if (other.CompareTag("MainCamera"))
            {
                if (!currentlyOpen)
                    OpenDoor();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (DoorObject)
        {
            if (other.CompareTag("MainCamera"))
            {
                if (currentlyOpen)
                    CloseDoor();
            }
        }
    }
}
