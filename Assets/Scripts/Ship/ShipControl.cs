using UnityEngine;
using System.Collections;

public class ShipControl : MonoBehaviour
{

	Rigidbody m_body;
	public float acc_force_per_unit_weight;//??ish
	public float steer_force_per_unit_weight;//??ish
	public float self_right_force_per_unit_weight;//??ish


	private float acc_force;//force magnitude
	private float steer_force;//force magnitude
	private float self_right_force;//force magnitude

	private float self_right_amount;

	private float hover_force_main;

	private float force_multiplier;

	
	private int layerMask = 1 << 8;
	
	// Use this for initialization
	void Start ()
    {
		m_body = GetComponent<Rigidbody>();

		acc_force = acc_force_per_unit_weight*m_body.mass;
		steer_force = steer_force_per_unit_weight*m_body.mass;
		self_right_force = self_right_force_per_unit_weight;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate()
	{
		RaycastHit hit;

		//Accelerate force
		m_body.AddRelativeForce(new Vector3(0.0f,0.0f,(acc_force * Input.GetAxis("Accelerate") * m_body.mass))*Time.deltaTime);
		
		//Turning force
		m_body.AddRelativeTorque(new Vector3(0.0f,(steer_force * Input.GetAxis("Horizontal")* m_body.mass),0.0f)*Time.deltaTime);

		
		if((m_body.transform.localRotation.z >= 0) && (m_body.transform.localRotation.z < 180))
		{
			self_right_amount = -m_body.transform.localRotation.z/180;
		}
		else if((m_body.transform.localRotation.z < 0) && (m_body.transform.localRotation.z > -180))
		{
			self_right_amount = m_body.transform.localRotation.z/180;
		}
		else
		{
			self_right_amount = 0;
		}

		m_body.AddRelativeTorque(new Vector3(0.0f,0.0f,(self_right_force * self_right_amount * m_body.mass))*Time.deltaTime);

	}
}
