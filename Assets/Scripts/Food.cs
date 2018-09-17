using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Food : MonoBehaviour {

    public Eatable WhichFoodIsIt;

    [HideInInspector]
    public bool inOtherWorld;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {   
        //checks if user is outside/inside other world -> problem: when user is outside but food is inside
        switch ((CompareFunction)System.Enum.ToObject(typeof(CompareFunction), Shader.GetGlobalInt("_StencilTest")))
        {
            case CompareFunction.Equal:
                inOtherWorld = false;
                meshRenderer.receiveShadows = false;
                break;
            case CompareFunction.NotEqual:
                inOtherWorld = true;
                meshRenderer.receiveShadows = true;
                break;
            default:
                Debug.Log("Compare Function had other values than expected. Default: Start in outside world.");
                inOtherWorld = false;
                break;
        }
    }
}
