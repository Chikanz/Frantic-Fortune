using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThumbController : MonoBehaviour
{
	public ScrollRect scrollRect;
	public Animation thumb;
	public string stateName = "Animation_Thumb";
	public float cycleDistance;
	public float pingpong;


	// Start is called before the first frame update
	void Start()
    {
    }

    // Update is called once per frame
    void Update()
	{
		//thumb.transform.localEulerAngles = Vector3.forward * (rotationStart + ((Mathf.PingPong(scrollRect.verticalScrollbarSpacing, cycleLength)) * rotationOffset));

		thumb[stateName].normalizedSpeed = 0;
		pingpong = Mathf.PingPong(scrollRect.verticalNormalizedPosition * scrollRect.content.sizeDelta.y, cycleDistance);
		thumb[stateName].normalizedTime = pingpong / cycleDistance;
	}
}
