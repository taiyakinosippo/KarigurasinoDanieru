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
//
