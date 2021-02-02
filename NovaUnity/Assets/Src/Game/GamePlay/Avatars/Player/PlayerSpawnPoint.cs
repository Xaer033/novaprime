using UnityEditor;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Collider triggerCollider;
    
    public PlayerSlot slot;
    
    
    
     
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        const float kCharTextWidth = 0.08f;

        if(triggerCollider == null || transform == null)
        {
            return;
        }
        
        Gizmos.color = new Color(.5f, 0.3f, 0.35f, 0.75f);
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
