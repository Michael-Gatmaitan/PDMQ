using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageDetectedScript : MonoBehaviour
{
    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        Debug.Log("On hnaged runned");
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            Debug.Log("New image Life cycle");
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            Debug.Log("Updated image Life cycle");
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
            Debug.Log("Removed image Life cycle");
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ListAllImages();
        }
    }

    void ListAllImages()
    {
        Debug.Log(
            $"There are {m_TrackedImageManager.trackables.count} images being tracked.");

        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {
            Debug.Log($"Image: {trackedImage.referenceImage.name} is at " +
                      $"{trackedImage.transform.position}");
        }
    }
}
