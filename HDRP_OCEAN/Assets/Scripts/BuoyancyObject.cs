using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class BuoyancyObject : MonoBehaviour
{
    public Transform[] floaters;

    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;

    public float airDrag = 0;
    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;
    public float pushPower = 1f;

    //public float waterHeight = 0;
    OceanManager oceanManager;

    Rigidbody rb;
    bool underwater;

    int floatersUnderWater;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        oceanManager = FindObjectOfType<OceanManager>();

    }

    void FixedUpdate()
    {
        floatersUnderWater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float difference = floaters[i].position.y - oceanManager.WaterHeightAtPosition(floaters[i].position);

            if (difference < 0)
            {
                rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);

                if (!underwater)
                {
                    underwater = true;
                    floatersUnderWater += 1;
                    SwitchState(true);
                }
            }
        }
        
        if (underwater && floatersUnderWater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            rb.drag = underWaterDrag;
            rb.angularDrag = underWaterAngularDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }
    }
}
