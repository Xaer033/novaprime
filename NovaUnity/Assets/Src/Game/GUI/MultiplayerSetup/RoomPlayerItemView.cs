﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using TMPro;

public class RoomPlayerItemView : UIView 
{
    public int playerSlot { get; set; }

    public Image checkmark;
    public TextMeshProUGUI playerName;
    public Image profileIcon;
}
