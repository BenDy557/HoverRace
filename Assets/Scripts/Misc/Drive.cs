using UnityEngine;
using System.Collections;

public class Drive : MonoBehaviour {

    public Rigidbody mRigidBody;

    public GameObject mBackRightWheel;
    public GameObject mBackLeftWheel;
    public GameObject mFrontRightWheel;
    public GameObject mFrontLeftWheel;
                     
    private float mSpeed;
    public float mSpeedMax;
    public float mAcceleration;
    public float mDeceleration;

    private float mTurnSpeed;
    public float mSteerAngle;
    public float mSteerSpeedMax;
    public float mSteerSpeedAcceleration;
    public float mSteerSpeedDeceleration;

    

	// Use this for initialization
	void Start () {

        mRigidBody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
        
        float accelerationAmount = Input.GetAxis("Accelerate");
        float steerAmount = Input.GetAxis("Horizontal");

        mFrontRightWheel.GetComponent<WheelCollider>().motorTorque = accelerationAmount * mAcceleration;
        mFrontLeftWheel.GetComponent<WheelCollider>().motorTorque = accelerationAmount * mAcceleration;

        mFrontRightWheel.GetComponent<WheelCollider>().steerAngle = steerAmount * mSteerAngle;
        mFrontLeftWheel.GetComponent<WheelCollider>().steerAngle = steerAmount * mSteerAngle;

        /*
        mSpeed += (accelerationAmount * mAcceleration) * Time.deltaTime;
        
        mSpeed = Mathf.Clamp(mSpeed, -mSpeedMax / 2, mSpeedMax);
        Debug.Log(mSpeed);

        if (steerAmount != 0)
        {
            mTurnSpeed += (steerAmount * mSteerSpeedAcceleration) * Time.deltaTime;
            
        }
        else
        {
            mTurnSpeed *= 0.8f;
        }

        mTurnSpeed = Mathf.Clamp(mTurnSpeed, -mSteerSpeedMax, mSteerSpeedMax);
        Debug.Log(mTurnSpeed);

        mRigidBody.AddRelativeTorque(0.0f, mTurnSpeed, 0.0f, ForceMode.Acceleration);
        //gameObject.transform.Rotate(0.0f, mTurnSpeed * Time.deltaTime, 0.0f);

        mRigidBody.velocity = new Vector3();
        //mRigidBody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        mRigidBody.AddRelativeForce(mSpeed, 0.0f, 0.0f, ForceMode.VelocityChange);
         */
	}
}
