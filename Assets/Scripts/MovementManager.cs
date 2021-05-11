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
    public GameObject Dungeon;

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
            if (hit.transform != null)
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
        StartCoroutine(ShowDungeon());
    }

    IEnumerator ShowDustParticles()
    {
        yield return new WaitForSeconds(2f);

        Vector3 pos = Character.transform.position;
        DustParticles.transform.position = new Vector3(pos.x - 3, pos.y, pos.z - 3);

        DustParticles.SetActive(true);
        DustParticles.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator ShowDungeon()
    {
        Dungeon.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        Dungeon.SetActive(true);

        Vector3 pos = Character.transform.position;
        Dungeon.transform.position = new Vector3(pos.x + 9, pos.y - 15, pos.z + 7);

        Character.transform.Translate(new Vector3(0, -15, 0));
    }
}
