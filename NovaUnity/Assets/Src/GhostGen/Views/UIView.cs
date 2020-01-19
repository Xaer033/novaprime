using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace GhostGen
{
    public class UIView : EventDispatcherBehavior
    {
        protected event Action<BaseEventData> _onTriggered;
        private InvalidationFlag _invalidateFlag = InvalidationFlag.ALL; // Default to invalidating everything

        public event Action<BaseEventData> onTriggered
        {
            add { _onTriggered += value; }
            remove { _onTriggered -= value; }
        }

        public InvalidationFlag invalidateFlag
        {
            get { return _invalidateFlag; }
            set { _invalidateFlag |= value; }
        }
        
        public void Validate(InvalidationFlag flag = InvalidationFlag.ALL)
        {
            invalidateFlag = flag;
            OnViewUpdate();
        }

        public void OnTriggered(BaseEventData eventData)
        {
            if(_onTriggered != null)
            {
                _onTriggered(eventData);
            }
        }

        protected virtual void OnViewUpdate()
        {
        }

        public virtual void OnViewOutro(Action finishedCallback)
        {
            if(finishedCallback != null)
            {
                finishedCallback();
            }
        }

        public virtual void OnViewDispose()
        {

        }

        protected bool IsInvalid(InvalidationFlag flag)
        {
            if(flag == InvalidationFlag.ALL) { return true; }
            if(_invalidateFlag == InvalidationFlag.ALL) { return true; }

            return _invalidateFlag.IsFlagSet(flag);         
        }
        
        public virtual void Update()
        {
            if(invalidateFlag != InvalidationFlag.NONE)
            {
                OnViewUpdate();
            }

            _invalidateFlag = InvalidationFlag.NONE;
        }
    }
}
