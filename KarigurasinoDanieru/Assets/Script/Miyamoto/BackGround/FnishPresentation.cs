using UnityEngine;

public class MissBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        rocket.GetComponent<Rocket_Mover>().MissRocketMove();
    }
}

public class SkyBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        rocket.GetComponent<Rocket_Mover>().SkyRocketMove();
    }
}

public class AtmosphereBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        rocket.GetComponent<Rocket_Mover>().AtmosphereRocketMove();
    }
}

public class SpaceBehavior : FlightBehavior
{
    public void Execute(GameObject rocket)
    {
        rocket.GetComponent<Rocket_Mover>().SpaceRocketMove();
    }
}