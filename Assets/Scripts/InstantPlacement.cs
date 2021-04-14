using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantPlacement : MonoBehaviour
{
    /// <summary>
    /// References the 3D cursor.
    /// </summary>
    public GameObject DepthCursor;

    /// <summary>
    /// References the object to place.
    /// </summary>
    public GameObject character;

    private const float k_AvatarOffsetMeters = 0.015f;

    void Start()
    {
    }

    void Update()
    {
        Touch touch;
        if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            Vector3 toCamera = Camera.main.transform.position - DepthCursor.transform.position;
            toCamera.Normalize();

            Vector3 destination = DepthCursor.transform.position + (toCamera * k_AvatarOffsetMeters);
            if (!character.activeSelf) {
                character.transform.SetPositionAndRotation(destination, DepthCursor.transform.rotation);
                character.SetActive(true);
            }
            character.GetComponent<CharacterController>().Destination = destination;
        }
    }
}
