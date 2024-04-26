namespace AgeOfEnlightenment.GestureDetection
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerGestureManager : MonoBehaviour
    {
        [SerializeField] private Transform LeftHandTransform;
        [SerializeField] private Transform RightHandTransform;

        private PhysicsTracker _LeftHandPhysicsTracker = new PhysicsTracker();
        private PhysicsTracker _RightHandPhysicsTracker = new PhysicsTracker();

        private void Start()
        {
            //We first reset both of them to 0 so we know where their starting positions are
            _LeftHandPhysicsTracker.Reset(LeftHandTransform.position, LeftHandTransform.rotation, Vector3.zero, Vector3.zero);
            _RightHandPhysicsTracker.Reset(RightHandTransform.position, RightHandTransform.rotation, Vector3.zero, Vector3.zero);
        }

        public void Update()
        {
            _LeftHandPhysicsTracker.Update(LeftHandTransform.position, LeftHandTransform.rotation, Time.deltaTime);
            _RightHandPhysicsTracker.Update(RightHandTransform.position, RightHandTransform.rotation, Time.deltaTime);

            Debug.Log(_LeftHandPhysicsTracker.Velocity);
        }
    }

}