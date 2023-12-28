using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.StabbingPhysics
{
    [CreateAssetMenu(menuName = "Stabbing Mechanics/Stabbable Settings")]
    public class StabbableObjectSettings : ScriptableObject
    {
        [Tooltip("Required velocity to initiate the stab")]
        public float RequiredVelocity = 1f;

        [Header("Friction")]

        [Tooltip("Joint Friction In effect until OuterShellDepth is breached by the stabber")]
        public float OuterShellDamper = 5000;

        [Tooltip("How thick is the outer shell (like a skull or box)")]
        public float OuterShellThickness = .025f;

        [Tooltip("Base Damper Scaled By InnerDamperCurve")]
        public float Damper = 1000;

        [Tooltip("If true uses the damper curve against depth / blade length, otherwise flat damper will be used")]
        public bool UseDamperCurve = true;

        [Tooltip("Joint Damper curve that takes over once the outer shell is breached, defaults to 1 if not provided")]
        public AnimationCurve InnerDamperCurve;



        public void CheckCurve()
        {
            if (InnerDamperCurve == null)
            {
                InnerDamperCurve = new AnimationCurve();
            }

            if (InnerDamperCurve.keys.Length == 0)
            {
                InnerDamperCurve.AddKey(0f, 1f);
                InnerDamperCurve.AddKey(1f, 1f);
            }

        }
    } 
}
