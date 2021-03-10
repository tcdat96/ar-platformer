﻿using System;
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

    private Pose destinationPose;
    private bool isMoving = false;
    public float speed = 10;
    public GameObject character;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        if (!character.activeInHierarchy)
        {
            GetPlacementPose();
            UpdatePlacementIndicator();
        }

        Touch touch;
        if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            // if we haven't placed the character
            if (!character.activeInHierarchy)
            {
                // check if it can be placed
                if (isPlacementPoseValid)
                {
                    showCharacter();
                }
            }
            // otherwise, we need to prepare to move it
            else
            {
                GetDestinationPose(touch.position);
            }
        }

        if (isMoving)
        {
            MoveCharacter(destinationPose.position);
        }
    }

    private List<ARRaycastHit> RayCast(Vector2 screenPoint)
    {
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPoint, hits, TrackableType.Planes);
        return hits;
    }

    private void GetPlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(.5f, .5f));
        var hits = RayCast(screenCenter);

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

    private void showCharacter()
    {
        if (character == null || !isPlacementPoseValid)
        {
            return;
        }

        placementIndicator.SetActive(false);
        
        character.SetActive(true);
        character.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        character.transform.Rotate(0, 180, 0);
    }

    private void GetDestinationPose(Vector3 touchPosition)
    {
        var hits = RayCast(touchPosition);
        if (hits.Count > 0)
        {
            destinationPose = hits[0].pose;
            isMoving = true;
        }
    }

    private void MoveCharacter(Vector3 target)
    {
        // move it to desired position
        Transform transform = character.transform;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        // check if we've reached the destionation
        if (transform.position == target)
        {
            isMoving = false;
        }
    }
}
