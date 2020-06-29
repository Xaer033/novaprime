using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using GhostGen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public class BruiserView : AvatarView
{

    [BoxGroup("Limb Hooks")]
    public Transform _l_backLeg;
    [BoxGroup("Limb Hooks")]
    public Transform _l_frontLeg;
    [BoxGroup("Limb Hooks")]
    public Transform _r_backLeg;
    [BoxGroup("Limb Hooks")]
    public Transform _r_frontLeg;
    
}
