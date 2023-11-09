using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCarFollow : MonoBehaviour
{

	public Transform carTransform;
	[Range(1, 10)]
	public float followSpeed = 2;
	[Range(1, 10)]
	public float lookSpeed = 5;
	Vector3 initialCarPosition, currentCarPosition;
	Vector3 initialCameraPosition, absoluteInitCameraPosition, targetPosition, lookDirection;
	Quaternion currentCameraRotation, lookRotation;


	void Start()
	{
		initialCameraPosition = gameObject.transform.position;
		initialCarPosition = carTransform.position;
		absoluteInitCameraPosition = initialCameraPosition - initialCarPosition;
	}

	void FixedUpdate()
	{
		//Look at car
		currentCarPosition = carTransform.position;
		currentCameraRotation = transform.rotation;
		lookDirection = currentCarPosition - transform.position;
		lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
		transform.rotation = Quaternion.Lerp(currentCameraRotation, lookRotation, lookSpeed * Time.deltaTime);

		//Move to car
		targetPosition = absoluteInitCameraPosition + carTransform.transform.position;
		transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

	}

}
