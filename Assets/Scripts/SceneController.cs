//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif


public class SceneController : MonoBehaviour {

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject PortalPrefab;

    //the instantiated portal prefab
    private GameObject portal = null;

    /// <summary>
    /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;

    public GameObject SearchingForPlanesText;
    public GameObject PortalCreationPossibleText;

    public GameObject CreateNewPortalButtonGameObject;

    private bool portalAlreadyCreated = false;

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    //IMAGE AUGMENTATION
    /// <summary>
    /// A prefab for visualizing an AugmentedImage.
    /// </summary>
    public AugmentedImageVisualizer AugmentedImageVisualizer;
    private Dictionary<int, AugmentedImageVisualizer> m_Visualizers
            = new Dictionary<int, AugmentedImageVisualizer>();
    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();



    void Start () {
        //start in real world
        Shader.SetGlobalInt("_StencilTest", (int)CompareFunction.Equal);
    }


    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        // Hide snackbar when currently tracking at least one plane.
        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
        bool showSearchingUI = true;
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                break;
            }
        }

        SearchingForPlaneUI.SetActive(showSearchingUI);
        SearchingForPlanesText.SetActive(showSearchingUI);

        if (portalAlreadyCreated)
        {
            CreateNewPortalButtonGameObject.SetActive(true);
            PortalCreationPossibleText.SetActive(false);
        }
        else
        {
            SearchingForPlaneUI.SetActive(true);
            CreateNewPortalButtonGameObject.SetActive(false);
            PortalCreationPossibleText.SetActive(!showSearchingUI);
        }


        //IMAGE AUGMENTATION
        //Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do not previously
        // have a visualizer. Remove visualizers for stopped images.
        foreach (var image in m_TempAugmentedImages)
        {
            AugmentedImageVisualizer visualizer = null;

            //when visualizer was added before
            m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);

            //if no visualizer was found
            if (image.TrackingState == TrackingState.Tracking && visualizer == null)
            {
                // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                Anchor anchor = image.CreateAnchor(image.CenterPose);
                visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizer, anchor.transform);
                visualizer.Image = image;
                m_Visualizers.Add(image.DatabaseIndex, visualizer);
            }
            else if ((image.TrackingState == TrackingState.Stopped && visualizer != null) ||(visualizer != null && visualizer.isAugmentedObjectDestroyed()))
            {
                m_Visualizers.Remove(image.DatabaseIndex);
                Destroy(visualizer.transform.parent.gameObject); //destroy visualizer + anchor
            }
        }



        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        //only perform raycast outside of buttons
        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    if (!portalAlreadyCreated)
                    {
                        // Instantiate portal at the hit pose.
                        portal = Instantiate(PortalPrefab, hit.Pose.position, hit.Pose.rotation);
                        portal.transform.Rotate(new Vector3(0, 90, 0));
                        portal.transform.position -= Vector3.up * 1.8f;

                        //set reference of portal for device
                        portal.GetComponentInChildren<PortalController>().device = FirstPersonCamera.transform;


                        // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                        // world evolves.
                        Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                        // Make the portal a child of the anchor.
                        portal.transform.parent = anchor.transform;

                        portalAlreadyCreated = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

    public void SetNewPortal()
    {
        portalAlreadyCreated = false;

        //Destroy old portal with anchor
        Destroy(portal.transform.parent.gameObject);
    }
}
