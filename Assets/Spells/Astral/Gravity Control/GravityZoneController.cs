using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AgeOfEnlightenment.PlayerController;
public class GravityZoneController : MonoBehaviour
{
    public float GravityModifier = 0;
    public float RemainingLifetime = 60;

    private bool cooldownCounting;
    private List<GravityObject> ActiveGravityObjects = new List<GravityObject>();

    public Transform ParentTransform;

    public UnityEvent ZoneDeath = new();
    private void Awake()
    {
        GravityModifier = Physics.gravity.magnitude;
    }
    public void StartCountdown()
    {
        cooldownCounting = true;
    }
    public void SetGravityModifier(float modValue)
    {
        GravityModifier = modValue;
        foreach (GravityObject gravObj in ActiveGravityObjects)
        {
            gravObj._ForceApplier.force = Physics.gravity * GravityModifier;
        }

        RemainingLifetime = 60;
    }
    private void OnTriggerEnter(Collider other)
    {
        var rigid = other.attachedRigidbody;

        if(rigid)
        {
            //if (other.TryGetComponent<PlayerMotionController>(out PlayerMotionController controller))
            //{

            //    controller.

            //    return;
            //}


            if (rigid.useGravity)
            {
                var tempObj = new GravityObject(rigid, other.gameObject.AddComponent<ConstantForce>());

                tempObj._Rigidbody.useGravity = false;

                tempObj._ForceApplier.force = Physics.gravity * GravityModifier;

                ActiveGravityObjects.Add(tempObj); 
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody)
        {
            if (other.TryGetComponent<PlayerMotionController>(out _))
            {



                return;
            }


            if (!other.attachedRigidbody.useGravity)
            {
                for (int i = 0; i < ActiveGravityObjects.Count; i++)
                {
                    if (ActiveGravityObjects[i]._Rigidbody == other.attachedRigidbody)
                    {
                        other.attachedRigidbody.useGravity = true;

                        Destroy(other.gameObject.GetComponent<ConstantForce>());

                        ActiveGravityObjects.Remove(ActiveGravityObjects[i]);
                        break;
                    }
                }
                //foreach (GravityObject floatingObject in ActiveGravityObjects)
                //{
                //    if (floatingObject._Rigidbody = other.attachedRigidbody)
                //    {
                //        other.attachedRigidbody.useGravity = floatingObject.OrigUseGravity;

                //        Destroy(other.gameObject.GetComponent<ConstantForce>());

                //        ActiveGravityObjects.Remove(floatingObject);
                //        break;
                //    }
                //} 
            }
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < ActiveGravityObjects.Count; i++)
        {
            ActiveGravityObjects[i]._Rigidbody.useGravity = true;

            Destroy(ActiveGravityObjects[i]._ForceApplier);

            //ActiveGravityObjects.Remove(ActiveGravityObjects[i]);
        }
    }
    private void Update()
    {
        if (cooldownCounting)
        {
            if(RemainingLifetime > 0)
            {
                RemainingLifetime -= Time.deltaTime;
            }


            if (RemainingLifetime <= 0)
            {
                ZoneDeath?.Invoke();
                Destroy(ParentTransform.gameObject);
            }
        }
    }
    private struct GravityObject
    {
        public Rigidbody _Rigidbody;
        public ConstantForce _ForceApplier;

        public GravityObject(Rigidbody rb, ConstantForce force)
        {
            _Rigidbody = rb;
            _ForceApplier = force;
        }
    }
}
