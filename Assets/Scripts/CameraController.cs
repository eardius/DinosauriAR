using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float WalkSpeed;
    public float RotateSpeed;

    private float horizontalMovement;
    private float verticalMovement;
    private float rotation;

    void Start () {
		
	}
	
	void Update () {
        horizontalMovement = Input.GetAxis("Horizontal") * Time.deltaTime * WalkSpeed; ;
        verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime * WalkSpeed;
        rotation += Input.GetAxis("Mouse X") * Time.deltaTime * RotateSpeed;
	}

    private void FixedUpdate()
    {
        //transform.Rotate(0, horizontalMovement, 0);
        transform.Translate(horizontalMovement, 0, verticalMovement);
        transform.eulerAngles = new Vector3(0, rotation, 0);
    }
}
