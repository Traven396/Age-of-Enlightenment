using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : SpellBlueprint
{
    [Header("Mana Cost")]
    public int manaCost = 8;
    [Header("Range")]
    public float raycastDistance = 3;
    public float sphereRadius = .5f;
    
    [Header("Audio")]
    public AudioClip launchSound;

    private ApplyMotion castEffect;
    private IMovement gesture;

    private TargettableEntity currentTargettedEntity;
    private List<TargettableEntity> selectedObjects = new List<TargettableEntity>();
    private void Start()
    {
        castEffect = GetComponent<ApplyMotion>();
        gesture = GetComponent<IMovement>();
    }
    #region Old Logic
    //private ITargetable potentialTarget;


    //public float launchStrength = 5f;

    //private List<TelekinesisTargetable> floatingObjects = new List<TelekinesisTargetable>();


    //public override void TriggerHold()
    //{
    //    base.TriggerHold();
    //    potentialTarget = _targetManager.GetClosest(currentHand);
    //}
    //public override void TriggerRelease()
    //{
    //    base.TriggerRelease();
    //    if (potentialTarget != null)
    //    {
    //        if (potentialTarget is TelekinesisTargetable)
    //        {
    //            var targetConverted = (Component)potentialTarget;
    //            castEffect.Cast(targetConverted.transform);
    //            if (!floatingObjects.Contains((TelekinesisTargetable)potentialTarget))
    //            {
    //                floatingObjects.Add((TelekinesisTargetable)potentialTarget);
    //            }
    //        }

    //    }
    //}

    //public override void GripHold()
    //{
    //    base.GripHold();
    //    if (floatingObjects.Count != 0 && gesture.GesturePerformed(_gestureManager, out Vector3 direction))
    //    {
    //        foreach (TelekinesisTargetable targetable in floatingObjects)
    //        {
    //            targetable.CatapultLaunch(direction * launchStrength);
    //        }
    //        floatingObjects = new List<TelekinesisTargetable>();
    //    }
    //}

    #endregion
    #region New Logic
    
    public override void TriggerHold()
    {
        currentTargettedEntity = _TargetManager.GetClosestTeleTarget(currentHand, raycastDistance, sphereRadius);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (currentTargettedEntity)
        {
            if (!selectedObjects.Contains(currentTargettedEntity))
            {
                if (CheckCurrentMana(manaCost))
                {
                    selectedObjects.Add(currentTargettedEntity);
                    StartCoroutine(FloatingSelection(currentTargettedEntity));
                    PlayerSingleton.Instance.SubtractMana(manaCost);
                }
            }
        }
        currentTargettedEntity = null;
    }

    public override void GripHold()
    {
        if (selectedObjects.Count != 0 && gesture.GesturePerformed(_HandPhysicsTracker, out Vector3 direction))
        {
            foreach (TargettableEntity targetable in selectedObjects)
            {
                UnfloatSomeMayCallItFalling(targetable);
                castEffect.Cast(targetable.GetTargetRB(), direction);
                AudioSource.PlayClipAtPoint(launchSound, targetable.transform.position, .4f);
            }
            selectedObjects = new List<TargettableEntity>();
            StopAllCoroutines();
        }
    }

    void UnfloatSomeMayCallItFalling(TargettableEntity target)
    {
        target.GetTargetRB().isKinematic = false;
        target.GetTargetRB().useGravity = true;
        target.isSelected = false;
    }
    IEnumerator FloatingSelection(TargettableEntity target)
    {
        target.Deselect();
        target.isSelected = true;

        iTween.MoveAdd(target.gameObject, iTween.Hash("y", .4f, "time", .15f, "space", Space.World));
        target.GetTargetRB().useGravity = false;
        yield return new WaitForSeconds(.15f);
        target.GetTargetRB().isKinematic = true;
        yield return new WaitForSeconds(5);

        selectedObjects.Remove(target);
        target.GetTargetRB().useGravity = true;
        target.GetTargetRB().isKinematic = false;

        target.isSelected = false;
    }
    #endregion

    public override void OnDeselect()
    {
        base.OnDeselect();
        foreach (TargettableEntity targetable in selectedObjects)
        {
            UnfloatSomeMayCallItFalling(targetable);
        }
    }
}
