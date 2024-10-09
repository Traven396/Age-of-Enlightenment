using UnityEngine;
using System.Collections;
using System.Collections.Generic;


	[System.Serializable]
	public class Sensor {

		//Basic raycast parameters;
		public float castLength = 1f;

		//Starting point of (ray-)cast;
		private Vector3 origin = Vector3.zero;

		//Enum describing local transform axes used as directions for raycasting;
		public enum CastDirection
		{
			Forward,
			Right,
			Up,
			Backward, 
			Left,
			Down
		}

		private CastDirection castDirection;

		//Raycast hit information variables;
		private bool hasDetectedHit;
		private Vector3 hitPosition;
		private Vector3 hitNormal;
		private float hitDistance;
		private List<Collider> hitColliders = new List<Collider>();
		private List<Transform> hitTransforms = new List<Transform>();

		//References to attached components;
		private Transform tr;
		private Collider col;

		public LayerMask layermask = 255;

		//Layer number for 'Ignore Raycast' layer;
		int ignoreRaycastLayer;

		//Optional list of colliders to ignore when raycasting;
		private Collider[] ignoreList;

		//Array to store layers of colliders in ignore list;
		private int[] ignoreListLayers;

		//Whether to draw debug information (hit positions, hit normals...) in the editor;
		public bool isInDebugMode = false;

		//Constructor;
		public Sensor (Transform _transform, Collider _collider)
		{
			tr = _transform;

			if(_collider == null)
				return;

			ignoreList = new Collider[1];

			//Add collider to ignore list;
			ignoreList[0] = _collider;

			//Store "Ignore Raycast" layer number for later;
			ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

			//Setup array to store ignore list layers;
			ignoreListLayers = new int[ignoreList.Length];
		}

		//Reset all variables related to storing information on raycast hits;
		private void ResetFlags()
		{
			hasDetectedHit = false;
			hitPosition = Vector3.zero;
			hitNormal = -GetCastDirection();
			hitDistance = 0f;

			if(hitColliders.Count > 0)
				hitColliders.Clear();
			if(hitTransforms.Count > 0)
				hitTransforms.Clear();
		}

		public void Cast()
		{
			ResetFlags();

			//Calculate origin and direction of ray in world coordinates;
			Vector3 _worldDirection = GetCastDirection();
			Vector3 _worldOrigin = tr.TransformPoint(origin);

			//Check if ignore list length has been changed since last frame;
			if(ignoreListLayers.Length != ignoreList.Length)
			{
				//If so, setup ignore layer array to fit new length;
				ignoreListLayers = new int[ignoreList.Length]; 
			}

			//(Temporarily) move all objects in ignore list to 'Ignore Raycast' layer;
			for(int i = 0; i < ignoreList.Length; i++)
			{
				ignoreListLayers[i] = ignoreList[i].gameObject.layer;
				ignoreList[i].gameObject.layer = ignoreRaycastLayer;
			}

			
			CastRay(_worldOrigin, _worldDirection);
					

			//Reset collider layers in ignoreList;
			for(int i = 0; i < ignoreList.Length; i++)
			{
				ignoreList[i].gameObject.layer = ignoreListLayers[i];
			}
		}

		//Cast a single ray into '_direction' from '_origin';
		private void CastRay(Vector3 _origin, Vector3 _direction)
		{
        hasDetectedHit = Physics.Raycast(_origin, _direction, out RaycastHit _hit, castLength, layermask, QueryTriggerInteraction.Ignore);

        if (hasDetectedHit)
			{
				hitPosition = _hit.point;
				hitNormal = _hit.normal;

				hitColliders.Add(_hit.collider);
				hitTransforms.Add(_hit.transform);

				hitDistance = _hit.distance;
			}
		}

		//Calculate a direction in world coordinates based on the local axes of this gameobject's transform component;
		Vector3 GetCastDirection()
		{
			switch(castDirection)
			{
			case CastDirection.Forward:
				return tr.forward;

			case CastDirection.Right:
				return tr.right;

			case CastDirection.Up:
				return tr.up;

			case CastDirection.Backward:
				return -tr.forward;

			case CastDirection.Left:
				return -tr.right;

			case CastDirection.Down:
				return -tr.up;
			default:
				return Vector3.one;
			}
		}

		//Draw debug information in editor (hit positions and ground surface normals);
		public void DrawDebug()
		{
			if(hasDetectedHit && isInDebugMode)
			{
				Debug.DrawRay(hitPosition, hitNormal, Color.red, Time.deltaTime);
				float _markerSize = 0.2f;
				Debug.DrawLine(hitPosition + Vector3.up * _markerSize, hitPosition - Vector3.up * _markerSize, Color.green, Time.deltaTime);
				Debug.DrawLine(hitPosition + Vector3.right * _markerSize, hitPosition - Vector3.right * _markerSize, Color.green, Time.deltaTime);
				Debug.DrawLine(hitPosition + Vector3.forward * _markerSize, hitPosition - Vector3.forward * _markerSize, Color.green, Time.deltaTime);
			}
		}

		//Getters;

		//Returns whether the sensor has hit something;
		public bool HasDetectedHit()
		{
			return hasDetectedHit;
		}

		//Returns how far the raycast reached before hitting a collider;
		public float GetDistance()
		{
			return hitDistance;
		}

		//Returns the surface normal of the collider the raycast has hit;
		public Vector3 GetNormal()
		{
			return hitNormal;
		}

		//Returns the position in world coordinates where the raycast has hit a collider;
		public Vector3 GetPosition()
		{
			return hitPosition;
		}

		//Returns a reference to the collider that was hit by the raycast;
		public Collider GetCollider()
		{
			return hitColliders[0];
		}

		//Returns a reference to the transform component attached to the collider that was hit by the raycast;
		public Transform GetTransform()
		{
			return hitTransforms[0];
		}

		//Setters;

		//Set the position for the raycast to start from;
		//The input vector '_origin' is converted to local coordinates;
		public void SetCastOrigin(Vector3 _origin)
		{
			if(tr == null)
				return;
			origin = tr.InverseTransformPoint(_origin);
		}

		//Set which axis of this gameobject's transform will be used as the direction for the raycast;
		public void SetCastDirection(CastDirection _direction)
		{
			if(tr == null)
				return;

			castDirection = _direction;
		}
	}
