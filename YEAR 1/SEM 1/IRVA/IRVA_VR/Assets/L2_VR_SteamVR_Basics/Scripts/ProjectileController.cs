using NaughtyAttributes;
using UnityEngine;

namespace L2_VR_SteamVR_Basics.Scripts
{
    /// <summary>
    /// Controls a projectile's lifetime.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] 
        [BoxGroup("External components")]
        private float destroyTime = 60f;

        private void Awake() => Destroy(gameObject, destroyTime);
    }
}
