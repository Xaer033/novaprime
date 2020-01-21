using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController
{
    public float skinWidth { get; set; }
    
    public LayerMask collisionMask { get; set; }
    public int horizontalRayCount { get; set; }
    public int verticalRayCount  { get; set; }

    public Collider collider { get; set; }
   public float horizontalRaySpacing { get; private set;}
   public float verticalRaySpacing { get; private set;}
    
    public  RaycastOrigins origins
    {
        get { return _origins; }
    }
    
   
    private RaycastOrigins _origins;
    
    public struct RaycastOrigins
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }
    
    public RaycastController(int hRayCount, int vRayCount, Collider boundsCollider, LayerMask collisionLayerMask, float collisionSkinWidth = 0.015f)
    {
        horizontalRaySpacing = 0;
        verticalRaySpacing = 0;
        skinWidth = collisionSkinWidth;
        horizontalRayCount = hRayCount;
        verticalRayCount = vRayCount;

        collider = boundsCollider;
        collisionMask = collisionLayerMask;

        if (collider)
        {
            _calculateRaySpacing();
            _updateRaycastOrigins();
        }
        else
        {
            Debug.LogError("Collider not set for Raycast Controller!");
        }
    }

    public void UpdateRaycastOrigins()
    {
        _calculateRaySpacing(); // *TEMP*
        _updateRaycastOrigins();
    }
    
    private void _updateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        
       _origins.bottomLeft = new Vector3(bounds.min.x, bounds.min.y);
       _origins.bottomRight = new Vector3(bounds.max.x, bounds.min.y);
       _origins.topLeft = new Vector3(bounds.min.x, bounds.max.y);
       _origins.topRight = new Vector3(bounds.max.x, bounds.max.y);
    }
    
    private void _calculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        int clampedHRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        int clampedVRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        
        horizontalRaySpacing = bounds.size.y / (clampedHRayCount - 1);
        verticalRaySpacing = bounds.size.x / (clampedVRayCount - 1);
    }
}
