using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private GameObject earth;

    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        if (eventArgs.added.Count > 0)
        {
            UpdateImage(eventArgs.added[0]);
        }
        if (eventArgs.updated.Count > 0)
        {
            UpdateImage(eventArgs.updated[0]);
        }
        if (eventArgs.removed.Count > 0)
        {
            earth.SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage image)
    {
        earth.SetActive(true);

        Vector3 direction = (image.transform.position - Camera.current.transform.position).normalized;
        earth.transform.position = image.transform.position + 6 * direction;
    }
}
