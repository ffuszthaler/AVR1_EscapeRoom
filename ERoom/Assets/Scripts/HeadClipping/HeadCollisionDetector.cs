using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollisionDetector : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)]
    private float detectionDelay = 0.05f;
    [SerializeField]
    private float detectionDistances = 0.2f;
    [SerializeField]
    private LayerMask detectionLayers;
    public List<RaycastHit> DetectedColliderHits { get; private set; }

    private float currentTime = 0;

    [field: SerializeField]
    public bool InsideCollider { get; private set; }


    private List<RaycastHit> PreformDetection
    (Vector3 position, float distance, LayerMask mask)
    {
        List<RaycastHit> detectedHits = new();

        List<Vector3> directions
            = new() { transform.forward, transform.right, -transform.right };

        RaycastHit hit;
        foreach (var dir in directions)
        {
            if (Physics.Raycast(position, dir, out hit, distance, mask))
            {
                detectedHits.Add(hit);
            }
        }
        return detectedHits;
    }

    private void Start()
    {
        DetectedColliderHits = PreformDetection(transform.position,
           detectionDistances, detectionLayers);
    }
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > detectionDelay)
        {
            currentTime = 0;
            InsideCollider = false;
            DetectedColliderHits = PreformDetection(transform.position,
                detectionDistances, detectionLayers);
            if (DetectedColliderHits.Count <= 0)
                InsideCollider 
                    = CheckIfInsideCollider(transform.position, 
                    detectionDistances, detectionLayers);
        }
    }
    public bool CheckIfInsideCollider(Vector3 position, float distance, LayerMask mask)
    {
        return Physics.CheckSphere(position, distance, mask, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            return;
        Color c = Color.green;
        c.a = 0.5f;
        if (DetectedColliderHits.Count > 0)
        {
            c = Color.red;
            c.a = 0.5f;
        }

        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, detectionDistances);

        List<Vector3> directions = new() { transform.forward, transform.right, -transform.right };
        Gizmos.color = Color.magenta;
        foreach (var dir in directions)
        {
            Gizmos.DrawRay(transform.position, dir);
        }
    }
}
