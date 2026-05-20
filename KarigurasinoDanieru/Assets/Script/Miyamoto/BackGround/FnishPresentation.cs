using UnityEngine;

///=========================================
/// プレイヤーの状態に応じた挙動を定義するためのインターフェース
///=========================================


/// =========================
/// プレイヤーの状態がミスだった場合の挙動
/// =========================
public class MissBehavior : FlightBehavior
{
    public void Execute(Rocket_Mover rocket)
    {
        rocket.MissRocketMove();
    }
}

/// =========================
/// プレイヤーの状態が空中だった場合の挙動
/// =========================

public class SkyBehavior : FlightBehavior
{
    public void Execute(Rocket_Mover rocket)
    {
        rocket.SkyRocketMove();
    }
}

/// =========================
/// プレイヤーの状態が大気圏だった場合の挙動
/// =========================

public class AtmosphereBehavior : FlightBehavior
{
    public void Execute(Rocket_Mover rocket)
    {
        rocket.SkyRocketMove();
    }
}

/// =========================
/// プレイヤーの状態が宇宙だった場合の挙動
/// =========================

public class SpaceBehavior : FlightBehavior
{
    public void Execute(Rocket_Mover rocket)
    {
        rocket.AtmosphereRocketMove();
    }
}

/// =========================
/// プレイヤーの状態が銀河だった場合の挙動
/// =========================

public class GalaxyBehavior : FlightBehavior
{
    public void Execute(Rocket_Mover rocket)
    {
        rocket.GalaxyRocketMove();
    }
}