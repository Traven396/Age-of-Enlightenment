using System;
using System.Collections;
using System.Collections.Generic;
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
        public static NamedConditionSet Punch(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;
            
            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
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
        public static NamedConditionSet ReversePunch(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.z <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Reverse Punch {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the negative X axis of the hand. The direction towards your palm. Like a high five/Throwing gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet Push(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
                    break;
            }

            return Tuple.Create($"Push {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }

        /// <summary>
        /// A motion towards the positive X axis of the hand. The direction towards the back of your hand. Imagine backhand slapping someone, that gesture
        /// </summary>
        /// <param name="playerHand">The actual hand to track. This should be given to the ability through the PlayerGestureManager</param>
        /// <param name="direction">Which direction to check against</param>
        /// <param name="velocity">The velocity required to perform the action</param>
        /// <returns></returns>
        public static NamedConditionSet ReversePush(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.x >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
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
        public static NamedConditionSet Slash(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y >= minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
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
        public static NamedConditionSet ReverseSlash(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.x <= -minVelocity;
                    break;
                case AxisDirection.Right:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.x >= minVelocity;
                    break;
                case AxisDirection.Up:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.y >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.y <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.z >= minVelocity;
                    break;
                case AxisDirection.Back:
                    gesture = () => playerHand.SelfSpaceVelocity.y <= -minVelocity && playerHand.ViewSpaceVelocity.z <= -minVelocity;
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
        public static NamedConditionSet SwingGlobal(HandPhysicsTracker playerHand, AxisDirection direction, float? velocity = null)
        {
            //If you dont pass in the velocity, it just defaults to 2
            float minVelocity = velocity ?? SmallGestureSpeed;
            Func<bool> gesture = null;

            switch (direction)
            {
                case AxisDirection.Left:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case AxisDirection.Right:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case AxisDirection.Up:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) >= minVelocity;
                    break;
                case AxisDirection.Down:
                    gesture = () => Vector3.Dot(playerHand.Velocity, Vector3.up) <= -minVelocity;
                    break;
                case AxisDirection.Forward:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
                case AxisDirection.Back:
                    throw new Exception("A SwingGlobal gesture can only be checked against Up or Down.");
            }

            return Tuple.Create($"Swing {direction}", gesture != null ? new[] { gesture } : new Func<bool>[] { });
        }
    }

    public enum AxisDirection
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Back
    }
}