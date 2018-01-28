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
    float jumpVelocity;
    float velocityXSmoothing;

    Vector3 velocity;

    Controller2D controller2D;
    

	// Use this for initialization
	void Start () {
        controller2D = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;


    }
	
	// Update is called once per frame
	void Update () {

        if (controller2D.collisions.above || controller2D.collisions.below) {
            velocity.y = 0;
        } 

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (Input.GetKeyDown(KeyCode.Space) && controller2D.collisions.below) {
            
            velocity.y = jumpVelocity;            
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
