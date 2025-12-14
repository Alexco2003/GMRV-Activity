using NaughtyAttributes;
using UnityEngine;

namespace L3_VR_SteamVR_Advanced.Scripts.GravityGun
{
    public class GravityGunObject : MonoBehaviour
    {
        [field: SerializeField] 
        [field: BoxGroup("Game object components")]
        [field: Required]
        public Transform Root { get; private set; }
        
        [field: SerializeField] 
        [field: BoxGroup("Game object components")]
        [field: Required]
        public Rigidbody Rigidbody { get; private set; }
        
        [SerializeField] 
        [field: BoxGroup("Game object components")]
        [field: Required]
        private MeshRenderer meshRenderer;
        
        [SerializeField] 
        [field: BoxGroup("External components")]
        [field: Required]
        private Material selectedMaterial;
        
        public bool IsSnapped { get; set; }
        public Vector3 KinematicVelocity { get; set; }
        
        private Material _defaultMaterial;
        private Vector3 _previousPosition;

        private void Awake()
        {
            if(meshRenderer == null)
            {
                meshRenderer = GetComponentInChildren<MeshRenderer>();
            }
            _defaultMaterial = meshRenderer.material;
        }

        /// <summary>
        /// The Rigidbody is kinematic whilst it's snapped.
        /// A kinematic Rigidbody's velocity is irrelevant, hence we need to compute it manually.
        /// </summary>
        private void FixedUpdate()
        {
            if (IsSnapped)
            {
                KinematicVelocity = (transform.position - _previousPosition) / Time.fixedDeltaTime;
                _previousPosition = transform.position;
            }
        }

        public void HighlightMesh() => meshRenderer.material = selectedMaterial;

        public void RemoveMeshHighlight() => meshRenderer.material = _defaultMaterial;
        
        public void SetParent(Transform t) => Root.SetParent(t);
        
        public void ResetParent() => Root.SetParent(null);
    }
}
