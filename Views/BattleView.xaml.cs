using Battleship.Classes;
using Battleship.Enums;
using Battleship.Interfaces;
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

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for BattleView.xaml
    /// </summary>
    public partial class BattleView : Page
    {
        private GameController _gameController;
        private IBoard _ownBoard;

        public BattleView()
        {
            InitializeComponent();
            InitializePlayerData();
            InitializeBattleGrid();
        }

        private void InitializePlayerData()
        {
            _gameController = GameController.GetInstance();
            _ownBoard = _gameController.GetCurrentPlayerBoard()[GameController.OWN_BOARD_INDEX];

            BattlePanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Turn";
        }

        private void InitializeBattleGrid()
        {
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;
            TrackingGrid.Rows = GameController.BOARD_HEIGHT;
            TrackingGrid.Columns = GameController.BOARD_WIDTH;

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    var trackingButton = new Button
                    {
                        Margin = new Thickness(1),
                        Background = Brushes.LightYellow,
                    };

                    Grid.SetRow(trackingButton, row);
                    Grid.SetColumn(trackingButton, col);
                    TrackingGrid.Children.Add(trackingButton);

                    var ownButton = new Button
                    {
                        Margin = new Thickness(1),
                        IsEnabled = false,
                    };

                    Grid.SetRow(ownButton, row);
                    Grid.SetColumn(ownButton, col);
                    OwnGrid.Children.Add(ownButton);
                }
            }
        }
    }
}
