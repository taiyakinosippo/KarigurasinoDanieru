using UnityEngine;

public class MissBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
       Debug.Log("さぼったな");
    }
}

public class SkyBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        Debug.Log("まだまだやな");
    }
}

public class AtmosphereBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        Debug.Log("大気圏突破！");
    }
}

public class SpaceBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        Debug.Log("宇宙到達！");
    }
}