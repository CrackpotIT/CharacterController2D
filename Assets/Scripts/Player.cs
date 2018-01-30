using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .3f;
    public float moveSpeed = 6;
    public float multiplierComicFall = 1.07f;
    public float multiplierSmashFall = 2f;


    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallJumpLeap;

    public float wallSlideSpeedMax = 1;
    public float wallStickTime = .25f;

    float timeToWallUnstick;

    float accelerationTimeAirborne = .1f;
    float accelerationTimeGrounded = .005f;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    float velocityXSmoothing;

    bool wallSliding;
    int wallDirX;

    Vector3 velocity;

    Controller2D controller2D;
    Vector2 directionalInput;

	// Use this for initialization
	void Start () {
        controller2D = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex,2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(Mathf.Abs(gravity) * minJumpHeight);
    }


    // Update is called once per frame
    void Update() {
        CalculateVelocity();
        HandleWallSliding();

        controller2D.Move(velocity * Time.deltaTime, directionalInput);

        if (controller2D.collisions.above || controller2D.collisions.below) {
            velocity.y = 0;
        }
    }



    public void SetDirectionalInput (Vector2 input) {
        directionalInput = input;
    }

    public void OnJumpInputDown() {
        if (wallSliding) {
            if (wallDirX == directionalInput.x) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
                Debug.Log("wallJumpClimp");
            } else if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
                Debug.Log("wallJumpOff");
            } else {
                velocity.x = -wallDirX * wallJumpLeap.x;
                velocity.y = wallJumpLeap.y;
                Debug.Log("wallJumpLeap");
            }
        }
        if (controller2D.collisions.below) {
            velocity.y = maxJumpVelocity;
        }
    }


    public void OnJumpInputUp() {
        if (velocity.y > minJumpVelocity) {
            velocity.y = minJumpVelocity;
        }
    }    


    void HandleWallSliding() {
        wallDirX = (controller2D.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller2D.collisions.left || controller2D.collisions.right) && !controller2D.collisions.below && velocity.y < 0) {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocity.x = 0;
                velocityXSmoothing = 0;
                if (directionalInput.x != wallDirX && directionalInput.x != 0) {
                    timeToWallUnstick += Time.deltaTime;
                } else {
                    timeToWallUnstick = wallStickTime;
                }
            } else {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    void CalculateVelocity() {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller2D.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        if (velocity.y < 0) {
            velocity.y *= multiplierComicFall;

            if (directionalInput.y == -1) {
                velocity.y *= multiplierSmashFall;
            }
        }
    }
}
