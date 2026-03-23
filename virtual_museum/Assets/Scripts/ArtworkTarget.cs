using UnityEngine;

[DisallowMultipleComponent]
public class ArtworkTarget : MonoBehaviour
{
    [Header("Search")]
    [Tooltip("Display/Product name used for text search (unique if possible)")]
    public string productName;

    [Tooltip("Optional alternate names / aliases")]
    public string[] aliases;

    [Header("Optional precise stand point")]
    public Transform anchor;

    [Header("Optional look-at pivot (e.g., art center)")]
    public Transform lookAt;

    [Min(0.1f)]
    public float standOff = 1.0f;
}
