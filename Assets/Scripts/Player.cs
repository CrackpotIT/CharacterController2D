using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float jumpHeight = 2;
    public float timeToJumpApex = .3f;
    public float moveSpeed = 6;

    float accelerationTimeAirborne = .01f;
    float accelerationTimeGrounded = .005f;

    float gravity;
    float jumpVelocitySmall;
    float velocityXSmoothing;

    Vector3 velocity;

    Controller2D controller2D;

    bool jumpKeyPressed = false;
    float jumKeyPressedTime = 0;

	// Use this for initialization
	void Start () {
        controller2D = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
        jumpVelocitySmall = Mathf.Abs(gravity) * timeToJumpApex;


    }
	
	// Update is called once per frame
	void Update () {

        if (controller2D.collisions.above || controller2D.collisions.below) {
            velocity.y = 0;
        } 

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        

        if (Input.GetKeyDown(KeyCode.Space) && controller2D.collisions.below && jumpKeyPressed == false) {
            // initial small jump
            velocity.y = jumpVelocitySmall;
            jumpKeyPressed = true;
            jumKeyPressedTime = Time.time;
        } else {
            if (jumpKeyPressed == true && Input.GetKey(KeyCode.Space)) {
                    
                float keyPressedTime = (Time.time - jumKeyPressedTime);
                if (keyPressedTime < 1f) {
                    float smallInterval = 0.4f;
                    velocity.y += smallInterval;
                    Debug.Log("Added small jump interval: " + velocity.y);
                }
            }
        }
        


        // Wenn Jump key losgelassen wird, wieder in den ursprungszustand
        if (Input.GetKeyUp(KeyCode.Space) && jumpKeyPressed) {
            Debug.Log("Jump Key losgelassen");
            jumpKeyPressed = false;
        }

        float targetVelocityX = input.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller2D.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;


        if (velocity.y < 0) {
            velocity.y = velocity.y * 1.05f;
        }
        
        controller2D.Move(velocity * Time.deltaTime);
	}
}
