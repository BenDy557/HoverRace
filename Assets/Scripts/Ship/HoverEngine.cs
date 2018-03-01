using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Hover engine.
/// The hover engine's job is to counteract the force of gravity and to also keep the craft's distance from the track steady
/// It functions with three seperate component forces
/// AntiGrav: when the craft's up vector is aligned with force of gravity counteracts the force of gravity and no more than that
/// Height Correction: uses force to move the craft towards or away from the detected track distance to reach a target height
/// Dampening: uses force to restrict the vertical movement of the craft relative to the track in order to stabilise it
/// </summary>
public class HoverEngine : MonoBehaviour
{

	[Header("Engine")]
	public float mEnginePower;//The maximum force the ship can use through its hover engine
	private float mMaxHoverAcceleration = 0f;//worked out from engine power and ship mass
	private float mMaxAntiGravAcceleration = 0f;//worked out from engine power and ship mass
	private float mMaxHeightCorrectionAcceleration = 0f;//a fraction of "mMaxAntiGravAcceleration"

	[SerializeField]
	float mHoverAngleDeadZone = 35f;//Angle difference where hover force is still at full effectiveness
	[SerializeField]
	float mHoverAngleMax = 50f;//Angle difference where hover force has completely decayed and is no longer acting

	public bool mEnableAntiGravForce = true;
	public bool mEnableHeightCorrectionForce = true;
	public bool mEnableDampeningForce = true;

	//public float mEngineDampingPower;//TODO//?? do i need this//The maximum force the ship can use to dampen its velocity towards/away from the track surface

	private ShipSensors mShipSensors;
	private Transform mTransform;
	private Rigidbody mRigidBody;//the ships rigidbody
	private float mShipWeight;//TODO//??do i need this??//the mass of the ships rigidbody;
	//private HoverStabiliser mStabiliser;

	//Dampening
	//private float mPrevTrackDistance;
	//private float mCurrentTrackDistance;
	//private float mHoverVelocity;

	[Header("Positional")] 
	public float mHoverRange;//Max distance that the hover engine can exert force on the ship
	public float HoverRange { get { return mHoverRange; } }
	//public float mMaxForceHoverRange;//The distance at which the hover engine can exert 100% force on the ship
	public float mTargetHeight;//The height above the track the ship will always try to move towards
	public float TargetHeight { get { return mTargetHeight; } }

	private Vector3 mTrackNormal = new Vector3 ();
	public Vector3 TrackNormal { get { return mTrackNormal; } }

	//private bool mIsLanding//TODO//changes how height correction forces work

	void Start()
	{
		mTransform = transform;
		mRigidBody = gameObject.GetComponent<Rigidbody> ();
		//mShip = transform.gameObject;
		mShipWeight = mRigidBody.mass;

		mShipSensors = GetComponent<ShipSensors> ();

		mMaxAntiGravAcceleration = -Physics.gravity.y;
		mMaxHoverAcceleration = mEnginePower / mShipWeight;//the highest acceleration the ship can provide given it's engine power and weight

		mMaxHeightCorrectionAcceleration = mMaxHoverAcceleration * 0.3f;//TODO//value seems arbitrary
	}

