using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    public float magnitude = 1;
    public float frequency = 0.5f;

    public EasingFunction.Ease easeType;

    public float offset;

    private Vector3 localStart;
    
    // Start is called before the first frame update
    void Start()
    {
        localStart = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        EasingFunction.Function Easing = EasingFunction.GetEasingFunctionDerivative(easeType);
        var i = Mathf.PingPong(Time.time * frequency + offset, 1);
        var y = Easing(0, magnitude, i);
        transform.localPosition = new Vector3(localStart.x,y,localStart.y);
    }
    
}
