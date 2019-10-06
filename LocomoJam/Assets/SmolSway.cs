using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmolSway : MonoBehaviour
{
    private Vector3 startPos;
    public float moveMag;
    public float timeFreq = 0.25f;
    public float rotationFactor = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time * timeFreq;
        var perlinx = Mathf.PerlinNoise(t, t + 3);
        var perliny = Mathf.PerlinNoise(t + 6, t + 2);
        var perlinRot = Mathf.PerlinNoise(t+4, t - 1);
        transform.position = startPos + (new Vector3(perlinx, perliny, 0) * moveMag);
        
        transform.rotation = Quaternion.EulerAngles(0,0,perlinRot * rotationFactor);
    }
}