	void FixedUpdate ()
	{
		if (mShipSensors.mRelativeVerticalHit != null)
		{
			if (mShipSensors.mTrackDistance != null && mShipSensors.mTrackDistance.Value < mHoverRange)
			{
				//DampingForce addition
				//tHoverAmount += -mHoverVelocity*0.03f;//TODO//magic numbers

				Vector3 tTrackNormal = mTransform.up; //TODO//should actually be track normal

				Vector3 tResultantAcceleration = new Vector3 ();

				//ANTIGRAV FORCE
				if (mEnableAntiGravForce)
				{
					//HOVER SCALE
					float tAntiGravHoverAmount = 0.0f;
					if (mShipSensors.mTrackDistance.Value < mTargetHeight)
					{
						tAntiGravHoverAmount = 1.0f;
					}
					else
					{
						tAntiGravHoverAmount = 1.0f - ((mShipSensors.mTrackDistance.Value - mTargetHeight) / (mHoverRange - mTargetHeight));
					}

					tAntiGravHoverAmount = Mathf.Clamp01 (tAntiGravHoverAmount);

					//HOVER ANGLE
					float tHoverAngleScalar = 1.0f;
					float tHoverAngle = Vector3.Angle (Vector3.up, mTransform.up);
					if ((tHoverAngle - mHoverAngleMax) < 0f)
					{
						if ((tHoverAngle - mHoverAngleDeadZone) < 0)
						{
							//inside deadzone
							tHoverAngleScalar = 1.0f;
						}
						else
						{
							//inside threshold
							tHoverAngleScalar = (1f - ((tHoverAngle - mHoverAngleDeadZone) / (mHoverAngleMax - mHoverAngleDeadZone)));
						}
					}
					else
					{
						//outside threshold
						tHoverAngleScalar = 0f;
					}

					tResultantAcceleration += Vector3.up * (tHoverAngleScalar * tAntiGravHoverAmount * mMaxAntiGravAcceleration);
				}

				//HEIGHT CORRECTION FORCE
				if (mEnableHeightCorrectionForce)
				{
					if (mShipSensors.mTrackDistance.Value < mHoverRange)
					{
						float tHeightCorrectionAmount = 0.0f;

						if (mShipSensors.mTrackDistance.Value > mTargetHeight)
						{
							//bring towards

							tHeightCorrectionAmount = -(mShipSensors.mTrackDistance.Value - mTargetHeight) / (mHoverRange - mTargetHeight);//normalised distance from target height
						}
						else
						{
							//push away
							tHeightCorrectionAmount = -(mShipSensors.mTrackDistance.Value - mTargetHeight) / (mTargetHeight);//normalised distance from target height
						}

						tResultantAcceleration += tTrackNormal * (tHeightCorrectionAmount * mMaxHeightCorrectionAcceleration);

						//Debug.Log (tTrackDistance - mTargetHeight);

						//Debug.Log (tTrackDistance/mHoverRange);
						//Debug.Log ((tTrackDistance)/mHoverRange);

						//mTargetHeight
						//tHeightCorrectionAmount  = (tTrackDistance/mHoverRange);

						//tHeightCorrectionAmount = Mathf.Clamp (tHeightCorrectionAmount,-1f,1f);
					}
				}


				//DAMPENING FORCE
				if (mEnableDampeningForce)
				{
				}

				//CLAMP to engine power
				tResultantAcceleration = Vector3.ClampMagnitude (tResultantAcceleration, mMaxHoverAcceleration);

				//FORCE
				mRigidBody.AddForce (tResultantAcceleration, ForceMode.Acceleration);
			}
		}
	}

	#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		Vector3 tForwardFlat = transform.forward;
		tForwardFlat.y = 0f;
		Vector3 tRightFlat = transform.right;
		tRightFlat .y = 0f;

		//Vector3 tForwardHudPosition = transform.position + (mTransform.forward * 6f);
		Vector3 tPosition = transform.position;
		Handles.color = Color.white;
		Handles.DrawWireDisc (tPosition, Vector3.up, 15f);
		Handles.DrawWireDisc (tPosition, tForwardFlat, 15f);


		Handles.DrawDottedLine (tPosition, tPosition + (tRightFlat * 15f), 5f);
		Handles.DrawDottedLine (tPosition, tPosition + (-tRightFlat * 15f), 5f);

		//HOVER THRESHOLD
		//Handles.DrawDottedLine (tPosition, tPosition + (Quaternion.Euler((tForwardFlat) transform.right * 15f), 5f);
		//Handles.DrawDottedLine (tPosition, tPosition + (-transform.right * 15f), 5f);






		//CURRENT
		Handles.DrawDottedLine (tPosition, tPosition + (transform.right * 15f), 5f);
		Handles.DrawDottedLine (tPosition, tPosition + (-transform.right * 15f), 5f);




	}
	#endif
}
