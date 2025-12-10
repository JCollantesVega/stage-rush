using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;

    [Header("Settings")]
    public string targetTag = "Prop";
    public float sphereRadius = 3f;
    public float maxDistance = 600f;

    private CullingGroup cullingGroup;
    private List<GameObject> props = new List<GameObject>();
    private BoundingSphere[] spheres;

    void Start()
    {
        InitializeCullingGroup();
        UpdateAllVisibility();
    }

    void InitializeCullingGroup()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        props.Clear();
        props.AddRange(taggedObjects);

        spheres = new BoundingSphere[props.Count];


        for(int i = 0; i < props.Count; i++)
        {
            Vector3 pos = props[i].transform.position; 
            spheres[i] = new BoundingSphere(pos, sphereRadius);
        }


        cullingGroup = new CullingGroup();
        cullingGroup.targetCamera = targetCamera;
        cullingGroup.SetBoundingSpheres(spheres);
        cullingGroup.SetBoundingSphereCount(props.Count);

        cullingGroup.SetDistanceReferencePoint(targetCamera.transform);
        cullingGroup.SetBoundingDistances(new float[]{maxDistance});

        cullingGroup.onStateChanged += OnStateChanged;

        Debug.Log($"CullingManager: Initialized with {props.Count} objects", this);
    }

    private void OnStateChanged(CullingGroupEvent sphere)
    {
        if (sphere.index < 0 || sphere.index >= props.Count) return;

        GameObject obj = props[sphere.index];

        if (obj == null) return;

        bool visible = sphere.isVisible && sphere.currentDistance <= 0;

        if(obj.activeSelf != visible)
            obj.SetActive(visible);
    }

    void UpdateAllVisibility()
    {
        for(int i = 0; i < props.Count; i++)
        {
            if(props[i] == null) continue;

            int sphereIndex = i;
            bool isVisible = cullingGroup.IsVisible(sphereIndex);
            float distance = cullingGroup.GetDistance(sphereIndex);

            bool visible = isVisible && distance <= 0;
            props[i].SetActive(visible);
        }
    }

    void OnDestroy()
    {
        cullingGroup?.Dispose();
    }

    void OnDrawGizmosSelected()
    {
        if (spheres == null || !Application.isPlaying) return;
        
        Gizmos.color = Color.cyan;
        foreach (var sphere in spheres)
        {
            Gizmos.DrawWireSphere(sphere.position, sphere.radius);
        }
    }
}
