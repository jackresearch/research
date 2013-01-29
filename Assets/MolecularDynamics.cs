using UnityEngine;
using System.Collections;

public class MolecularDynamics : MonoBehaviour {

	public static float x_0 = 9f;
	public static float v_0 = 0f;
	public float delta_t = 0.1f;
	public float k = 1f;
	public float l = 1f;
	public float m = 1f;
	
	private float currentAccel = 0f;
	private float currentPos = x_0;
	private float currentVelo = v_0;
	private float currentTime = 0f;
	
	void Start() {
		transform.position = new Vector3(x_0, 0,0);	
	}
	
	void Update() {
		if(Input.GetKeyDown(KeyCode.RightArrow)) {
			IterateStep();	
		}
	}
	
	void IterateStep() {
		currentAccel = (-k*(currentPos - l))/m;
		Debug.Log ("a("+currentTime+") = "+currentAccel);
		currentTime += delta_t;
		currentPos = currentPos + currentVelo*delta_t + 0.5f*currentAccel*Mathf.Pow(delta_t, 2);
		currentVelo = currentVelo + currentAccel*delta_t;
		transform.position = new Vector3(currentPos, 0,0);
		Debug.Log ("x("+currentTime+") = "+currentPos);
		Debug.Log ("v("+currentTime+") = "+currentVelo);

	}
	
}
