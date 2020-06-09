using UnityEngine;

public class FootIK : MonoBehaviour
{
    public LayerMask collisionLayer;
    
    [Range(0, 1.0f)]
    public float distanceToGround;

    public float footOffset = 1;
    
    [SerializeField]
    public Animator _animator;

    private RaycastHit2D[] _rayList;
    // Start is called before the first frame update
    void Awake()
    {
        _rayList = new RaycastHit2D[1];
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(!_animator)
        {
            Debug.LogError("No Animator set for foot IK!");
            return;
        }
        
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

        Vector3 leftOrigin = _animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up * footOffset;
        Vector3 rightOrigin = _animator.GetIKPosition(AvatarIKGoal.RightFoot ) + Vector3.up * footOffset;

        RaycastHit2D hit;
        int hitCount = 0;
        
        hitCount = Physics2D.RaycastNonAlloc(leftOrigin, Vector2.down, _rayList, distanceToGround, collisionLayer);
        if(hitCount > 0)
        {
            hit = _rayList[0];
            Vector2 correctedPosition = hit.point + Vector2.up * distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, correctedPosition);
        }
        
        hitCount = Physics2D.RaycastNonAlloc(rightOrigin, Vector2.down, _rayList, distanceToGround, collisionLayer);
        if(hitCount > 0)
        {
            hit = _rayList[0];
            Vector2 correctedPosition = hit.point + Vector2.up * distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, correctedPosition);
        }
    }
}
