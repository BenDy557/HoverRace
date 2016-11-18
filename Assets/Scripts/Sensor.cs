using UnityEngine;
using System.Collections;

public class Sensor : MonoBehaviour {

    Ray sensorRay;

	// Use this for initialization
	void Start ()
    {

        sensorRay = new Ray(GetComponent<Transform>().position, GetComponent<Transform>().rotation.eulerAngles);
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(sensorRay.GetPoint(0), sensorRay.direction,Color.red);   
	}

    void FixedUpdate()
    {

    }
}
