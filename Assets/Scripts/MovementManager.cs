using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementManager : MonoBehaviour
{
    /// <summary>
    /// References the object to place.
    /// </summary>
    public GameObject Character;

    public GameObject DustParticles;
    public GameObject Dungeon;


    public Image Image;

    public GameObject Hole;
    private bool isTurfMoving = false;

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
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            CutHole();
        }
    }

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return frameEnd;

        int width = 300;
        int height = 300;

        int srcX = 300;
        int srcY = 200;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true);
        Rect rect = new Rect(srcX, srcY, srcX + width, srcY + height);
        texture.ReadPixels(rect, 0, 0);
        texture.LoadRawTextureData(texture.GetRawTextureData());
        texture.Apply();

        Image.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2());
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

    private Vector3 GetCursorPosition()
    {
        return new Vector3(0, 0, 0);
        // return new Vector3(0, 5, 0);
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

        Hole.transform.position = position;

        Transform turf = Hole.transform.GetChild(0);
        if (Hole.activeSelf)
        {
            // put the turf down
            Vector3 dest = turf.position - new Vector3(0, turfHeight * 2, 0);
            StartCoroutine(_MoveTurf(turf, turf.position, dest, 10));
        }
        else
        {
            Hole.SetActive(true);
            // pull the turf up
            Vector3 dest = position + new Vector3(0, turfHeight * 2, 0);
            StartCoroutine(_MoveTurf(turf, position, dest, 10));
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
        if (b.y < a.y)
        {
            turf.parent.gameObject.SetActive(false);
        }
    }
}
