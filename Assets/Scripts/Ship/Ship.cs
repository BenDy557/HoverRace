using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
	//Components
	[SerializeField]
	private HoverEngine m_HoverEngine;

	//Input
	private float m_AccelerateInput = 0f;
	private float m_SteerInput = 0f;

	void Start ()
	{
		
	}
	

	void Update ()
	{
		//INPUT
		m_AccelerateInput = Input.GetAxis ("Accelerate");
		m_SteerInput = Input.GetAxis ("Horizontal");
	}
}
