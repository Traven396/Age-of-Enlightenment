namespace AgeOfEnlightenment.GestureDetection
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class PlayerGestureManager : MonoBehaviour
    {

        //We need to also keep track of the players body so that them moving around normally doesnt cause the gestures to activate.
        //since if you were to walk forward in game, your hands would count as moving in world space. So we just subtract out the player motion while in HandPhysicsTracker

        [Header("Player's Body")]
        [SerializeField] private Transform PlayerBody;
        [Header("Hands")]
        [SerializeField] private Transform LeftHandTransform;
        [SerializeField] private Transform RightHandTransform;

        [Header("Debug Stuff")]
        [SerializeField] private bool DisplayLeftHandValues;
        [SerializeField] private bool DisplayRightHandValues;
        [SerializeField] private TMP_Text DebugOutputText;

        private HandPhysicsTracker _LeftHandPhysicsTracker;
        private HandPhysicsTracker _RightHandPhysicsTracker;

        //This is an example of how you can setup a step sequence. Everything that uses this is currently commented out. But you can enable it to check it out.
        //Also if you have questions, message me directly if you can.
        private Step testingStep;

        private void Start()
        {
            _LeftHandPhysicsTracker = new HandPhysicsTracker(LeftHandTransform, PlayerBody, true);
            _RightHandPhysicsTracker = new HandPhysicsTracker(RightHandTransform, PlayerBody);

            //We first reset both of them to 0 so we know where their starting positions are
            _LeftHandPhysicsTracker.Reset(LeftHandTransform.position, LeftHandTransform.rotation, Vector3.zero, Vector3.zero);
            _RightHandPhysicsTracker.Reset(RightHandTransform.position, RightHandTransform.rotation, Vector3.zero, Vector3.zero);

            //testingStep = Step.Start(TestingMethod);
            //testingStep.Then(PremadeGestureLibrary.Push(_RightHandPhysicsTracker, AxisDirection.Left))
            //    .Then(PremadeGestureLibrary.ReversePush(_RightHandPhysicsTracker, AxisDirection.Right))
            //    .Then(PremadeGestureLibrary.Punch(_RightHandPhysicsTracker, AxisDirection.Forward))
            //    .Do(FinalMethod, "Finishing Touches");
        }

        public void NewLeftSpell(SpellSwapCallbackContext ctx)
        {
            if (ctx.spawnedScript)
            {
                ctx.spawnedScript._handPhysics = _LeftHandPhysicsTracker;

                Debug.Log(ctx.spawnedScript._handPhysics);
            }
        }
        public void NewRightSpell(SpellSwapCallbackContext ctx)
        {
            if (ctx.spawnedScript)
            {
                ctx.spawnedScript._handPhysics = _RightHandPhysicsTracker;
            }
        }


        public void Update()
        {
            _LeftHandPhysicsTracker.Update(LeftHandTransform.position, LeftHandTransform.rotation, Time.deltaTime);
            _RightHandPhysicsTracker.Update(RightHandTransform.position, RightHandTransform.rotation, Time.deltaTime);

            if (DisplayLeftHandValues || DisplayRightHandValues)
            {
                string OutputMessage = "";
                if (DisplayLeftHandValues)
                {
                    OutputMessage += "Left Hand: \n " +
                        "\tWorld Velocity - " + _LeftHandPhysicsTracker.Velocity + "\n" +
                        "\tSelf Space Velocity - " + _LeftHandPhysicsTracker.SelfSpaceVelocity + "\n" +
                        "\tView Space Velocity - " + _LeftHandPhysicsTracker.ViewSpaceVelocity + "\n" +
                        "\tFinger Velocity - " + _LeftHandPhysicsTracker.FingerLinearVelocity + "\n\n" +
                        "\tAngular Velocity - " + _LeftHandPhysicsTracker.AngularVelocity + "\n" +
                        "\tFinger Angular Velocity - " + _LeftHandPhysicsTracker.FingerAngularVelocity + "\n";
                }
                if (DisplayRightHandValues)
                {
                    OutputMessage += "Right Hand: \n " +
                        "\tWorld Velocity - " + _RightHandPhysicsTracker.Velocity + "\n" +
                        "\tSelf Space Velocity - " + _RightHandPhysicsTracker.SelfSpaceVelocity + "\n" +
                        "\tView Space Velocity - " + _RightHandPhysicsTracker.ViewSpaceVelocity + "\n" +
                        "\tFinger Velocity - " + _RightHandPhysicsTracker.FingerLinearVelocity + "\n\n" +
                        "\tAngular Velocity - " + _RightHandPhysicsTracker.AngularVelocity + "\n" +
                        "\tFinger Angular Velocity - " + _RightHandPhysicsTracker.FingerAngularVelocity;
                }

                if (DebugOutputText)
                    DebugOutputText.text = OutputMessage;
                else
                    Debug.LogWarning("You have not set somewhere for the results to output to!");
            }


            //testingStep.Update();

            //if (testingStep.AtEnd())
            //{
            //    testingStep.Reset();
            //}
        }

        //private void TestingMethod()
        //{
        //    Debug.Log("It worked");
        //}

        //private void FinalMethod()
        //{
        //    Debug.Log("We made it to the end!");
        //}
    }

}