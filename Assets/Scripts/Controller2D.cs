using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    const float skinWidth = .015f;

    public LayerMask collisionMask;

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    public CollisionInfo collisions;


    float horizontalRaySpacing;
    float verticalRaySpacing;


    BoxCollider2D boxCollider2D;
    RaycastOrigins raycastOrigins;


    // Use this for initialization
    void Start () {
        boxCollider2D = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }


    public void Move(Vector3 velocity) {
        UdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0) {
            HorizontalCollisions(ref velocity);
        }
        
        if (velocity.y != 0) {
            VerticalCollisions(ref velocity);
        }        

        transform.Translate(velocity);
    }


    void HorizontalCollisions(ref Vector3 velocity) {

        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);

            if (hit) {
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;

                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;
            }
        }
    }


    void VerticalCollisions(ref Vector3 velocity) {
        
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.blue);


            if (hit) {
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
            }
        }
    }

    void UdateRaycastOrigins() {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2); // shrink bounds for Skinwidth on all sides

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        
    }

    void CalculateRaySpacing() {
        Bounds bounds = boxCollider2D.bounds;
        bounds.Expand(skinWidth * -2); // shrink bounds for Skinwidth on all sides

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, 99);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, 99);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    
	
	struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public void Reset() {
            above = below = false;
            left = right = false;
        }
    }
}
