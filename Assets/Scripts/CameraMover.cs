using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

	public float movingScale = 0.6f;
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3(movingScale* Input.GetAxis("Horizontal"),movingScale*Input.GetAxis("Vertical"),0);
	}
}
