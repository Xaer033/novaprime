using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;


[CreateAssetMenu(menuName = "Nova/Game Config")]
public class GameConfig : ScriptableObject, IPostInit
{
    public string initalState;

    public GuiManager guiManager;
//    public CardResourceBank cardResourceBank;
    public GameplayResources gameplayResources;

    public void PostInit()
    {
        
    }
}
