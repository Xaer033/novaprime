using UnityEngine;

public class SpawnPointView : MonoBehaviour
{
    public SpawnMode spawnMode = SpawnMode.ONCE;
    public float spawnInterval;
    public int maxSpawnCount;
    public Collider triggerCollider;
    
    public SpawnPointData spawnPointData;
    
    private void OnDrawGizmos()
    {
        
        Gizmos.color = new Color(0.8f, 0.2f, 0.3f, 0.8f);
        Vector3 platformSize = triggerCollider != null ?  triggerCollider.bounds.size : Vector3.one;
        Gizmos.DrawCube(transform.position, platformSize);
    }
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
