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

namespace Battleship
{
    /// <summary>
    /// Interaction logic for BattlePage.xaml
    /// </summary>
    public partial class BattlePage : Page
    {
        private const int GridSize = 14;
        public BattlePage()
        {
            InitializeComponent();
            InitializeBattleGrid();
        }

        private void InitializeBattleGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                TrackingGrid.ColumnDefinitions.Add(new ColumnDefinition());
                OwnGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < GridSize; i++)
            {
                TrackingGrid.RowDefinitions.Add(new RowDefinition());
                OwnGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    // For Tracking Grid
                    var button = new Button
                    {
                        Content = $"{(char)('A' + row)}{col + 1}",
                        Margin = new Thickness(1)
                    };

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    TrackingGrid.Children.Add(button);
                }
            }
        }
    }
}
