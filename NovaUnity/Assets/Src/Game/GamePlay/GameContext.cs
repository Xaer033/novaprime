﻿using System.Collections.Generic;

public enum GameMode
{
    SINGLE_PLAYER_CAMPAIGN,
    MULTI_PLAYER_VS,
    MULTI_PLAYER_COOP
}

public class GameContext
{
    public GameMode         gameMode        { get; private set; }
    //public PlayerState[]    playerList      { get; private set; }

    public bool             isMasterClient  { get; set; }

    public static GameContext Create(GameMode type)//, PlayerState[] playerList)
    {
        GameContext gc = new GameContext();
        gc.gameMode = type;
        //gc.playerList = playerList;
        return gc;
    }

    private GameContext() { }
}
