using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    public PolygonCollider2D collider;
    public float speed = 1;

    public bool inBounds {get; private set; }
    private bool isMoving => velocity.sqrMagnitude != 0;
    public bool isStill { get; private set;}
    
    private RectTransform RT;

    private Vector3 velocity;

    [Tooltip("how long the hand can be still for")]
    public float StillTime = 2;

    private float stillTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        RT = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //move da hand
        inBounds = collider.bounds.Contains(transform.GetChild(0).position);

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        
        velocity = new Vector3(h,v) * speed * Time.deltaTime;
        RT.position += velocity;

        //Movement check for stillness
        if (!isMoving)
        {
            stillTimer += Time.deltaTime;
            if (stillTimer >= StillTime)
            {
                isStill = true;
            }
        }
        else
        {
            stillTimer = 0;
            isStill = false;
        }
    }
}
