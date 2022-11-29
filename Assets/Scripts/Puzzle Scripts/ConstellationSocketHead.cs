using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Aidyn Reis
 * 9/29/22
 * Advanced Technologies Project 2
 */

public class ConstellationSocketHead : MonoBehaviour
{
    
    [HideInInspector]
    public MeshRenderer[] childSockets;
    //public bool visualOverride = false;
    void Start()
    {
        childSockets = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in childSockets)
        {
            meshRenderer.enabled = false;
        }
    }
    private void Update()
    {
        //if (visualOverride && !childSockets[0].enabled)
        //{
        //    foreach (MeshRenderer meshRenderer in childSockets)
        //    {
        //        meshRenderer.enabled = true;
        //    }
            
        //}
        //else if (!visualOverride && childSockets[0].enabled)
        //{
        //    foreach (MeshRenderer meshRenderer in childSockets)
        //    {
        //        meshRenderer.enabled = false;
        //    }
            
        //}
    }

}
