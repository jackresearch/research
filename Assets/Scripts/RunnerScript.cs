using UnityEngine;
using System.Collections;

public class RunnerScript : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 19.0F;
	public float doubleJumpSpeed = 5.0f;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
	private bool isDoubleJumping = false; 
	
    void Update() {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
			isDoubleJumping = false;
            moveDirection = new Vector3(1, 0, 0);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
            
        } else {
            if (Input.GetButton("Jump") && !isDoubleJumping) {
				isDoubleJumping = true;
                moveDirection.y = doubleJumpSpeed;
			}
		}
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
		Camera.main.transform.position = new Vector3(this.transform.position.x + 5, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }
	
}
