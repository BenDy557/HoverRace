using UnityEngine;
using System.Collections;

public class Camera_follow : MonoBehaviour {
	
	public Transform follow_position;
    public Transform LookAtPosition;
	public float follow_modifier_x;
	public float follow_modifier_y;
	public float follow_modifier_z;

	public float frames_to_catch_up;
	private Transform m_transform;

	// Use this for initialization
	void Start () {
		m_transform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
	
	
	}

	void FixedUpdate()
	{
		m_transform.position -= (m_transform.position - (follow_position.position + new Vector3(follow_modifier_x,follow_modifier_y,follow_modifier_z)))/frames_to_catch_up;
		//m_transform.Translate(new Vector3(0.0f,10.0f,0.0f));


        m_transform.LookAt(LookAtPosition.position);


	}
}
