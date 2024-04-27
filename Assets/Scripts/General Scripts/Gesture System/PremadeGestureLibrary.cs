using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace AgeOfEnlightenment.GestureDetection
{
    using NamedConditionSet = Tuple<string, Func<bool>[]>;
    using NamedCondition = Tuple<string, Func<bool>>;

    public static class PremadeGestureLibrary
    {

        /*
         * Some initial terminology so that these names make more sense. If you want to make your own, go hog wild lol
         * 
         *  Hand space is motion in reference to the hand's local space. So if you turn your hand to have the palm facing up, then the local "up" now becomes to the right. Essentially following your thumb
         * 
         *  A punch is a motion along the local Z axis, IE towards your fingers.
         *  A push is a motion along the local X axis, IE your palm.
         *  A slash is a motion along the local Y axis, IE your thumb
         * 
         *  A swing is only used for going up or down, it follows the Global Y axis
         *  
         */
        //The consistent velocity threshold to be used over and over
        const float SmallGestureSpeed = 2f;
        const float MediumGestureSpeed = 3f;
        const float LargeGestureSpeed = 4f;

        /// <summary>
        /// A motion towards the positive Z axis of the hand. Some directions of this gesture are almost physically impossible, so be aware lol
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet PunchInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;
            
            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && !playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Punch {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative Z axis of the hand. Kinda like yanking your hand back. Some directions of this gesture are almost physically impossible, so be aware lol
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePunchInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && !playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Reverse Punch {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards your palm. Like a high five/Throwing gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet PushInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x <= -minVelocity : playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
            }

            return Tuple.Create($"Push {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the positive X axis of the hand (flipped for left hand, the system automatically corrects it). The direction towards the back of your hand. Imagine backhand slapping someone, that gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePushInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                // These first two cases on Left and Right have the weird conditional at the end because we are Flipping the X axis so that gestures are universal for each hand
                //The ? means that if the physics tracker IS the left hand, we use the >= side of it, and if its not the left hand we use the other
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x <= -minVelocity : playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
            }

            return Tuple.Create($"Reverse Push {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the positive Y axis of the hand. The direction towards your thumb. Imagine pulling something out of the ground
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet SlashInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && !playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Slash {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative Y axis of the hand. The direction away your thumb. A sort of stabbing motion
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReverseSlashInViewDirection(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case ViewSpaceDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case ViewSpaceDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
                case ViewSpaceDirection.InwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case ViewSpaceDirection.OutwardH:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && !playerHand.leftHand ? playerHand.ViewSpaceVelocity.x >= minVelocity : playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Slash {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion compared to the Global Y axis. Regardless of any rotation or hand orientation. This only works Up and Down
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against. The only ones that work for this are Up and Down</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet SwingGlobal(HandPhysicsTracker playerHand, ViewSpaceDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case ViewSpaceDirection.Left:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Right:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Up:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) >= minVelocity;
                    break;
                case ViewSpaceDirection.Down:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) <= -minVelocity;
                    break;
                case ViewSpaceDirection.Forward:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case ViewSpaceDirection.Back:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
            }

            return Tuple.Create($"Swing {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// This is a gesture for just pulling your hand back in any direction, along the negative Z axis. The opposite gesture of a fistbump
        /// </summary>
        /// <param name="playerHand">The Hand that should be checked for this gesture</param>
        /// <param name="velocity">The minimum velocity required to achieve the gesture. If not set it defaults to 2 units per second.</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePunchGlobal(HandPhysicsTracker playerHand, float? velocity = null)
        {
            float minVelocity = velocity ?? SmallGestureSpeed;

            Func<bool> gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity;

            return Tuple.Create($"Global Reverse Punch", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }


        #region Helper Functions
        /// <summary>
        /// Gets the absolute value of a float.
        /// </summary>
        public static float Abs(this float num) => Mathf.Abs(num);
        /// <summary>
        /// Returns true if the vector's X component is its largest component
        /// </summary>
        public static bool MostlyX(this Vector3 vec) => vec.x.Abs() > vec.y.Abs() && vec.x.Abs() > vec.z.Abs();

        /// <summary>
        /// Returns true if the vector's Y component is its largest component
        /// </summary>
        public static bool MostlyY(this Vector3 vec) => vec.y.Abs() > vec.x.Abs() && vec.y.Abs() > vec.z.Abs();

        /// <summary>
        /// Returns true if the vector's Z component is its largest component
        /// </summary>
        public static bool MostlyZ(this Vector3 vec) => vec.z.Abs() > vec.x.Abs() && vec.z.Abs() > vec.y.Abs(); 
        #endregion
    }


    public enum ViewSpaceDirection
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back,
        InwardH,
        OutwardH
    }
}