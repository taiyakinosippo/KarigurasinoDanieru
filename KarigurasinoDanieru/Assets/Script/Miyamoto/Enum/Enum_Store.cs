using UnityEngine;

// UIのアクションを定義する列挙型
public enum UIAction
{
    Show,
    Close
}

// ゲームモードを定義する列挙型
public enum GameMode
{
    Solo = 0,
    Multi = 1
}
// ゲームレベルを定義する列挙型
public enum GameLevel
{
    Normal = 0,
    Hard = 1,
}
// SEの種類を定義する列挙型
public enum SEType
{
    indicatorSE,
    SelectbuttonSE,
    clickSE,
    missSE,
    goodSE,
    greatSE,
    perfectSE
}
//背景のステージを定義する列挙型
public enum StageGroup
{
    Ground,
    Sky,
    UpperSky,
    Atmosphere,
    AtmoToSpace,
    Space
}

public enum StageType
{
    Ground_A,  
    Sky_A,
    Sky_B,
    Sky_C,
    UpperSky_A, 
    Atmosphere_A, 
    Atmosphere_B, 
    Atmosphere_C, 
    AtmoToSpace_A, 
    Space_A, 
    Space_B, 
    Space_C
}