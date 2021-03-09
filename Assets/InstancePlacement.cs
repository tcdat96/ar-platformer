using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InstancePlacement : MonoBehaviour
{
    private ARRaycastManager raycastManager;

    private Pose placementPose;
    private bool isPlacementPoseValid = false;

    public GameObject placementIndicator;
    public GameObject character;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        PlaceCharacter();
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(.5f, .5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlacementPoseValid = hits.Count > 0;

        if (isPlacementPoseValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null)
        {
            return;
        }

        if (isPlacementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceCharacter()
    {
        Touch touch;
        if (!isPlacementPoseValid || Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        //if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        //{
        //    return;
        //}

        Instantiate(character, placementPose.position, placementPose.rotation);
        Instantiate(placementIndicator, placementPose.position, placementPose.rotation);
    }
}
