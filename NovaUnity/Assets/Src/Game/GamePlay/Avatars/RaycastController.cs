using UnityEngine;

public class RaycastController
{
    public float skinWidth { get; set; }
    public float skinHeight { get; set; }
    
    public LayerMask collisionMask { get; set; }
    public int horizontalRayCount { get; set; }
    public int verticalRayCount  { get; set; }

    public float distanceBetweenRays { get; set; }
    

    public Collider2D collider { get; set; }
   public float horizontalRaySpacing { get; private set;}
   public float verticalRaySpacing { get; private set;}
    
    public  RaycastOrigins origins
    {
        get { return _origins; }
    }
    
   
    private RaycastOrigins _origins;
    
    public struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }
    
    public RaycastController(float distBetweenRays, Collider2D boundsCollider, LayerMask collisionLayerMask, float collisionSkinWidth = 0.015f, float collisionSkinHeight = 0.015f)
    {
        distanceBetweenRays = distBetweenRays;
        horizontalRaySpacing = 0;
        verticalRaySpacing = 0;
        skinWidth = collisionSkinWidth;
        skinHeight = collisionSkinHeight;
        horizontalRayCount = 1;
        verticalRayCount = 1;

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
        bounds.Expand(new Vector2(skinWidth * -2, skinHeight * -2));
        
       _origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
       _origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
       _origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
       _origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    
    private void _calculateRaySpacing()
    {
        distanceBetweenRays = Mathf.Clamp(distanceBetweenRays, 0.0001f, 20);
        
        Bounds bounds = collider.bounds;
        bounds.Expand(new Vector3(skinWidth * -2, skinHeight * -2, 0));

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;
        
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        int clampedHRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        int clampedVRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        
        horizontalRaySpacing = bounds.size.y / (clampedHRayCount - 1);
        verticalRaySpacing = bounds.size.x / (clampedVRayCount - 1);
    }
}
