using UnityEngine;

namespace GhostGen
{
    public class GameStateMachine
	{
        private IStateFactory _stateFactory;
        private IGameState _currentState;

        private string _currentId;

        public GameStateMachine( IStateFactory p_stateFactory )
		{
			_currentState 	= null;
			_currentId 		= "-666";
			_stateFactory 	= p_stateFactory;
		}

		public void Step( float p_deltaTime )
		{
			if( _currentState != null )
				_currentState.Step( p_deltaTime );
		}

		public void FixedStep(float p_fixedDeltaTime)
		{
			if (_currentState != null)
			{
				_currentState.FixedStep(p_fixedDeltaTime);
			}
		}

		public void LateStep(float p_deltaTime)
		{
			if (_currentState != null)
			{
				_currentState.LateStep(p_deltaTime);
			}
		}
		
		public void ChangeState( string stateId, object changeStateInfo = null )
		{
			if (_currentId == stateId)
				return;

			if (_currentState != null)
			{
				Debug.Log ("Leaving State: " + _currentId);
				_currentState.Exit( );
			}

			
			Debug.Log ("Entering State: " + stateId);
			_currentState = _stateFactory.CreateState( stateId );
			_currentState.Init(this, changeStateInfo);
		}
	}
}