using UnityEngine;

namespace FunnyMusic
{
    public class UITrackObject : MonoBehaviour
    {
        [Tooltip("The 3D touch collider.")]
        [SerializeField] public Collider TouchCollider3D;
        [Tooltip("The 2D touch collider.")]
        [SerializeField] public Collider2D TouchCollider2D;
    }
}