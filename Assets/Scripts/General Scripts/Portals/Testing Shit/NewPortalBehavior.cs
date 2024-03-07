using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPortalBehavior : MonoBehaviour
{
    public TestingScript PORTALCAMERA;
    private Material _portalMaterial;
    private void Awake()
    {
        _portalMaterial = GetComponent<MeshRenderer>().material;
    }

    private void OnWillRenderObject()
    {
        //PORTALCAMERA.RenderIntoMaterial(_portalMaterial);
    }
}
