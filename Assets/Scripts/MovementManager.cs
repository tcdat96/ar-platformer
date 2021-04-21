using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if(hit.transform != null)
            {
                character.GetComponent<CharacterController>().Destination = hit.point;         
            }
        }
    }
}
