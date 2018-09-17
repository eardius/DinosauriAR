using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalController : MonoBehaviour {

    public Transform device;

    private bool isColliding;
    private bool inOtherWorld;
    private bool wasInFrontOfPortal;

	void Start () {

        //check where the user started
        switch((CompareFunction)System.Enum.ToObject(typeof(CompareFunction), Shader.GetGlobalInt("_StencilTest")))
        {
            case CompareFunction.Equal:
                inOtherWorld = false;
                break;
            case CompareFunction.NotEqual:
                inOtherWorld = true;
                break;
            default:
                Debug.Log("Compare Function had other values than expected. Default: Start in outside world.");
                inOtherWorld = false;
                break;
        }
    }
	
	void Update () {

        //while colliding with portal checks if user gets behind the portal at some time
        if (isColliding)
        {
            bool isInFrontOfPortal = DeviceIsInFront();

            if((wasInFrontOfPortal && !isInFrontOfPortal) || (!wasInFrontOfPortal && isInFrontOfPortal))
            {
                switchStencilTest();
            }
            wasInFrontOfPortal = isInFrontOfPortal;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.transform != device)
        {
            return;
        }
        wasInFrontOfPortal = DeviceIsInFront();
        isColliding = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform != device)
        {
            return;
        }
        isColliding = false;
    }

    private void switchStencilTest()
    {
        //if previously inOtherWorld, switch to NotEqual, otherwise switch to Equal
        CompareFunction stencilTest = inOtherWorld ? CompareFunction.Equal : CompareFunction.NotEqual;
        Shader.SetGlobalInt("_StencilTest", (int)stencilTest);
        inOtherWorld = !inOtherWorld;
    }

    private bool DeviceIsInFront()
    {
        //the position of the near clip plane is needed, otherwise the portal will not be rendered when it is closer to the camera than the near clip plane and therefore the stencil buffer is not used...
        Vector3 devicePosition = device.position + device.forward * Camera.main.nearClipPlane;

        //transforms position of portal from world space to local space -> relative to the camera
        Vector3 position = transform.InverseTransformPoint(devicePosition);

        //if z of portal is >= 0: device is in front, otherwise behind
        return position.z >= 0 ? true : false;
    }
}
