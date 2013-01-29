using UnityEngine;
using System.Collections;

public class SpinScript : MonoBehaviour {
	
	public float accelTime = 3f;
	public float accelConstant = 0.5f; //lower is faster, the function is exponential
	public float maxAngularVelocity = 10f;
	public bool isAccel = true;
	public bool isDeaccel = false;
	public float deaccelTime = 4f;
	public float deaccelConstant = 0.001f;
	public float finalTargetRotation = 90f;
	

	void Awake() {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
		timeOffset = Time.time;
		StartCoroutine(SpinLoop());
	}
	
	IEnumerator SpinLoop() {
		yield return new WaitForSeconds(accelTime);
		isAccel = false;
		isDeaccel = true;
		timeOffset = Time.time;
		yield return new WaitForSeconds(deaccelTime);
		isDeaccel = false;
	}
		
	void FixedUpdate() {
		if (isAccel) {
			rigidbody.AddTorque(-transform.up * maxAngularVelocity * accelConstant*(Time.time - timeOffset));
		}
		if (isDeaccel) {
			float magnitude = Mathf.Clamp(-deaccelConstant * Mathf.Pow((Time.time - timeOffset),2f) ,0,maxAngularVelocity);
			rigidbody.AddTorque(transform.up * -magnitude);
			Debug.Log (rigidbody.angularVelocity);
		}
	}
	
	bool beginDampening = false;
	float timeOffset = 0f;
	public float dampeningConstant = 1f;
	public float dampeningAmplitude = 4f;
	
	void Update() {
		if (!isAccel && !isDeaccel) {
			float diff = (finalTargetRotation - 5f - transform.localEulerAngles.y);
			// Might have to change range of catch from -5<x<5, doesn't always work at higher velocities
			if (diff < 5f && diff > -5f && !beginDampening) {
				beginDampening = true;
				timeOffset = Time.time;		
			}
			if (beginDampening) {
				rigidbody.angularVelocity = new Vector3(0,Mathf.Exp(-dampeningConstant*(Time.time - timeOffset))*Mathf.Cos((Time.time - timeOffset)*dampeningAmplitude*Mathf.PI),0);	
			}
		}
	}
}
