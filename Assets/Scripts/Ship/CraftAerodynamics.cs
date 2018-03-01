using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CraftAerodynamics : MonoBehaviour
{

	[SerializeField]
	private bool mAerodynamicsActive =true;
	private float mZXAeroScalar = 0f;
	private float mYZAeroScalar = 0f;
	[SerializeField]
	private float mAerodynamicStrength = 50f;

	[SerializeField]
	private float mMaxAeroAcceleration = 50f;
	private float mCurrentMaxAeroAcceleration = 0f;


	[SerializeField]
	private AnimationCurve mAerodynamicZXAnglePerformace;//How much air resistance there is on the ZX plane
	[SerializeField]
	private AnimationCurve mAerodynamicYZAnglePerformace;//How much air resistance there is on the YZ plane

	private Transform mTransform;
	private Rigidbody mRigidBody;//the ships rigidbody
	private ThrustEngine mThrustEngine;
	private float mShipWeight;//TODO//??do i need this??//the mass of the ships rigidbody;


	private Vector3 mResultantAcceleration = Vector3.zero;

	void Start ()
	{
		mCurrentMaxAeroAcceleration = mMaxAeroAcceleration;
		mTransform = transform;
		mRigidBody = GetComponent<Rigidbody> ();
		mThrustEngine = GetComponent<ThrustEngine> ();
	}

	void Update ()
	{
		
	}

	void FixedUpdate()
	{
		//Aerodynamics
		if(mAerodynamicsActive)
		{
			//mControlSurfaceFactor = mAerodynamicSurfaceOrientationPerformace.Evaluate ();

			//convert velocity into local space
			Vector3 tLocalVelocity = mTransform.InverseTransformDirection (mRigidBody.velocity);
			//tLocalVelocity = mRigidBody.velocity * Quaternion.Inverse(mTransform.rotation);
			//Quaternion.

			Vector3 tResultantAcceleration = new Vector3();


			//get the ZX plane and solve for horizontal air resitance
			Vector3 tZXVelocity = new Vector3(tLocalVelocity.x,0f,tLocalVelocity.z);
			float tZXAeroAngle = Vector3.Angle(tZXVelocity,Vector3.forward);
			mZXAeroScalar = mAerodynamicZXAnglePerformace.Evaluate (tZXAeroAngle);
			if (Vector3.Angle (tZXVelocity, Vector3.right) < 90f)
			{
				mZXAeroScalar *= -1f;
			}

			Vector3 ZXAeroAcceleration = mTransform.right * mZXAeroScalar * mAerodynamicStrength * (tZXVelocity.magnitude/mThrustEngine.MaxSpeed);
			tResultantAcceleration += ZXAeroAcceleration;

			//get the YZ plane and solve for vertical air resitstance
			Vector3 tYZVelocity = new Vector3(0f,tLocalVelocity.y,tLocalVelocity.z);
			float tYZAeroAngle = Vector3.Angle (tYZVelocity, Vector3.forward);
			mYZAeroScalar = mAerodynamicYZAnglePerformace.Evaluate (tYZAeroAngle);
			if (Vector3.Angle (tYZVelocity, Vector3.up) < 90f)
			{
				mYZAeroScalar *= -1f;
			}

			Vector3 YZAeroAcceleration = mTransform.up * mYZAeroScalar * mAerodynamicStrength *(tYZVelocity.magnitude/mThrustEngine.MaxSpeed);
			tResultantAcceleration += YZAeroAcceleration;


			/*if (mRigidBody.velocity < mMinAeroSpeed)
			{
				mAeroFactor = 0f;
			}*/

			//APPLY FORCES
			if (tResultantAcceleration.sqrMagnitude > mMaxAeroAcceleration * mMaxAeroAcceleration)
			{
				tResultantAcceleration = tResultantAcceleration.normalized * mCurrentMaxAeroAcceleration;
			}

			mRigidBody.AddForce (tResultantAcceleration, ForceMode.Acceleration);
		}

	}
		

	#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		Vector3 tForwardFlat = transform.forward;
		tForwardFlat.y = 0f;
		Vector3 tRightFlat = transform.right;
		tRightFlat .y = 0f;
		Vector3 tPosition = transform.position;

		Handles.color = Color.white;
		Handles.DrawDottedLine (tPosition, tPosition + (mResultantAcceleration.normalized*15f), 2f); 
		Handles.color = Color.blue;
		Handles.DrawDottedLine (tPosition, tPosition + (mResultantAcceleration.normalized*(mResultantAcceleration.magnitude/mCurrentMaxAeroAcceleration)*15f), 2f); 

	}
	#endif
}
