using UnityEngine;
using System.Collections;


namespace GhostGen
{
	public interface IStateFactory 
	{
		IGameState CreateState( string stateId );
	}
}