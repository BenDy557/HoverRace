using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabiliserEngine : MonoBehaviour
{

	[Header("Manual Control")]
	//[SerializeField]
	//private float mMaxPitchSpeed = 50f;
	[SerializeField]
	private float mPitchAcceleration = 10f;

	//[SerializeField]
	//private float mMaxYawSpeed = 50f;
	[SerializeField]
	private float mYawAcceleration = 10f;

	//[SerializeField]
	//private float mMaxRollSpeed = 50f;
	[SerializeField]
	private float mRollAcceleration = 10f;

	[Header("Alignment")]
	//[SerializeField]
	//private float mMaxAutoPitchSpeed = 80f;
	[SerializeField]
	private float mAutoPitchAcceleration = 10f;
	//[SerializeField]
	//private float mMaxAutoRollSpeed = 80f;
	[SerializeField]
	private float mAutoRollAcceleration = 10f;

	[SerializeField]
	private float mAngleThreshold = 20f;


	//DAMPENING





	private ShipSensors mShipSensors;
	private Transform mTransform;
	private Rigidbody mRigidBody;//the ships rigidbody
	private float mShipWeight;

	//INPUT
	private float mPitchInput = 0f;
	private float mYawInput = 0f;
	private float mRollInput = 0f;


	void Start ()
	{
		mShipSensors = GetComponent<ShipSensors> ();
		mTransform = transform;
		mRigidBody = GetComponent<Rigidbody> ();
		mShipWeight = mRigidBody.mass;
	}

	void Update () 
	{
		mPitchInput = Input.GetAxis ("Pitch");
		mYawInput = Input.GetAxis ("Yaw");
		mRollInput = Input.GetAxis ("Roll");

	}

	void FixedUpdate()
	{
		Vector3 tResultantTorque = new Vector3 ();

		//INPUT
		tResultantTorque += new Vector3 ((mPitchInput * mPitchAcceleration),(mYawInput * mYawAcceleration),(mRollInput*mRollAcceleration));


		//ALIGN

		Vector3 tLocalTargetNormal = Vector3.up;
		if (mShipSensors.mTrackNormal != null)
		{
			tLocalTargetNormal = mTransform.InverseTransformDirection (mShipSensors.mTrackNormal.Value);
		}

		//XZ

		//X PITCH
		float tPitchScalar = 0f;
		Vector3 tLocalTargetNormalYZ = new Vector3(0f,tLocalTargetNormal.y,tLocalTargetNormal.z);
		tPitchScalar = Vector3.Angle (tLocalTargetNormalYZ, Vector3.up)/mAngleThreshold;
		tPitchScalar = Mathf.Clamp01 (tPitchScalar);
		if (tLocalTargetNormal.z < 0f)
		{
			tPitchScalar *= -1f;
		}

		//Z ROLL
		float tRollScalar = 0f;
		Vector3 tLocalTargetNormalXY = new Vector3(tLocalTargetNormal.x,tLocalTargetNormal.y,0f);
		tRollScalar = Vector3.Angle (tLocalTargetNormalXY, Vector3.up)/mAngleThreshold;
		tRollScalar  = Mathf.Clamp01 (tRollScalar);
		if (tLocalTargetNormal.x > 0f)
		{
			tRollScalar *= -1f;
		}

		tResultantTorque += new Vector3 (tPitchScalar * mAutoPitchAcceleration, 0f, tRollScalar * mAutoRollAcceleration);


		mRigidBody.AddRelativeTorque (tResultantTorque,ForceMode.Acceleration);
	}
}
