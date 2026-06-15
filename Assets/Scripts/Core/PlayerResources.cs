public class PlayerResources
{
    public int CurrentEnergy { get; private set; }
    public int MaxEnergy { get; private set; }

    public int CardDraw { get; private set; }
    public PlayerResources(int maxEnergy, int cardDraw)
    {
        MaxEnergy = maxEnergy;
        CurrentEnergy = maxEnergy;
        CardDraw = cardDraw;
        
    }

}