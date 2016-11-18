using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    public Vector3 rotateAmount;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateAmount.x * Time.deltaTime, rotateAmount.y * Time.deltaTime, rotateAmount.z * Time.deltaTime);
	}
}
