using Battleship.Classes;
using Battleship.Enums;
using Battleship.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Page
    {
        private readonly GameController _gameController;
        private List<TextBox> _playerNameFields;
        private int _playersCounter;

        public RegisterView(GameController gameController)
        {
            _gameController = gameController;
            _playerNameFields = [];
            _playersCounter = 0;
            InitializeComponent();
            InitializeRegistrationForm();
        }

        public void InitializeRegistrationForm()
        {
            _playerNameFields = [];
            AddPlayerFormField();
            
            Button addPlayerField_Click = new Button
            {
                Content = "Add Player",
                Width = 100,
                Height = 20,
                Background = Brushes.LightGreen,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            ButtonPanel.Children.Add(addPlayerField_Click);
            addPlayerField_Click.Click += AddPlayers_Click;

            Button registerButton = new Button
            {
                Content = "Start Game!",
                Width = 100,
                Height = 20,
                Margin = new(20),
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            ButtonPanel.Children.Add(registerButton);
            registerButton.Click += RegisterPlayers_Click;
        }

        private void AddPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (_playersCounter >= GameController.MAX_PLAYERS_AMOUNT)
            {
                MessageBox.Show(ErrorMessage.MAX_PLAYERS_AMOUNT_ERROR);
                return;
            }

            AddPlayerFormField();
        }

        private void AddPlayerFormField()
        {
            _playersCounter++;

            StackPanel playerNameStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            TextBlock playerLabel = new TextBlock
            {
                Text = $"Player {_playersCounter}:",
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBox playerNameField = new TextBox
            {
                Name = $"Player{_playersCounter}Textbox",
                Width = 200,
                Height = 30,
                Margin = new(10),
            };

            playerNameStack.Children.Add(playerLabel);
            playerNameStack.Children.Add(playerNameField);
            NameFieldsPanel.Children.Add(playerNameStack);

            _playerNameFields.Add(playerNameField);
        }

        private void RegisterPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (_playerNameFields.Count < 2)
            {
                if (MessageBox.Show(ErrorMessage.CONFIRM_TO_PLAY_WITH_BOT, "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
                _gameController.SetIsPlayingWithBot(true);
            }

            foreach (TextBox playerNameField in _playerNameFields)
            {
                string name = playerNameField.Text.Trim();

                Player player = new Player(name);
                _gameController.AddPlayer(player);
                _gameController.AddPlayerFleet(player, CreateFleet());
                _gameController.AddPlayerBoard(player, CreateBoards());
            }

            if (_gameController.GetIsPlayingWithBot())
            {
                Player botPlayer = new Player("Bot", PlayerType.BOT);
                _gameController.AddPlayer(botPlayer);
                _gameController.AddPlayerFleet(botPlayer, CreateFleet());
                _gameController.AddPlayerBoard(botPlayer, CreateBoards());
                _gameController.SetIsPlayingWithBot(true);
            }

            _gameController.SetGameState(GameStates.PLACING_SHIPS);

            NavigationService?.Navigate(new StrategyView(_gameController));
        }

        private List<IShip> CreateFleet()
        {
            List<IShip> ships = [];
            foreach (ShipType shipType in Enum.GetValues<ShipType>())
            {
                Ship ship = new(shipType);
                ships.Add(ship);
            }

            return ships;
        }

        private List<IBoard> CreateBoards()
        {
            List<IBoard> boards = [];
            foreach (BoardType boardType in Enum.GetValues(typeof(BoardType)))
            {
                Board board = new(GameController.BOARD_WIDTH, GameController.BOARD_HEIGHT, boardType);
                boards.Add(board);
            }

            return boards;
        }
    }
}
