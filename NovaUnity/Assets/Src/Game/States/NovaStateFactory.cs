using GhostGen;
using UnityEngine;

public class NovaGameState
{
    public const string NO_STATE                = "none";

    public const string INTRO                   = "intro";
    public const string MAIN_MENU               = "main_menu";
    public const string SINGLEPLAYER_GAMEPLAY   = "singleplayer_gameplay";
    public const string CREDITS                 = "credits";
}


public class NovaStateFactory : IStateFactory
{
    public IGameState CreateState(string stateId)
    {
        switch (stateId)
        {
            case NovaGameState.INTRO:                       return new IntroState();
            case NovaGameState.MAIN_MENU:                   return new MainMenuState();
            case NovaGameState.SINGLEPLAYER_GAMEPLAY:       return new SingleplayerGameplayState();
            case NovaGameState.CREDITS:                     break;
        }

        Debug.LogError("Error: state ID: " + stateId + " does not exist!");
        return null;
    }
}
