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
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyView : Page
    {
        private const int GridSize = 14;
        public StrategyView()
        {
            InitializeComponent();
            InitializeBattleGrid();
        }
        private void InitializeBattleGrid()
        {
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
                    OwnGrid.Children.Add(button);
                }
            }
        }

        private void ContinueToGame_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the game page
            NavigationService?.Navigate(new Uri("Views/BattleView.xaml", UriKind.Relative));
        }
    }
}