using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class HandIntro : MonoBehaviour
{
    public float lerpSpeed;

    private Vector3 goalPos;

    private Vector3 inpos;
    private Vector3 outpos;

    public float outHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        inpos = transform.position;
        outpos = inpos + Vector3.up * outHeight;
        
        transform.position = outpos;
        goalPos = outpos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, goalPos, lerpSpeed);
    }

    public void TogglePos(bool inView)
    {
        goalPos = inView ? inpos : outpos;
    }
}
