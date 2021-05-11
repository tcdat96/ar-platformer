using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GoogleARCore;

public class InstantPlacement : MonoBehaviour
{
    /// <summary>
    /// References the 3D cursor.
    /// </summary>
    public GameObject DepthCursor;

    /// <summary>
    /// References the object to place.
    /// </summary>
    public GameObject Character;

    public GameObject DustParticles;
    public GameObject Dungeon;

    public GameObject Portal;

    public Text CurrentPositionText;
    public Text ReticlePositionText;
    public Text RayHitDistanceText;

    private const float k_AvatarOffsetMeters = 0.015f;

    void Start()
    {

    }

    void Update()
    {
        // Touch touch;
        // if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        // {
        //     MoveCharacter();
        // }

        CurrentPositionText.text = Character.transform.position.ToString();
        ReticlePositionText.text = DepthCursor.transform.position.ToString();
        RayHitDistanceText.text = Character.GetComponent<CharacterController>().RayHitDistance.ToString();
    }

    private Vector3 GetCursorPosition()
    {
        Vector3 toCamera = Camera.main.transform.position - DepthCursor.transform.position;
        toCamera.Normalize();
        return DepthCursor.transform.position + (toCamera * k_AvatarOffsetMeters);
    }

    public void MoveCharacter()
    {
        Vector3 destination = GetCursorPosition();
        if (!Character.activeSelf)
        {
            Character.transform.SetPositionAndRotation(destination, DepthCursor.transform.rotation);
            Character.SetActive(true);
        }
        Character.GetComponent<CharacterController>().Destination = destination;
    }

    public void SlamCharacter()
    {
        Character.GetComponent<CharacterController>().Slam();
        StartCoroutine(ShowDustParticles());
        StartCoroutine(ShowDungeon());
    }

    IEnumerator ShowDustParticles()
    {
        yield return new WaitForSeconds(2f);

        Vector3 pos = Character.transform.position;
        //TODO: okay, hardcoding is actually pretty bad
        DustParticles.transform.position = new Vector3(pos.x - 0.06f, pos.y, pos.z - 0.06f);

        DustParticles.SetActive(true);
        DustParticles.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator ShowDungeon()
    {
        Dungeon.SetActive(false);

        yield return new WaitForSeconds(2.5f);

        Dungeon.SetActive(true);

        Vector3 pos = Character.transform.position;
        Dungeon.transform.position = new Vector3(pos.x + 0.18f, pos.y - 0.3f, pos.z + 0.14f);

        Character.transform.Translate(new Vector3(0, -0.25f, 0));
    }

    public void ShowPortal()
    {
        Vector3 cursorPos = GetCursorPosition();
        Portal.transform.position = cursorPos;
    }
}
