using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleRenderer : MonoBehaviour
{
    [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
    [SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
    [SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

    private void Update()
    {
        HandRaycast();
    }


    private void HandRaycast()
    {

        // Create a ray that points forwards from the camera.
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Do the raycast forweards to see if we hit an interactive item
        if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
        {
            // Something was hit, set at the hit position.
            if (m_Reticle)
                m_Reticle.SetPosition(hit);
        }
        else
        {
            // Position the reticle at default distance.
            if (m_Reticle)
                m_Reticle.SetPosition();
        }
    }
}
