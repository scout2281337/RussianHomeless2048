using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class RaycastInfo : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The character's collision skin width")]
    [SerializeField] private float _skinWidth = 0.015f;
    [Tooltip("Specifies the length of the raycasts used for collision detection")]
    [SerializeField] private float _rayLength = 0.05f;
    [Tooltip("Sets the number of raycasts to be cast for vertical collision detection")]
    [SerializeField] private int _verticalRayCount = 4;
    [Tooltip("Sets the number of raycasts to be cast for horizontal collision detection")]
    [SerializeField] private int _horizontalRayCount = 4;
    [Tooltip("Specifies the layers for collision detection")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("Debug")]
    [SerializeField] private bool _showDebugRays = true;

    [SerializeField] private RaycastHitInfo _hitGroundInfo;

    private CapsuleCollider _collider;

    private float _verticalRaySpacing;
    private float _horizontalRaySpacing;

    private float _cornersRaySpacing;

    public RaycastHitInfo HitGroundInfo => _hitGroundInfo;

    [System.Serializable]
    public struct RaycastHitInfo
    {
        [ReadOnly] public bool Forward, Down;

        public void Reset()
        {
            Forward = false;
            Down = false;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();

        // calculate the space between each raycast
        SetVerticalRaySpacing();
        SetHorizontalRaySpacing();
    }

    private void Update()
    {
        // check for collisions
        CheckGround();
        CheckForward();
    }

    #region Collisions
    enum CollisionType
    {
        LowerVertical, UpperForward
    }

    private void CheckForCollisions(CollisionType type)
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        switch (type)
        {
            case CollisionType.LowerVertical:
                _hitGroundInfo.Down = CheckForCollisions(_verticalRayCount, _verticalRaySpacing,
                    new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), Vector3.right + Vector3.forward, Vector3.down, _groundLayers);
                break;
            case CollisionType.UpperForward:
                _hitGroundInfo.Forward = CheckForCollisions(_verticalRayCount, _horizontalRaySpacing,
                    transform.position + transform.forward * _collider.bounds.extents.z,
                    transform.up, transform.forward, _groundLayers);
                break;
        }
    }

    /// <summary>
    /// Check for raycast collisions
    /// </summary>
    /// <param name="rayCount">number of raycasts to be cast</param>
    /// <param name="raySpacing">space between each raycast</param>
    /// <param name="startRayOrigin">starting position of the first raycast on that side</param>
    /// <param name="raycastShiftDirection">direction in which the position of the raycasts will be shifted</param>
    /// <param name="raycastDirection">raycasts direction</param>
    /// <param name="layer">check layer</param>
    /// <returns>whether or not there has been a collision</returns>
    private bool CheckForCollisions(int rayCount, float raySpacing, Vector3 startRayOrigin,
        Vector3 raycastShiftDirection, Vector3 raycastDirection, LayerMask layer)
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);
        bool hasHit = false;

        for (int i = 0; i < rayCount; i++)
        {
            for (int j = 0; j < rayCount; j++)
            {
                Vector3 rayOrigin = startRayOrigin;
                rayOrigin += new Vector3(raycastShiftDirection.x * (raySpacing * i), raycastShiftDirection.y * (raySpacing * j), raycastShiftDirection.z * (raySpacing * j));
                Color raycastColor = Color.red;
                if (Physics.Raycast(rayOrigin, raycastDirection, _rayLength, layer))
                {
                    hasHit = true;
                    raycastColor = Color.green;
                }
                if (_showDebugRays && layer == _groundLayers)
                    Debug.DrawRay(rayOrigin, raycastDirection * _rayLength, raycastColor);
            }
        }

        return hasHit;
    }
    #endregion

    #region Vertical Raycasts
    private void SetVerticalRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        _verticalRayCount = Mathf.Clamp(_verticalRayCount, 2, int.MaxValue);
        _verticalRaySpacing = bounds.size.x / (_verticalRayCount - 1);
    }

    private void CheckGround()
    {
        CheckForCollisions(CollisionType.LowerVertical);
    }
    #endregion

    #region Horizontal Raycasts
    private void SetHorizontalRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(_skinWidth * -2);

        _horizontalRayCount = Mathf.Clamp(_horizontalRayCount, 2, int.MaxValue);
        _horizontalRaySpacing = bounds.size.y / 2 / (_horizontalRayCount - 1);
    }

    private void CheckForward()
    {
        CheckForCollisions(CollisionType.UpperForward);
    }
    #endregion
}
