namespace AgeOfEnlightenment.GestureDetection
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerGestureManager : MonoBehaviour
    {
        [SerializeField] private Transform LeftHandTransform;
        [SerializeField] private Transform RightHandTransform;

        private HandPhysicsTracker _LeftHandPhysicsTracker;
        private HandPhysicsTracker _RightHandPhysicsTracker;

        private Step testingStep;

        private void Start()
        {
            _LeftHandPhysicsTracker = new HandPhysicsTracker(LeftHandTransform, true);
            _RightHandPhysicsTracker = new HandPhysicsTracker(RightHandTransform);

            //We first reset both of them to 0 so we know where their starting positions are
            _LeftHandPhysicsTracker.Reset(LeftHandTransform.position, LeftHandTransform.rotation, Vector3.zero, Vector3.zero);
            _RightHandPhysicsTracker.Reset(RightHandTransform.position, RightHandTransform.rotation, Vector3.zero, Vector3.zero);

            testingStep = Step.Start(TestingMethod);
            testingStep.Then(PremadeGestureLibrary.Push(_RightHandPhysicsTracker, AxisDirection.Left))
                .Then(PremadeGestureLibrary.ReversePush(_RightHandPhysicsTracker, AxisDirection.Right))
                .Then(PremadeGestureLibrary.Punch(_RightHandPhysicsTracker, AxisDirection.Forward))
                .Do(FinalMethod, "Finishing Touches");
        }

        public void Update()
        {
            _LeftHandPhysicsTracker.Update(LeftHandTransform.position, LeftHandTransform.rotation, Time.deltaTime);
            _RightHandPhysicsTracker.Update(RightHandTransform.position, RightHandTransform.rotation, Time.deltaTime);

            testingStep.Update();

            //Debug.Log(_RightHandPhysicsTracker.SelfSpaceVelocity);
            if (testingStep.AtEnd())
            {
                testingStep.Reset();
            }
        }

        private void TestingMethod()
        {
            Debug.Log("It worked");
        }

        private void FinalMethod()
        {
            Debug.Log("We made it to the end!");
        }
    }

}