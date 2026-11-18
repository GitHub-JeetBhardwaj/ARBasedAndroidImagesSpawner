using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageObjectTracker : MonoBehaviour
{
    [Serializable]
    public class TrackedImagePrefabConfig
    {
        [Tooltip("Exact name of the image in your XR Reference Image Library")]
        public string referenceImageName;

        [Tooltip("3D Prefab to spawn when this image is detected")]
        public GameObject arObjectPrefab;

        [Header("Placement Offsets")]
        public Vector3 localPosition = new Vector3(0f, 0f, 0.01f);
        public Vector3 localRotation = Vector3.zero;

        [Header("Animation Settings")]
        [Tooltip("Leave blank to play default state, or enter exact Animator state name")]
        public string animationStateName = "";
        public bool playAnimationOnTracking = true;
    }

    // Internal class to track the runtime state of each individual spawned object
    private class SpawnedObjectState
    {
        public GameObject instance;
        public Animator animator;
        public Animation legacyAnimation;
        public bool wasTracking;
        public TrackedImagePrefabConfig config;
    }

    [Header("AR Configuration")]
    [SerializeField]
    private ARTrackedImageManager imageManager;

    [Header("Image to Prefab Mappings")]
    [SerializeField]
    private List<TrackedImagePrefabConfig> imageMappings = new List<TrackedImagePrefabConfig>();

    // Fast lookup dictionaries
    private readonly Dictionary<string, TrackedImagePrefabConfig> configLookup = 
        new Dictionary<string, TrackedImagePrefabConfig>();
    
    private readonly Dictionary<string, SpawnedObjectState> activeObjects = 
        new Dictionary<string, SpawnedObjectState>();

    private void Awake()
    {
        // Populate the lookup dictionary from the Inspector list
        configLookup.Clear();
        foreach (var mapping in imageMappings)
        {
            if (!string.IsNullOrWhiteSpace(mapping.referenceImageName) && mapping.arObjectPrefab != null)
            {
                if (!configLookup.ContainsKey(mapping.referenceImageName))
                {
                    configLookup.Add(mapping.referenceImageName, mapping);
                }
                else
                {
                    Debug.LogWarning($"Duplicate referenceImageName found in mappings: {mapping.referenceImageName}");
                }
            }
        }
    }

    private void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    private void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
        {
            UpdateObject(image);
        }

        foreach (var image in args.updated)
        {
            UpdateObject(image);
        }

        foreach (var image in args.removed)
        {
            DestroyObject(image.referenceImage.name);
        }
    }

    private void UpdateObject(ARTrackedImage image)
    {
        string imageName = image.referenceImage.name;

        // Ignore reference images that aren't configured in our list
        if (!configLookup.TryGetValue(imageName, out TrackedImagePrefabConfig config))
            return;

        bool isTracking = image.trackingState == TrackingState.Tracking;

        // Spawn the prefab if it hasn't been created yet and is actively tracking
        if (!activeObjects.TryGetValue(imageName, out SpawnedObjectState state) && isTracking)
        {
            state = CreateObject(image, config);
            if (state != null)
            {
                activeObjects[imageName] = state;
            }
        }

        if (state != null && state.instance != null)
        {
            // Keep the model visible during temporary tracking loss
            state.instance.SetActive(true);

            // Trigger animation when tracking starts or resumes
            if (isTracking && !state.wasTracking && config.playAnimationOnTracking)
            {
                PlayObjectAnimation(state);
            }

            state.wasTracking = isTracking;
        }
    }

    private SpawnedObjectState CreateObject(ARTrackedImage image, TrackedImagePrefabConfig config)
    {
        if (config.arObjectPrefab == null)
        {
            Debug.LogError($"AR Object Prefab is not assigned for {config.referenceImageName}!");
            return null;
        }

        GameObject spawned = Instantiate(config.arObjectPrefab, image.transform);

        // Set AR placement without overwriting prefab scale
        spawned.transform.localPosition = config.localPosition;
        spawned.transform.localRotation = Quaternion.Euler(config.localRotation);

        Animator anim = spawned.GetComponentInChildren<Animator>(true);
        Animation legacyAnim = spawned.GetComponentInChildren<Animation>(true);

        if (anim != null)
        {
            anim.applyRootMotion = false;
            anim.enabled = true;
            anim.speed = 1f;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        if (anim == null && legacyAnim == null)
        {
            Debug.LogWarning($"No animation component found on prefab for {config.referenceImageName}!");
        }

        return new SpawnedObjectState
        {
            instance = spawned,
            animator = anim,
            legacyAnimation = legacyAnim,
            wasTracking = false,
            config = config
        };
    }

    private void PlayObjectAnimation(SpawnedObjectState state)
    {
        if (state.animator != null)
        {
            state.animator.enabled = true;
            state.animator.speed = 1f;

            if (!string.IsNullOrWhiteSpace(state.config.animationStateName))
            {
                state.animator.Play(state.config.animationStateName, 0, 0f);
                state.animator.Update(0f);
            }
            else
            {
                // Replay default state from frame 0 if no specific state name is given
                state.animator.Play(0, 0, 0f);
                state.animator.Update(0f);
            }
        }
        else if (state.legacyAnimation != null)
        {
            state.legacyAnimation.Stop();
            state.legacyAnimation.Play();
        }
    }

    private void DestroyObject(string imageName)
    {
        if (activeObjects.TryGetValue(imageName, out SpawnedObjectState state))
        {
            if (state.instance != null)
            {
                Destroy(state.instance);
            }
            activeObjects.Remove(imageName);
        }
    }
}