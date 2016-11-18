using UnityEngine;
using System.Collections;

public class HoverEngine : MonoBehaviour {

	public LayerMask mLayerMask;

	[Header("Positional")] 
	public float mHoverRange;//Max distance that the hover engine can exert force on the ship
	public float mMaxForceHoverRange;//The distance at which the hover engine can exert 100% force on the ship
	public float mTargetheight;//The height above the track the ship will always try to move towards

	[Header("Engine")]
	public float mEnginePower;//The maximum force the ship can use through its hover engine
	public float mEngineDampingPower;//The maximum force the ship can use to dampen its velocity towards/away from the track surface


	private Rigidbody mRigidBody;//the ships rigidbody
	private float mShipWeight;//the mass of the ships rigidbody;


	private float mPrevTrackDistance;
	private float mCurrentTrackDistance;
	private float mHoverVelocity;

	// Use this for initialization
	void Start ()
	{
		mRigidBody = transform.parent.gameObject.GetComponent<Rigidbody> ();
		mShipWeight = mRigidBody.mass;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		RaycastHit hit;
		Ray FloorCheckRay = new Ray (mRigidBody.worldCenterOfMass, -gameObject.transform.up);

		Debug.DrawLine(FloorCheckRay.origin, FloorCheckRay.origin + (FloorCheckRay.direction * mHoverRange), Color.green);
		Debug.DrawLine(FloorCheckRay.origin, FloorCheckRay.origin + (FloorCheckRay.direction * mMaxForceHoverRange), Color.red);

		if (Physics.Raycast (FloorCheckRay, out hit, mHoverRange,mLayerMask))
		{
			//Working out speed moving towards/away from track surface
			mPrevTrackDistance = mCurrentTrackDistance;
			mCurrentTrackDistance = hit.distance;
			mHoverVelocity = (mCurrentTrackDistance - mPrevTrackDistance) / Time.deltaTime;


			if (hit.distance < mHoverRange)
			{
				//HOVER FORCE
				float tHoverAmount = 0.0f;

				if (hit.distance < mMaxForceHoverRange)
				{
					tHoverAmount = 1.0f;
				}
				else
				{
					tHoverAmount = 1.0f - ((hit.distance - mMaxForceHoverRange) / (mHoverRange - mMaxForceHoverRange));

				}


				//DampingForce addition
				tHoverAmount += -mHoverVelocity*0.03f;//TODO//magic numbers

				//EngineForce
				tHoverAmount = Mathf.Clamp (tHoverAmount, -0.3f, 1.0f);//TODO//magic numbers
				Vector3 tResultForce = transform.up * (tHoverAmount * mEnginePower);//TODO//transform.up to be replaced with track normals


				//FORCE
				mRigidBody.AddForceAtPosition (tResultForce , mRigidBody.centerOfMass);
			}
		}
	}
}
