using UnityEngine;
using System.Collections;

public class HoverEngine : MonoBehaviour {

	[Header("Positional")] 
	public float mHoverRange;//Max distance that the hover engine can exert force on the ship
	public float mMaxForceHoverRange;//The distance at which the hover engine can exert 100% force on the ship
	public float mTargetheight;//The height above the track the ship will always try to move towards

	[Header("Engine")]
	public float mEnginePower;//The maximum force the ship can use through its hover engine

	//public float mEngineDampingPower;//


	private Rigidbody mRigidBody;//the ships rigidbody
	private float mShipWeight;//the mass of the ships rigidbody;


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

		if (Physics.Raycast (FloorCheckRay, out hit, mHoverRange + mRigidBody.velocity.magnitude))
		{
			if (hit.distance < mHoverRange)
			{
				//HOVER FORCE
			}
		}
	}
}
