using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBeforeSubmerged = 1f, displacementAmount = 3f;
    public int floaterCount = 1;

    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    OceanManager oceanManager;


    private void Start()
    {
        oceanManager = FindObjectOfType<OceanManager>();
    }
    private void FixedUpdate()
    {
        rb.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        float waveHeight = oceanManager.WaterHeightAtPosition(transform.position);
        if (transform.position.y < waveHeight)
        {
            float displacmentMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rb.AddForceAtPosition(new Vector3(0, Mathf.Abs(Physics.gravity.y) * displacmentMultiplier), transform.position, ForceMode.Acceleration);

            rb.AddForce(displacmentMultiplier * -rb.velocity * waterDrag * Time.deltaTime, ForceMode.VelocityChange);
            rb.AddTorque(displacmentMultiplier * -rb.angularVelocity * waterAngularDrag * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}
