using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    public float phase;
    public float depth;
    public float gravity;
    public Vector4 timeScales;
    public Vector3[] waveDirections;
    public float[] amplitudes;

    public float itterations = 5;

    public Transform ocean;
    Material oceanMat;

    private void Start()
    {
        SetVariables();
    }

    void SetVariables()
    {
        oceanMat = ocean.GetComponent<Renderer>().sharedMaterial;
    }

    private void OnValidate()
    {
        if (!oceanMat)
        {
            SetVariables();
        }
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        oceanMat.SetFloat("_Phase", phase);
        oceanMat.SetFloat("_Depth", depth);
        oceanMat.SetFloat("_Gravity", gravity);
        oceanMat.SetVector("_TimeScales", timeScales);

        for (int i = 0; i < waveDirections.Length; i++)
        {
            oceanMat.SetVector("_Direction" + (i+1).ToString(), waveDirections[i]);
        }

        for (int i = 0; i < amplitudes.Length; i++)
        {
            oceanMat.SetFloat("_Amp" + (i+1).ToString(), amplitudes[i]);
        }
    }

    public float WaterHeightAtPosition(Vector3 position)
    {
        Vector3 objectPos = new Vector3(position.x, 0, position.z);

        for (int i = 0; i < itterations; i++)
        {
            Vector3 _displacementPos = gerstnerDisplacement(objectPos, phase, depth, gravity, waveDirections, amplitudes, timeScales * Time.time);

            Vector3 horizontalDisplacement = new Vector3(_displacementPos.x - objectPos.x, 0, _displacementPos.z - objectPos.z);

            Vector3 newObjectPoint = objectPos - horizontalDisplacement;
            objectPos = newObjectPoint;
        }
        return gerstnerDisplacement(objectPos, phase, depth, gravity, waveDirections, amplitudes, timeScales * Time.time).y;
    }

    // Implimenting the formulae used in the shader.
    private float angularFrequency(float gravity, Vector3 waveDirection, float depth)
    {
        return Mathf.Sqrt(gravity * waveDirection.magnitude * (float)System.Math.Tanh(waveDirection.magnitude * depth));
    }

    private float theta(Vector3 waveDirection, Vector3 position, float depth, float gravity, float time, float phase)
    {
        return (waveDirection.x * position.x) + (waveDirection.z * position.z) - (time * angularFrequency(gravity, waveDirection, depth)) - phase;
    }

    private Vector3 gerstnerWave(Vector3 position, Vector3 waveDirection, float phase, float time, float gravity, float depth, float amplitude)
    {
        float _theta = theta(waveDirection, position, depth, gravity, time, phase);

        // Defining the deplacement vector.
        float x = - Mathf.Sin(_theta) * (amplitude / (float)System.Math.Tanh(waveDirection.magnitude * depth)) * (waveDirection.x / waveDirection.magnitude);

        float y = amplitude * Mathf.Cos(_theta);

        float z = -Mathf.Sin(_theta) * (amplitude / (float)System.Math.Tanh(waveDirection.magnitude * depth)) * (waveDirection.z / waveDirection.magnitude);

        Vector3 waveDisplacement = new Vector3(x, y, z);
        return waveDisplacement;
    }

    // Adding multiple waves together.
    private Vector3 gerstnerDisplacement(Vector3 position, float phase, float depth, float gravity, Vector3[] waveDirections, float[] amplitudes, Vector4 timeScales)
    {
        Vector3 sum = new Vector3 (0,0,0);
        for (int i = 0; i < waveDirections.Length; i++)
        {
            // Instant regret for using a vector4 and not an array for timeScales.
            if (i == 0)
                sum += gerstnerWave(position, waveDirections[i], phase, timeScales.x, gravity, depth, amplitudes[i]);
            else if (i == 1)
                sum += gerstnerWave(position, waveDirections[i], phase, timeScales.y, gravity, depth, amplitudes[i]);
            else if (i == 2)
                sum += gerstnerWave(position, waveDirections[i], phase, timeScales.z, gravity, depth, amplitudes[i]);
            else if (i == 3)
                sum += gerstnerWave(position, waveDirections[i], phase, timeScales.w, gravity, depth, amplitudes[i]);
        }
        return sum += position;
    }
}
