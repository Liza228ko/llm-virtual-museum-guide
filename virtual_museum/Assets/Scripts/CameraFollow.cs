using UnityEngine;

[DisallowMultipleComponent]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;           // avatar root or a Head anchor
    public Vector3 offset = new Vector3(0f, 2.0f, -4.5f);

    [Header("Smoothing")]
    [Range(0f, 1f)] public float posSmooth = 0.15f;
    [Range(0f, 1f)] public float rotSmooth = 0.15f;

    [Header("Look")]
    public bool lookAtTarget = true;
    public Vector3 lookOffset = new Vector3(0f, 1.5f, 0f); // aim a bit above hips

    [Header("Collision (prevents clipping through walls)")]
    public LayerMask collisionMask = ~0;
    public float sphereRadius = 0.2f;
    public float minDistance = 1.0f;

    Vector3 _vel;

    void LateUpdate()
    {
        if (!target) return;

        // desired camera position before collision handling
        Vector3 desiredWorldPos = target.position + target.TransformVector(offset);

        // collision: push camera closer if something blocks between target and desired
        Vector3 focusPoint = target.position + lookOffset;
        Vector3 toCam = desiredWorldPos - focusPoint;
        float dist = toCam.magnitude;
        if (Physics.SphereCast(focusPoint, sphereRadius, toCam.normalized, out var hit, dist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float safeDist = Mathf.Max(minDistance, hit.distance - 0.05f);
            desiredWorldPos = focusPoint + toCam.normalized * safeDist;
        }

        // smooth position
        var nextPos = Vector3.SmoothDamp(transform.position, desiredWorldPos, ref _vel, Mathf.Max(0.0001f, posSmooth));
        transform.position = nextPos;

        // smooth rotation (optional)
        if (lookAtTarget)
        {
            Vector3 aim = focusPoint - transform.position;
            if (aim.sqrMagnitude > 0.0001f)
            {
                Quaternion want = Quaternion.LookRotation(aim.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, want, 1f - Mathf.Pow(1f - rotSmooth, Time.deltaTime * 60f));
            }
        }
    }

    // handy binder if you spawn/replace avatars at runtime
    public void BindTarget(Transform newTarget) { target = newTarget; }
}
