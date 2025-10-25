using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPhysics : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;

    public float speed;

    [Header("Arrow Aerodynamics")]
    public float frontDrag = 0.1f;      // Drag when arrow points forward (minimal)
    public float sideDrag = 2.0f;       // Drag when arrow is sideways (maximum)
    public float angularDrag = 5.0f;    // Rotational drag
    public float stabilizingForce = 10.0f; // Force that tries to align arrow with velocity

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Set up rigidbody for realistic arrow physics
        rb.gravityScale = 1f; // Arrows are affected by gravity
        rb.linearDamping = 0f; // We handle drag manually
        rb.angularDamping = 0f; // We handle angular drag manually
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.magnitude <= 2f)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        ApplyAerodynamicForces();
    }

    private void ApplyAerodynamicForces()
    {
        Vector2 velocity = rb.linearVelocity;

        // Get arrow's forward direction (where it's pointing)
        Vector2 arrowForward = transform.right; // Assuming arrow points along negative X axis

        // Calculate the angle between arrow direction and velocity direction
        Vector2 velocityDirection = velocity.normalized;
        float dot = Vector2.Dot(arrowForward, velocityDirection);

        // Convert dot product to angle (0 = aligned, 1 = perpendicular)
        float alignmentFactor = Mathf.Abs(dot); // 1 when aligned, 0 when perpendicular
        float perpendicularFactor = 1f - alignmentFactor; // 0 when aligned, 1 when perpendicular

        // Calculate drag coefficient based on orientation
        float currentDrag = Mathf.Lerp(sideDrag, frontDrag, alignmentFactor);

        // Apply drag force opposite to velocity
        Vector2 dragForce = -velocity * currentDrag * velocity.magnitude;
        rb.AddForce(dragForce);

        // Apply stabilizing force to align arrow with velocity direction
        Vector3 targetUp = new Vector3(velocityDirection.x, velocityDirection.y, 0);
        Vector3 currentUp = transform.up;

        // Calculate the torque needed to align the arrow
        Vector3 torqueAxis = Vector3.Cross(currentUp, targetUp);
        float torqueMagnitude = torqueAxis.z * stabilizingForce * perpendicularFactor;

        rb.AddTorque(torqueMagnitude);

        // Apply angular drag to prevent excessive spinning
        rb.angularVelocity *= (1f - angularDrag * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }
}
