using UnityEngine;
using System.Collections;

public class Wheel : MonoBehaviour {



    public bool mContact;
    public float mTraction;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {


	}

    void OnCollisionEnter(Collision collisionInfo)
    {
        mContact = true;

        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        //mContact = false;
    }
    
    void OnCollisionStay(Collision collision)
    {
        mContact = true;
    }
}
