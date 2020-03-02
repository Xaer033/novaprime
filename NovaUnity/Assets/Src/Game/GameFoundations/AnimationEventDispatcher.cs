using GhostGen;
using UnityEngine;

public class AnimationEventDispatcher : StateMachineBehaviour
{
    public bool bubbling = false;
    
    public string onStateEnterEventName;
    public string onStateExitEventName;
    public string onStateUpdateEventName;
    public string onStateMoveEventName;

    
    private IEventDispatcher _dispatcher;
    
    public struct AnimationEventData
    {
        public AnimationEventData(Animator p_animator, AnimatorStateInfo p_stateInfo, int p_layerIndex)
        {
            animator = p_animator;
            stateInfo = p_stateInfo;
            layerIndex = p_layerIndex;
        }
        
        public readonly Animator animator;
        public readonly AnimatorStateInfo stateInfo;
        public readonly int layerIndex;
    }
    
    private void SetDispatcher(Animator animator)
    {
        if (_dispatcher == null)
        {
            _dispatcher = animator.GetComponent<IEventDispatcher>();
        }
    }
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetDispatcher(animator);
        if (!string.IsNullOrEmpty(onStateEnterEventName) && _dispatcher != null)
        {
            AnimationEventData data = new AnimationEventData(animator, stateInfo, layerIndex);
            _dispatcher.DispatchEvent(onStateEnterEventName, bubbling, data);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetDispatcher(animator);
        if (!string.IsNullOrEmpty(onStateExitEventName) && _dispatcher != null)
        {
            AnimationEventData data = new AnimationEventData(animator, stateInfo, layerIndex);
            _dispatcher.DispatchEvent(onStateExitEventName, bubbling, data);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetDispatcher(animator);
        if (!string.IsNullOrEmpty(onStateUpdateEventName) && _dispatcher != null)
        {
            AnimationEventData data = new AnimationEventData(animator, stateInfo, layerIndex);
            _dispatcher.DispatchEvent(onStateUpdateEventName, bubbling, data);
        }
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetDispatcher(animator);
        if (!string.IsNullOrEmpty(onStateMoveEventName) && _dispatcher != null)
        {
            AnimationEventData data = new AnimationEventData(animator, stateInfo, layerIndex);
            _dispatcher.DispatchEvent(onStateMoveEventName, bubbling, data);
        }
    }
}
