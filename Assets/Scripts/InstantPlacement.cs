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


    public GameObject Hole;
    private bool isTurfMoving = false;


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

    public void CutHole()
    {
        if (isTurfMoving)
        {
            return;
        }
        isTurfMoving = true;

        Vector3 position = GetCursorPosition();
        float turfHeight = Hole.transform.GetChild(0).lossyScale.y;
        position.y -= turfHeight / 2;

        Transform turf = Hole.transform.GetChild(0);
        if (Hole.activeSelf)
        {
            // put the turf down
            Vector3 dest = turf.position - new Vector3(0, turfHeight * 2, 0);
            StartCoroutine(_MoveTurf(turf, turf.position, dest, .2f));
        }
        else
        {
            // show the turf
            Hole.transform.position = position;
            Hole.SetActive(true);
            // pull it up
            Vector3 dest = position + new Vector3(0, turfHeight * 2, 0);
            StartCoroutine(_MoveTurf(turf, position, dest, .2f));
        }
    }

    private IEnumerator _MoveTurf(Transform turf, Vector3 a, Vector3 b, float speed)
    {
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            turf.position = Vector3.Lerp(a, b, t); // Move turf closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        
        turf.position = b;
        isTurfMoving = false;

        // if putting turf down
        if (b.y < a.y) {
            turf.parent.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        Character.SetActive(false);
        Dungeon.SetActive(false);
        DustParticles.SetActive(false);
        Portal.SetActive(false);
        Hole.SetActive(false);
    }
}
