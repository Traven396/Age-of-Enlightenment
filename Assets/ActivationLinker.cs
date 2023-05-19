using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationLinker : MonoBehaviour
{
    [SerializeField]
    private GameObject linkedObject;

    private void OnEnable()
    {
        linkedObject.SetActive(true);
    }
    private void OnDisable()
    {
        linkedObject.SetActive(false);
    }
}
