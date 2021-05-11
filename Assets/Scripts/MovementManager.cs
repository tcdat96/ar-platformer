using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    /// <summary>
    /// References the object to place.
    /// </summary>
    public GameObject Character;

    public GameObject DustParticles;

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
                Character.GetComponent<CharacterController>().Destination = hit.point;         
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Slam();
        }
    }

    void Slam()
    {
        Character.GetComponent<CharacterController>().Slam();
        StartCoroutine(ShowDustParticles());
    }

    IEnumerator ShowDustParticles()
    {
        yield return new WaitForSeconds(2f);
        DustParticles.transform.position = Character.transform.position;
        DustParticles.SetActive(true);
        DustParticles.GetComponent<ParticleSystem>().Play();
    }
}
    