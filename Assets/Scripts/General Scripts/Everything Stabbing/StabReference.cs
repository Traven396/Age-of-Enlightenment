using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.StabbingPhysics
{
    public class StabReference
    {
        public IsStabbable StabbableObject { get; private set; }
        public StabbableObjectSettings Settings { get; }

        public StabbingObject StabberObject { get; private set; }

        public ConfigurableJoint Joint { get; private set; }
        public Vector3 StabEntryPosition { get; private set; }

        public Vector3 StabExitPosition { get; private set; }

        public GameObject StabbedObject { get; private set; }
        public List<Collider> StabbedColliders { get; private set; }


        //So, there is a lot of variables here. If you have ever used hurricaneVR before they might look familiar. I used it as a basis for what I'm doing here.
        public Vector3 StabDirection => StabbedObject.transform.TransformDirection(_stabDirectionLocal);

        private readonly float _stabberLength;
        private bool _outerShellBreached;
        private float _previousDepth;
        private bool _canFullStab;
        private bool _hasFullStabbed;
        private readonly Vector3 _stabLocalPosition;
        private float _stabbedTime;
        private Vector3 _stabExitLocalPosition;
        private readonly Vector3 _stabDirectionLocal;
        private readonly Transform _tip;
        private readonly Transform _base;
        private readonly float _unstabThreshold;
        private bool _hasEntered;
        private Vector3 _entryToTip;
        private Vector3 _exitToBase;
        private bool _tipValid;
        private bool _baseValid;

        public StabReference(StabbingObject stabber, IsStabbable stabbable, StabbableObjectSettings settings, ConfigurableJoint joint, GameObject stabbedObject, Vector3 stabDirection, Transform tip, List<Collider> stabbedColliders)
        {
            StabberObject = stabber;
            StabbableObject = stabbable;
            Settings = settings;

            Joint = joint;
            StabbedObject = stabbedObject;
            StabbedColliders = stabbedColliders;

            _stabberLength = stabber.bladeLength; //use world in case stabber is scaled
            _canFullStab = true;
            _stabLocalPosition = Joint.connectedAnchor;
            _stabDirectionLocal = stabbedObject.transform.InverseTransformDirection(stabDirection);
            _tip = tip;
            _base = tip == stabber.Tip ? stabber.Base : stabber.Tip;

            _unstabThreshold = StabberObject.Settings.UnstabThreshold;

            UpdateEntryAndExit();
        }

        public bool Update()
        {
            if (!StabbedObject)
                return false;

            UpdateEntryAndExit();

            _entryToTip = _tip.position - StabEntryPosition;
            _exitToBase = _base.position - StabExitPosition;

            _tipValid = Vector3.Dot(_entryToTip, StabDirection) > 0f;
            _baseValid = Vector3.Dot(_exitToBase, -StabDirection) > 0f;


            if (CheckUnstab())
            {
                return false;
            }

            //First value is how far stabber we are.
            //Second value is that as a percent of the whole blade
            UpdateFriction(_entryToTip.magnitude, _entryToTip.magnitude / _stabberLength);


            return true;
        }
        private bool CheckUnstab()
        {
            if (_stabbedTime < StabberObject.Settings.UnstabDelay)
            {
                _stabbedTime += Time.deltaTime;
                return false;
            }

            var tipCheck = !_tipValid && _entryToTip.magnitude > _unstabThreshold;

            if (tipCheck)
                return true;

            var point = HandyHelperScript.FindNearestPointOnLine(_tip.position, _base.position, StabEntryPosition);
            var line = StabEntryPosition - point;
            if (line.sqrMagnitude > StabberObject.Settings.PerpendicularThreshold * StabberObject.Settings.PerpendicularThreshold)
            {
                return true;
            }

            return false;
        }
        private void UpdateFriction(float depth, float percent)
        {
            var damper = 0f;
            if (!_outerShellBreached)
            {
                damper = Settings.OuterShellDamper;
                _outerShellBreached = depth > Settings.OuterShellThickness;
            }

            if (_outerShellBreached)
            {
                if (Settings.UseDamperCurve)
                {
                    damper = Settings.InnerDamperCurve.Evaluate(percent) * Settings.Damper;
                }
                else
                {
                    damper = Settings.Damper;
                }
            }

            var drive = Joint.xDrive;
            drive.positionDamper = damper * (1 - StabberObject.Settings.Sharpness);
            Joint.xDrive = drive;
        }
        private void UpdateEntryAndExit()
        {
            if (Joint.connectedBody)
            {
                StabEntryPosition = Joint.connectedBody.transform.TransformPoint(_stabLocalPosition);
            }
            else
            {
                StabEntryPosition = _stabLocalPosition;
            }

            StabExitPosition = StabbedObject.transform.TransformPoint(_stabExitLocalPosition);
        }
    } 
}
