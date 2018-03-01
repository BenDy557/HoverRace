using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ThrustEngine : MonoBehaviour
{
	[Header("Engine")]
	public float mEnginePower;//The maximum force the ship can use through its hover engine
	[SerializeField]
	private float mMaxSpeed = 50f;
	public float MaxSpeed { get { return mMaxSpeed; } }
	//[SerializeField]
	//private float mForwardAcceleration = 10f;

	[SerializeField]
	private float mRigidBodyDragBase = 0.2f;
	[SerializeField]
	private float mRigidBodyDragAdditional = 1.2f;

	private ShipSensors mShipSensors;
	private Transform mTransform;
	private Rigidbody mRigidBody;//the ships rigidbody
	private HoverEngine mHoverEngine;
	private float mShipWeight;

	//INPUT
	private float mAccelerateInput = 0f;

	void Start ()
	{
		mTransform = transform;
		mRigidBody = GetComponent<Rigidbody> ();
		mHoverEngine = GetComponent<HoverEngine> ();
		mShipSensors = GetComponent<ShipSensors> ();
	}

	void Update ()
	{
		mAccelerateInput = Input.GetAxis ("Accelerate");
	}

	void FixedUpdate()
	{

		float tEngineEffectiveness = 0f;//engine power reduces as the ship gets further from the track surface

		//mHoverEngine.TrackDistance
		if (mShipSensors.mTrackDistance != null)
		{
			if (mShipSensors.mTrackDistance.Value < mHoverEngine.TargetHeight)
			{
				tEngineEffectiveness = 1.0f;
			}
			else
			{
				tEngineEffectiveness = 1.0f - ((mShipSensors.mTrackDistance.Value - mHoverEngine.TargetHeight) / (mHoverEngine.HoverRange - mHoverEngine.TargetHeight));
			}
		}

		//mRigidBody.drag = mRigidBodyDragBase + ((1f-tEngineEffectiveness) * mRigidBodyDragAdditional);

		mRigidBody.AddForce (mTransform.forward * (mAccelerateInput * mEnginePower * tEngineEffectiveness), ForceMode.Force);
	}

	#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		Vector3 tForwardFlat = transform.forward;
		tForwardFlat.y = 0f;
		Vector3 tRightFlat = transform.right;
		tRightFlat .y = 0f;
		Vector3 tPosition = transform.position;

		//Flat
		Handles.color = Color.white;
		Handles.DrawDottedLine (tPosition, tPosition + (tForwardFlat* 15f), 5f);
		Handles.color = Color.red;
		Handles.DrawDottedLine (tPosition, tPosition + (tForwardFlat* mAccelerateInput * 15f), 5f);

		//Actual
		Handles.color = Color.white;
		Handles.DrawDottedLine (tPosition, tPosition + (transform.forward* 15f), 5f);
		Handles.color = Color.red;
		Handles.DrawDottedLine (tPosition, tPosition + (transform.forward* mAccelerateInput * 15f), 5f);



		//Velocity
		if (mRigidBody != null)
		{
			Handles.color = Color.white;
			Handles.DrawDottedLine (tPosition, tPosition + (mRigidBody.velocity.normalized * 15f), 5f);
			Handles.color = Color.yellow;
			Handles.DrawDottedLine (tPosition, tPosition + (mRigidBody.velocity.normalized * (mRigidBody.velocity.magnitude / mMaxSpeed) * 15f), 5f);
		}

	}
	#endif
}
