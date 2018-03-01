using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSensors : MonoBehaviour
{
	public LayerMask mLayerMask;
	private Transform mTransform;
	private Rigidbody mRigidBody;//the ships rigidbody
	private float mShipWeight;//TODO//??do i need this??//the mass of the ships rigidbody;

	public float mDetectionRange = 40f;



	//Results
	public RaycastHit? mVerticalHit;
	public Ray mVerticalRayCheck;

	public RaycastHit? mRelativeVerticalHit;
	public Ray mRelativeVerticalRayCheck;


	public Vector3? mTrackNormal;
	public float? mTrackDistance;

	void Start ()
	{
		mTransform = transform;
		mRigidBody = gameObject.GetComponent<Rigidbody> ();
		//mShip = transform.gameObject;
		mShipWeight = mRigidBody.mass;
	}

	void FixedUpdate ()
	{
		
		mVerticalRayCheck = new Ray (mTransform.position + mRigidBody.centerOfMass, -Vector3.up);
		Debug.DrawLine(mVerticalRayCheck.origin, mVerticalRayCheck.origin + (mVerticalRayCheck.direction * mDetectionRange), Color.red);
		RaycastHit tHit;
		if (Physics.Raycast (mVerticalRayCheck, out tHit, mDetectionRange, mLayerMask))
		{
			mVerticalHit = tHit;
		}
		else
		{
			mVerticalHit = null;
		}


		mRelativeVerticalRayCheck = new Ray (mTransform.position + mRigidBody.centerOfMass, -mTransform.up);
		Debug.DrawLine (mRelativeVerticalRayCheck.origin, mRelativeVerticalRayCheck.origin + (mRelativeVerticalRayCheck.direction * mDetectionRange), Color.red);
		if (Physics.Raycast (mRelativeVerticalRayCheck, out tHit, mDetectionRange, mLayerMask))
		{
			mRelativeVerticalHit = tHit;
			mTrackNormal = mRelativeVerticalHit.Value.normal;

			Ray tNormalRay = new Ray (mTransform.position + mRigidBody.centerOfMass, -mTrackNormal.Value);
			Debug.DrawLine (tNormalRay.origin,tNormalRay.origin+(tNormalRay.direction*mDetectionRange),Color.red);

			if (Physics.Raycast (tNormalRay, out tHit, mDetectionRange, mLayerMask))
			{
				mTrackDistance = tHit.distance;
			}
			else
			{
				mTrackDistance = mRelativeVerticalHit.Value.distance;
			}
		}
		else
		{
			mTrackNormal = null;
			mRelativeVerticalHit = null;
		}
	}
}
