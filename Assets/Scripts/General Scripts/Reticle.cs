using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    [SerializeField] private float m_DefaultDistance = 5f;      // The default distance away from the camera the reticle is placed.
    [SerializeField] private Image m_Image;                     // Reference to the image component that represents the reticle.
    [SerializeField] private Transform m_ReticleTransform;      // We need to affect the reticle's transform.
    [SerializeField] private Transform m_Emitter;                


    private Vector3 m_OriginalScale;                            // Since the scale of the reticle changes, the original scale needs to be stored.


    public Transform ReticleTransform { get { return m_ReticleTransform; } }


    private void Awake()
    {
        // Store the original scale and rotation.
        m_OriginalScale = m_ReticleTransform.localScale;
    }


    public void Hide()
    {
        m_Image.enabled = false;
    }


    public void Show()
    {
        m_Image.enabled = true;
    }

    public void SetPosition()
    {
        // Set the position of the reticle to the default distance in front of the camera.
        m_ReticleTransform.position = m_Emitter.position + m_Emitter.forward * m_DefaultDistance;

        // Set the scale based on the original and the distance from the camera.
        m_ReticleTransform.localScale = m_OriginalScale * m_DefaultDistance;

        transform.LookAt(Camera.main.transform);
    }

    public void SetPosition(RaycastHit hit)
    {
        m_ReticleTransform.position = hit.point;
        m_ReticleTransform.localScale = m_OriginalScale * hit.distance;

        transform.LookAt(Camera.main.transform);
    }
}
