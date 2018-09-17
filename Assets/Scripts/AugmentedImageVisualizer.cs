using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentedImageVisualizer : MonoBehaviour {

    /// <summary>
    /// The AugmentedImage to visualize. Get it from the SceneController.
    /// </summary>
    public AugmentedImage Image;

    //when childobjects were destroyed waits "timer" (in seconds) to destory visualizer
    public float TimerForDestroyingObject;
    private float timer = 0;

    //The prefabs to visualize on different images, which prefab to use for which image depends on position in list -> position 0: image in database with position: 0
    public List<GameObject> prefabGameObjects;
    
    private GameObject augmentedObject = null;
    private bool augmentedObjectIsDestroyed = false;


    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update()
    {
        if (Image == null || Image.TrackingState != TrackingState.Tracking)
        {
            augmentedObject.SetActive(false);
            return;
        }

        if (augmentedObject != null && augmentedObject.transform.childCount == 0)
        {
            timer += Time.deltaTime;
            if(timer >= TimerForDestroyingObject)
            {
                augmentedObjectIsDestroyed = true;
            }
        }

        //if no prefab was already created, create one
        if (augmentedObject == null)
        {
            augmentedObject = Instantiate(prefabGameObjects[Image.DatabaseIndex]);
            augmentedObject.transform.parent = this.transform;
        }

        //change position to image position
        augmentedObject.transform.localPosition = Vector3.zero;

        augmentedObject.SetActive(true);
    }

    public bool isAugmentedObjectDestroyed()
    {
        return augmentedObjectIsDestroyed;
    }
}
