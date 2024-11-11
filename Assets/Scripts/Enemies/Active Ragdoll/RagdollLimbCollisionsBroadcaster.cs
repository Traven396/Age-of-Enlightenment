using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgeOfEnlightenment.Enemies
{
    #region Supporting Structs and Classes
    /// <summary>
    /// All the required information when a limb collides with something.
    /// </summary>
    public struct RagdollCollision
    {

        /// <summary>
        /// The index of the colliding Limb in the RagdollAnimatorV2.Limbs array.
        /// </summary>
        public int limbIndex;

        /// <summary>
        /// The collision from OnCollisionEnter/Stay/Exit.
        /// </summary>
        public Collision collision;

        public bool isStay;

        public RagdollCollision(int limbIndex, Collision collision, bool isStay = false)
        {
            this.limbIndex = limbIndex;
            this.collision = collision;
            this.isStay = isStay;
        }
    }
	/// <summary>
	/// A way to hit limbs through code methods. Like raycasting
	/// </summary>
	public struct RagdollLimbHit
	{

		/// <summary>
		/// The index of the colliding Limb
		/// </summary>
		public int limbIndex;

		/// <summary>
		/// How much should the limb be unpinned by the hit?
		/// </summary>
		public float unPin;

		/// <summary>
		/// The force to add to the limb's Rigidbody.
		/// </summary>
		public Vector3 force;

		/// <summary>
		/// The world space hit point.
		/// </summary>
		public Vector3 position;

		public RagdollLimbHit(int limbIndex, float unPin, Vector3 force, Vector3 position)
		{
			this.limbIndex = limbIndex;
			this.unPin = unPin;
			this.force = force;
			this.position = position;
		}
	}
	#endregion
	public class RagdollLimbCollisionsBroadcaster : MonoBehaviour
    {
		[HideInInspector] public RagdollAnimatorV2 ragdollAnimator;
		/// <summary>
		/// The index of this limb in the Limbs array
		/// </summary>
		[HideInInspector] public int limbIndex;

		public void Hit(float unPin, Vector3 force, Vector3 position)
		{
			if (!enabled) return;
			if (!ragdollAnimator.knockoutComponent) return;
			
			ragdollAnimator.knockoutComponent.OnLimbHit(new RagdollLimbHit(limbIndex, unPin, force, position));
		}
		// Prevents processing internal collisions.
		private bool IsSelf(Collider c)
		{
			//return c.transform.root == transform.root; // Might be faster, but make sure characters are not stacked to the same root!
			return c.transform.IsChildOf(ragdollAnimator.transform); // Use this if you need all your puppets to be parented to a single container gameobject.
		}
		void OnCollisionEnter(Collision collision)
		{
			if (!enabled) return;
			if (ragdollAnimator == null) return;
			if (IsSelf(collision.collider)) return;
			if (!ragdollAnimator.Limbs[limbIndex].ActiveLimb) return;
			if (!ragdollAnimator.knockoutComponent) return;

			ragdollAnimator.knockoutComponent.OnLimbCollision(new RagdollCollision(limbIndex, collision));
		}
	} 
}
