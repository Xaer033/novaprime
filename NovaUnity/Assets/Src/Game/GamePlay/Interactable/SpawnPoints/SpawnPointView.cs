using UnityEditor;
using UnityEngine;

public class SpawnPointView : MonoBehaviour
{
    public SpawnMode spawnMode = SpawnMode.ONCE;
    public float spawnInterval;
    public int maxSpawnCount;
    public Collider triggerCollider;
    
    public SpawnPointData spawnPointData;
    
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        const float kCharTextWidth = 0.08f;

        if(triggerCollider == null || transform == null)
        {
            return;
        }
        
        Gizmos.color = new Color(0.8f, 0.2f, 0.3f, 0.8f);
        Bounds triggerBounds = triggerCollider.bounds;

        Vector3 platformSize = triggerBounds.size;
        Gizmos.DrawCube(transform.position, platformSize);

        Vector3 textPosition = transform.position + (Vector3.up * (triggerBounds.size.y + 0.1f));
        float size = HandleUtility.GetHandleSize(textPosition);
        
        textPosition.x -= (gameObject.name.Length * kCharTextWidth * size) / 2.0f;
        Handles.Label(textPosition, gameObject.name);
    }
#endif
    
}

[System.Serializable]
public struct SpawnPointData
{
    public SpawnType spawnType;
    public string subtypeId;
    
    [HideInInspector]
    public Vector3 position;
    
    public string customString;
    public int customInt;
    public float customFloat;
    public Vector3 customVector3;
}


[System.Serializable]
public enum SpawnType
{
    NONE = 0,
    AVATAR,
    ITEM,
    HAZARD
}

public enum SpawnMode
{
    ONCE,
    REPEATING, 
    VOLUME_TRIGGER
}
