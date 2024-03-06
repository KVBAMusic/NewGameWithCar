public struct GameParams
{
    public uint numberOfPlayers;
    public bool spawnBots;
    public bool useItems;
    public PlayerSortingMethod sortingMethod;

    public GameParams(uint numberOfPlayers, bool spawnBots, bool useItems, PlayerSortingMethod sortingMethod)
    {
        this.numberOfPlayers = numberOfPlayers;
        this.sortingMethod = sortingMethod;
        this.spawnBots = spawnBots;
        this.useItems = useItems;
    }

    public static GameParams SinglePlayer_SingleRace() => new GameParams(Constants.maxPlayers, true, true, PlayerSortingMethod.BotsFirst);
    public static GameParams SinglePlayer_TimeTrial() => new GameParams(1, false, false, PlayerSortingMethod.PlayersFirst);
    public static GameParams SinglePlayer_Itemless() => new GameParams(Constants.maxPlayers, true, false, PlayerSortingMethod.BotsFirst);
}