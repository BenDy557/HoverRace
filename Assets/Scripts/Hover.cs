
using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour {

	Rigidbody m_body;
	public float hover_force_per_unit_weight;//490ish
	public float acc_force_per_unit_weight;//??ish
	public float steer_force_per_unit_weight;//??ish
	public float self_right_force_per_unit_weight;//??ish

	private float hover_force;//force magnitude
	private float acc_force;//force magnitude
	private float steer_force;//force magnitude
	private float self_right_force;//force magnitude

	private float self_right_amount;

	private float hover_force_main;

	private float force_multiplier;

	public Transform repulsor_main;
	public Transform repulsor_back_left;
	public Transform repulsor_back_right;
	public Transform repulsor_front;

	public float repulsion_distance_main;
	public float repulsion_distance_back_left;
	public float repulsion_distance_back_right;
	public float repulsion_distance_front;
	
	private int layerMask = 1 << 8;
	
	// Use this for initialization
	void Start () {
		m_body = GetComponent<Rigidbody>();
		hover_force = hover_force_per_unit_weight*m_body.mass;
		acc_force = acc_force_per_unit_weight*m_body.mass;
		steer_force = steer_force_per_unit_weight*m_body.mass;
		self_right_force = self_right_force_per_unit_weight;

		layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void FixedUpdate()
	{
		RaycastHit hit;

		hit = RayCastHover(repulsor_main,repulsion_distance_main);
		hit = RayCastHover(repulsor_back_left,repulsion_distance_back_left);
		hit = RayCastHover(repulsor_back_right,repulsion_distance_back_right);
		hit = RayCastHover(repulsor_front,repulsion_distance_front);

		//Accelerate force
		m_body.AddRelativeForce(new Vector3(0.0f,0.0f,(acc_force * Input.GetAxis("Accelerate") * m_body.mass))*Time.deltaTime);
		
		//Turning force
		m_body.AddRelativeTorque(new Vector3(0.0f,(steer_force * Input.GetAxis("Horizontal")* m_body.mass),0.0f)*Time.deltaTime);
		//m_body.AddRelativeForce(new Vector3((30 * Input.GetAxis("Horizontal")* m_body.mass),0.0f,0.0f)*Time.deltaTime);
		
		//TURNING RESISTANCE
		//m_body.AddRelativeForce(new Vector3(-100.0f * m_body.mass * m_body.velocity.x,0.0f,0.0f)*Time.deltaTime);

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

		//m_body.rigidbody.velocity.
		//m_body.transform.rotation.y
		//TURNING RESISTANCE

	}

	RaycastHit RayCastHover(Transform repulsor,float repulsor_distance)
	{
		RaycastHit hit;
		float floor_distance;
		float repulsion_force;

		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(repulsor.position, -Vector3.up, out hit, repulsor_distance, layerMask)) 
		{
			Debug.DrawRay(repulsor.position, -Vector3.up *repulsor_distance, Color.green);
			Debug.DrawRay(repulsor.position, -Vector3.up * hit.distance, Color.red);

						floor_distance = hit.distance;
			repulsion_force = (hover_force/floor_distance);
		}
		else
		{
			Debug.DrawRay(repulsor.position, -Vector3.up *repulsor_distance, Color.green);

			repulsion_force = 0;
		}

		m_body.AddForceAtPosition(new Vector3(0.0f,repulsion_force,0.0f)*Time.deltaTime,repulsor.position);
		return hit;
	}
}
