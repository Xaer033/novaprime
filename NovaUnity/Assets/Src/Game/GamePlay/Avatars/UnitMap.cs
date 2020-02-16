using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Nova/Unit Map")]
public class UnitMap : ScriptableObject
{
    private Dictionary<string, Unit> _unitMap = new Dictionary<string, Unit>();

    [SerializeField]
    private List<Unit> _unitList;

    public void OnEnable()
    {
        for (int i = 0; i < _unitList.Count; ++i)
        {
            Unit unit = _unitList[i];
            _unitMap.Add(unit.id, unit);
        }
    }

    public Unit GetUnit(string id)
    {
        return _unitMap[id];
    }
    
    [Serializable]
    public class Unit
    {
        public string id;
        public UnitType type;
        public AvatarView view;
        public UnitStats stats;
    }
}
