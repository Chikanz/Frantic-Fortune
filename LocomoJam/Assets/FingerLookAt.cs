using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerLookAt : MonoBehaviour
{

    public Vector2 thumbSizes = new Vector2(1, 1.2f);
    public float scaleEasing = 0.1f;
    private float currentScale;

    private float startScale;
    
    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //look at
        var target = Input.mousePosition;
        transform.up = (target - transform.position);
        
        //scale
        var goal = Input.GetMouseButton(0) ? 1 : 0;
        currentScale = Mathf.Lerp(currentScale, goal, scaleEasing);
        transform.localScale = Vector3.one * Mathf.Lerp(thumbSizes.x * startScale, thumbSizes.y * startScale, currentScale);
    }
}
