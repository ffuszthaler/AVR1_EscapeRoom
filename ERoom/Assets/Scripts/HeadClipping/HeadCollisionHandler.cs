using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private HeadCollisionDetector detector;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    public float pushBackStrength = 1.0f;
    [SerializeField]
    private FadeEffect blackScreenFade;

    public bool IsClimbing
    {
        get;
        set;
    }

    private Vector3 CalculatePushBackDirection(List<RaycastHit> colliderHits)
    {
        Vector3 combinedNormal = Vector3.zero;
        foreach (RaycastHit hitPoint in colliderHits)
        {
            combinedNormal +=
                new Vector3(hitPoint.normal.x, 0, hitPoint.normal.z); ;
        }
        return combinedNormal;
    }

    private void Update()
    {
        if (detector.InsideCollider)
        {
            blackScreenFade.Fade(true);
            return;
        }
        if (detector.DetectedColliderHits.Count <= 0)
        {
            blackScreenFade.Fade(false);
            return;
        }
        Vector3 pushBackDirection
            = CalculatePushBackDirection(detector.DetectedColliderHits);

        Debug.DrawRay(transform.position, pushBackDirection.normalized, Color.magenta);

        characterController
            .Move(pushBackDirection.normalized * pushBackStrength * Time.deltaTime);
    }
}
