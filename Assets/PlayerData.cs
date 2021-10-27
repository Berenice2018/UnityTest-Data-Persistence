using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public List<PlayerItem> allPlayers = new List<PlayerItem>();
}

[Serializable]
public class PlayerItem
{
    public string userName;
    public int score;
}
