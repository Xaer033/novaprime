using UnityEngine;

public class CollisionInfo
{
    public bool above;
    public bool below;
    public bool right;
    public bool left;
    public bool belowOld;
    public bool climbingSlope;
    public bool decendingSlope;
    public bool slidingDownMaxSlope;
    public float slopeAngle;
    public Vector2 slopeNormal;
    public float slopeAngleOld;
    public Vector2 oldMoveDelta;
    public bool stepUp;
    public int faceDir;
    public bool crushed;

    public bool fallingThroughPlatform;

    public void Reset(Vector2 oldDelta)
    {
        oldMoveDelta = oldDelta;
        belowOld = below;
        above = below = right = left = false;
        stepUp = false;
        climbingSlope = false;
        decendingSlope = false;
        slidingDownMaxSlope = false;
        crushed = false;
        slopeAngleOld = slopeAngle;
        slopeAngle = 0;
        slopeNormal = Vector3.zero;
    }
}
