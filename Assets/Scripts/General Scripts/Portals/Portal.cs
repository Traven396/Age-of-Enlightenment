using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal DestinationPortal { get; private set; }
    [SerializeField]
    public Renderer PortalRenderer;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();
    public bool IsActive = false;
    private Collider wallCollider;

  

    private void Update()
    {
        PortalRenderer.enabled = DestinationPortal.IsActive;

        for (int i = portalObjects.Count - 1; i >= 0 ; i--)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);

            if (objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
                portalObjects.Remove(portalObjects[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out PortalableObject obj))
        {
            if (!portalObjects.Contains(obj)) { 
                portalObjects.Add(obj);
                obj.SetIsInPortal(this, DestinationPortal, wallCollider);
            }
        }
        else if(other.transform.parent.TryGetComponent(out PortalableObject obj2))
        {
            if (!portalObjects.Contains(obj2))
            {
                portalObjects.Add(obj2);
                obj2.SetIsInPortal(this, DestinationPortal, wallCollider);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PortalableObject obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        }
        else if (other.transform.parent.TryGetComponent(out PortalableObject obj2))
        {
            portalObjects.Remove(obj2);
            obj2.ExitPortal(wallCollider);
        }
    }
}
