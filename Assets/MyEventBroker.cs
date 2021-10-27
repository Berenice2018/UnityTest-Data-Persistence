using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEventBroker 
{
    public static event Action<string> NameChanged;
    public static void CallNameChanged(string name)
    {
        NameChanged?.Invoke(name);
    }

    public static event Action<int> BestScoreChanged;
    public static void CallScoreChanged(int score)
    {
        BestScoreChanged?.Invoke(score);
    }
}
