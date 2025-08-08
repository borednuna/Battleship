namespace Battleship.UnitTests.GameController;

using NUnit.Framework;
using Battleship.Classes;
using Battleship.Enums;
using Battleship.Interfaces;

[TestFixture]
public class GameController_IsGameControllerShould
{
    private GameController _gameController;

    [SetUp]
    public void Setup()
    {
        _gameController = new();
    }

    [Test]
    public void GetRandomInstance_ShouldReturnRandom()
    {
        Random random = _gameController.GetRandomInstance();

        Assert.That(random, Is.InstanceOf<Random>());
    }

    [Test]
    public void GetCurrentGameState_ShouldReturnInitialize()
    {
        _gameController.Reset();
        GameStates state = _gameController.GetCurrentGameState();

        Assert.That(state, Is.EqualTo(GameStates.INITIALIZING));
    }

    [Test]
    public void IsPlayerFleetPlaced_InputIsPlayerHasUnplacedShips_ShouldReturnFalse()
    {
        Player testPlayer = new("TestPlayer");

        List<IShip> ships = [];
        foreach (ShipType shipType in Enum.GetValues<ShipType>())
        {
            ships.Add(new Ship(shipType));
        }

        _gameController.AddPlayer(testPlayer);
        _gameController.AddPlayerFleet(testPlayer, ships);

        bool isFleetPlaced = _gameController.IsPlayerFleetPlaced(testPlayer);

        Assert.That(isFleetPlaced, Is.EqualTo(false));
    }

    [Test]
    public void IsPlayerFleetPlaced_InputIsPlayerHasPartiallyPlacedShips_ShouldReturnFalse()
    {
        Player testPlayer = new("TestPlayer");

        List<IShip> ships = [];
        foreach (ShipType shipType in Enum.GetValues<ShipType>())
        {
            ships.Add(new Ship(shipType));
        }

        _gameController.AddPlayer(testPlayer);
        _gameController.AddPlayerFleet(testPlayer, ships);

        for (int i = 0; i < (int)Math.Floor((double)(ships.Count / 2)); i++)
        {
            ships[i].SetIsPlaced(true);
        }

        bool isFleetPlaced = _gameController.IsPlayerFleetPlaced(testPlayer);
        Assert.That(isFleetPlaced, Is.EqualTo(false));
    }

    [Test]
    public void IsPlayerFleetPlaced_InputIsPlayerHasAllShipsPlaced_ShouldReturnTrue()
    {
        Player testPlayer = new("TestPlayer");

        List<IShip> ships = [];
        foreach (ShipType shipType in Enum.GetValues<ShipType>())
        {
            ships.Add(new Ship(shipType));
        }

        _gameController.AddPlayer(testPlayer);
        _gameController.AddPlayerFleet(testPlayer, ships);

        foreach (IShip ship in ships)
        {
            ship.SetIsPlaced(true);
        }

        bool isFleetPlaced = _gameController.IsPlayerFleetPlaced(testPlayer);
        Assert.That(isFleetPlaced, Is.EqualTo(true));
    }

    [TestCase(ShipType.BATTLESHIP)]
    [TestCase(ShipType.CARRIER)]
    [TestCase(ShipType.CRUISER)]
    [TestCase(ShipType.DESTROYER)]
    [TestCase(ShipType.SUBMARINE)]
    public void GetShipByType_InputIsShipTypePlayer_ShouldReturnShipByType(ShipType type)
    {
        Player testPlayer = new("TestPlayer");

        List<IShip> ships = [];
        foreach (ShipType shipType in Enum.GetValues<ShipType>())
        {
            ships.Add(new Ship(shipType));
        }

        _gameController.AddPlayer(testPlayer);
        _gameController.AddPlayerFleet(testPlayer, ships);

        IShip? shipByType = _gameController.GetShipByType(type, testPlayer);
        Assert.That(shipByType?.GetShipType(), Is.EqualTo(type));
    }

    [Test]
    public void GetIsPlayingWithBot_ShouldReturnFalse()
    {
        _gameController.SetIsPlayingWithBot(false);

        bool isPlayingWithBot = _gameController.GetIsPlayingWithBot();
        Assert.That(isPlayingWithBot, Is.EqualTo(false));
    }

    [Test]
    public void GetIsPlayingWithBot_ShouldReturnTrue()
    {
        _gameController.SetIsPlayingWithBot(true);

        bool isPlayingWithBot = _gameController.GetIsPlayingWithBot();
        Assert.That(isPlayingWithBot, Is.EqualTo(true));
    }

    [Test]
    public void GetBotPlayer_ShouldReturnBotPlayer()
    {

        Player humanPlayer = new("humanPlayer");
        Player botPlayer = new("botPlayer", PlayerType.BOT);

        _gameController.AddPlayer(humanPlayer);
        _gameController.AddPlayer(botPlayer);

        IPlayer? getBotPlayer = _gameController.GetBotPlayer();
        Assert.That(getBotPlayer?.GetPlayerType(), Is.EqualTo(PlayerType.BOT));
    }
}
