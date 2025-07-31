using System;
using System.Collections.Generic;
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
using Battleship.Classes;
using Battleship.Enums;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Page
    {
        private GameController _gameController;
        private List<TextBox> _playerNameFields;

        public RegisterView()
        {
            InitializeComponent();
            InitializeFields();
            _gameController = GameController.GetInstance();
        }

        public void InitializeFields()
        {
            _playerNameFields = [];

            for (int i = 0; i < GameController.PLAYERS_AMOUNT; i++)
            {
                StackPanel playerName = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                TextBlock playerLabel = new TextBlock
                {
                    Text = $"Player {i + 1}:",
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBox playerNameField = new TextBox
                {
                    Name = $"Player{i}Textbox",
                    Width = 200,
                    Height = 30,
                    Margin = new(10),
                };

                playerName.Children.Add(playerLabel);
                playerName.Children.Add(playerNameField);
                NameFieldsPanel.Children.Add(playerName);
                _playerNameFields.Add(playerNameField);
            }

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
            registerButton.Click += RegisterPlayers_Click;

            ButtonPanel.Children.Add(registerButton);
        }

        private void AddPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (GameController.PLAYERS_AMOUNT >= GameController.MAX_PLAYERS_AMOUNT)
            {
                MessageBox.Show($"Maximum players amount is {GameController.MAX_PLAYERS_AMOUNT}!");
                return;
            }

            GameController.PLAYERS_AMOUNT++;
            StackPanel playerName = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            TextBlock playerLabel = new TextBlock
            {
                Text = $"Player {GameController.PLAYERS_AMOUNT}:",
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBox playerNameField = new TextBox
            {
                Name = $"Player{GameController.PLAYERS_AMOUNT}Textbox",
                Width = 200,
                Height = 30,
                Margin = new(10),
            };

            playerName.Children.Add(playerLabel);
            playerName.Children.Add(playerNameField);
            NameFieldsPanel.Children.Add(playerName);
            _playerNameFields.Add(playerNameField);
        }

        private void RegisterPlayers_Click(object sender, RoutedEventArgs e)
        {
            if (_playerNameFields.Count < 2)
            {
                MessageBox.Show("Not enough players!");
                return;
            }

            List<string> _playerNames = [];
            foreach (TextBox playerNameField in _playerNameFields)
            {
                string name = playerNameField.Text.Trim();
                _playerNames.Add(name);
            }

            _gameController.SetPlayerNames(_playerNames);
            NavigationService?.Navigate(new Uri("/Views/StrategyView.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
