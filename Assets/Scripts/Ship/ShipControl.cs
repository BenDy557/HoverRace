using UnityEngine;
using System.Collections;

public class ShipControl : MonoBehaviour
{

	private Rigidbody mRigidBody;
	private GameObject mShip;
	//public float mAccelerationForce;//force magnitude
	//public float mSteerForce;//force magnitude
	public float mAcceleration;

	public float mSteeringAcceleration;
	public float mSteeringDecceleration;

	public float mMaxSpeed;
	public float mMaxSteerSpeed;

	private float mAccelerateInput = 0.0f;
	private float mSteerInput = 0.0f;

	public float mAirResistanceForce;
	public float mAerodynamicForce;

	// Use this for initialization
	void Start ()
    {
		mRigidBody = transform.parent.gameObject.GetComponent<Rigidbody>();	
		mShip = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		mAccelerateInput = Input.GetAxis ("Accelerate");
		mSteerInput = Input.GetAxis ("Horizontal");
	}

	void FixedUpdate()
	{
		if (mRigidBody.velocity.sqrMagnitude < mMaxSpeed*mMaxSpeed)
		{
			mRigidBody.AddRelativeForce(new Vector3(0.0f,0.0f,mAccelerateInput*mAcceleration),ForceMode.Acceleration);
		}
			
		if ((mRigidBody.angularVelocity.y > 0.0f && mSteerInput < 0.0f) || (mRigidBody.angularVelocity.y < 0.0f && mSteerInput > 0.0f))
		{
			//Decceleration
			mRigidBody.AddRelativeTorque (new Vector3 (0.0f, mSteerInput * mSteeringDecceleration, 0.0f), ForceMode.Acceleration);	
		}
		else if(mSteerInput == 0.0f)
		{
			//Debug.Log ("Velocity" + mRigidBody.angularVelocity.y);

			/*
			if (mRigidBody.angularVelocity.y > 0.0f)
			{
				mRigidBody.AddRelativeTorque (new Vector3 (0.0f, -mSteeringDecceleration, 0.0f), ForceMode.Acceleration);
			}
			else if (mRigidBody.angularVelocity.y < 0.0f)
			{
				mRigidBody.AddRelativeTorque (new Vector3 (0.0f, mSteeringDecceleration, 0.0f), ForceMode.Acceleration);
			}
			*/
		}
		else
		{
			if (Mathf.Abs (mRigidBody.angularVelocity.y) < mMaxSteerSpeed)
			{
			//Acceleration
				mRigidBody.AddRelativeTorque (new Vector3 (0.0f,mSteerInput *mSteeringAcceleration, 0.0f), ForceMode.Acceleration);	
			}
		}

		Vector3 tVelocity = mRigidBody.velocity;
		//tVelocity = Vector3.RotateTowards (tVelocity, mShip.transform.forward, 0.01f, 0.0f);
		//mRigidBody.velocity = new Vector3(tVelocity.x, tVelocity.y, tVelocity.z);




		//AIR RESISTANCE
		float tAirResistanceFactor = (Vector3.Angle(mRigidBody.velocity.normalized,mShip.transform.forward)/90.0f);

		Vector3 tAirResitanceForce = -mRigidBody.velocity.normalized * tAirResistanceFactor * mAirResistanceForce;
		Vector3 tAerodynamicForce = mShip.transform.forward * tAirResistanceFactor * mAerodynamicForce;

		//Vector3 tAirflowResultForce = tAirResitanceForce;
		Vector3 tAirflowResultForce = (tAirResitanceForce + tAerodynamicForce);
		tAirflowResultForce = tAirflowResultForce *  (mRigidBody.velocity.magnitude / mMaxSpeed);
		mRigidBody.AddForce (tAirflowResultForce, ForceMode.Force);


	}
}
