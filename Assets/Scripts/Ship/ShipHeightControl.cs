using UnityEngine;
using System.Collections;

public class ShipHeightControl : MonoBehaviour
{
    Rigidbody mRigidBody;

    private bool mNearTrack;
    [SerializeField]private bool mLanding;

    public float mTargetHeightRatio;

    public float mMaxHoverDistance;

    public AnimationCurve mForceModifierCurve;

    public float mDamping;
    private float mPrevHoverDist = 0.0f;

    //LANDING THINGS
    float mLandingAcceleration;
    float mLandingForce;


    private Transform mRepulsorMain;
    private float mForceModifier; 

	// Use this for initialization
    void Start() {
        mRigidBody = GetComponent<Rigidbody>();

        //SetsRepulsors
        Transform []childList =  GetComponentsInChildren<Transform>();
        foreach(Transform item in childList )
        {
            if(item.CompareTag("RepulsorMain"))
            {
                mRepulsorMain = item;
            }
        }

        Mathf.Clamp(mTargetHeightRatio, 0.0f, 1.0f);

        mNearTrack = true;
	}

    // Update is called once per frame
    void Update()
    {
	
	}

    void FixedUpdate()
    {
        RaycastHit hit;
        Ray tempRay = new Ray(mRigidBody.worldCenterOfMass, -gameObject.transform.up);

        //Casts ray below ship, to check for track
        if (Physics.Raycast(tempRay, out hit, mMaxHoverDistance+mRigidBody.velocity.magnitude))
        {
            if (hit.transform.gameObject.CompareTag("HoverMaterial"))
            {
                if (hit.distance < mMaxHoverDistance)
                {
                    float tempHoverDiff = hit.distance - mPrevHoverDist;

                    float tempLandingSpeed;

                    //Works out the acceleration needed to stop the ship from hitting the ground
                    if (!mNearTrack)
                    {
                        mNearTrack = true;

                        mLanding = true;
                        

                        tempLandingSpeed = tempHoverDiff * (1.0f / Time.deltaTime);
                        Debug.Log("tempLandingSpeed" + tempLandingSpeed);

                        mLandingAcceleration = (-tempLandingSpeed) / (hit.distance / tempLandingSpeed);
                        Debug.Log("mLandingAcceleration" + mLandingAcceleration);

                        mLandingForce = mRigidBody.mass * mLandingAcceleration;
                        Debug.Log("mLandingForce" + mLandingForce);
                    }
                    


                    {
                        mForceModifier = 1.0f;

                        float tempRatioDifference = mTargetHeightRatio - (hit.distance / mMaxHoverDistance);

                        float tempExtraForce = mForceModifierCurve.Evaluate(tempRatioDifference);

                        float tempDamping = mDamping;

                        //if (tempExtraForce < 0 && mLanding)
                        //{
                        //    tempExtraForce = 0.0f;
                        //    tempDamping = 0.0f;
                        //}

                        mForceModifier += tempExtraForce;
                    }




                    Debug.Log("ForceModifier" + mForceModifier);


                    Vector3 tempHoverForce = tempRay.direction * (mRigidBody.mass * Physics.gravity.y * mForceModifier);
                    if (mLanding)
                    {
                        Vector3 tempLandingHoverForce = (tempRay.direction * (mRigidBody.mass * Physics.gravity.y)) + (tempRay.direction * mLandingForce);

                        /*
                        if (tempHoverForce. > tempLandingHoverForce.sqrMagnitude)
                        {
                            mLanding = false;
                        }
                        else
                        {
                            tempHoverForce = tempLandingHoverForce;
                        }
                         * */

                        Debug.Log("HoverForce" + tempHoverForce);
                        Debug.Log("LandingForce" + tempLandingHoverForce);

                        tempHoverForce = tempLandingHoverForce;
                    }

                   
                   



                    //HOVER FORCE
                    mRigidBody.AddForceAtPosition(tempHoverForce, tempRay.origin);

                }
                else
                {
                    mNearTrack = false;
                    mLanding = false;
                }
                mPrevHoverDist = hit.distance;
            }
        }
        else
        {
            mNearTrack = false;
        }

        Debug.DrawRay(tempRay.origin, tempRay.direction * mMaxHoverDistance, Color.red);
        Debug.DrawRay(tempRay.origin, tempRay.direction * hit.distance, Color.green);
        Debug.DrawRay(tempRay.origin, tempRay.direction * (mMaxHoverDistance * mTargetHeightRatio), Color.white);
        
    }
}
