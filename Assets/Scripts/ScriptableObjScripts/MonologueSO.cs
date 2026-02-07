using UnityEngine;
using System;
using System.Collections.Generic;

public enum MonologueType
{
    CLUE,
    AFTERDIED
}

[Serializable]
public class MonologueTextData
{
    public MonologueType type;
    public Sprite icon;
    [TextArea(3, 10)]
    public string text;
}

[CreateAssetMenu(fileName = "NewMonologue", menuName = "Dialogue/MonologueSO")]
public class MonologueSO : ScriptableObject
{
    public List<MonologueTextData> monologueDataList;
}