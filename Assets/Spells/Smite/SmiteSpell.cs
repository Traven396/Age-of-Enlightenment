using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargettingIndicator))]
[RequireComponent(typeof(ObjectSpawn))]
public class SmiteSpell : SpellBlueprint
{
    public float maxDistance = 10;

    private IMovement _requiredGesture;
    private TargettingIndicator _targetter;
    private ObjectSpawn _objectSpawn;

    private RaycastHit hit;
    private void Start()
    {
        _requiredGesture = GetComponent<IMovement>();
        _targetter = GetComponent<TargettingIndicator>();
            _targetter.SetupReferences(this);
        _objectSpawn = GetComponent<ObjectSpawn>();
    }
    private void Update()
    {
        if (!triggerPressed)
        {
            _visualsManager.ReturnCircleToHolder(currentHand);
        }
    }
    public override void TriggerPress()
    {
        base.TriggerPress();
        if (gripPressed)
        {
            _targetter.readyToCast = false;
        }
    }
    public override void TriggerHold()
    {
        base.TriggerHold();

        hit = _targetManager.RaycastFromHandToGround(currentHand, maxDistance);

        _targetter.TargetMove(hit);
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();

        _targetter.TargetReturn();
    }


    /*Ok, future me. Heres the plan. We should make it so when you perform the gesture for the spell
     it starts a coroutine that moves the circle up and does the lightning strike. Do the raycast thing we thought
    about where it checks if there is a ceiling or not above it to see how high to move it

    While this corotuine is running, or at the start, set a boolean to continue the rest of the script to false. That way the spell circle
    doesnt come flying back in the middle of the lightning strike. At the end of the coroutine call all the targetting and other
    return things so that the spell circle comes back and can be cast again.

    You might be wondering, like I just was, what happens if the player deactivates the spell. Well I thought I had a plan, but upon
    just thinking it over more I realized it wouldnt work. So here are some ideas that might work
    - Put all the strike spell on the spell circle and make it so the script checks if lightning is currently striking before
    deleting the spell circle, instead destroying it on a timer once deactivated. You just need to set this spells reference to the circle to null 
    after doing everything
    - Make a copy of the spell circle and do a substitution jutsu style thing to make it so there is a version that wont get destroyed
    along with the spell when swapping. Have this be the object that is spawned and just make it a big animation thing
    - 
     */

}
