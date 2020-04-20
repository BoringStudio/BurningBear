public class BuildingController : Singleton<BuildingController>
{
    public Spawnable currentBuilding { get; private set; }

    public void SetUnit(Spawnable unit)
    {
        currentBuilding = unit;

        if (unit)
        {
            Player.Instance.state = Player.State.Build;
        }
    }
}
