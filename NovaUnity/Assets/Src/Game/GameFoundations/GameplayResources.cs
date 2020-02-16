using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Gameplay Resources")]
public class GameplayResources : ScriptableObject
{
    public GameplayCamera gameplayCamera;
    public BulletView bulletView;
    public TextAsset inputList;

    public HealthUIView healthUIPrefab;
    public UnitMap unitMap;
}
