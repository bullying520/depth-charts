using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;
using Unity.Injection;

namespace DepthChartsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ISport sport;
        private IDepthCharts<PositionRankList> depthCharts;
        private IPositionRankListFactory<PositionRankList> factory;
        private IPlayer currentSelectedPlayer = null;
        private string currentSelectedPosition = "";
        private string currentSport = STR_NFL;
        private UnityContainer container;
        private const string STR_NFL = "NFL";
        private const string STR_SOCCER = "Soccer";

        public OutputViewModel OutputModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            btnRemove.IsEnabled = false;
            btnPrintBackups.IsEnabled = false;
            OutputModel = new OutputViewModel(outputBox);

            container = new UnityContainer();
            container.RegisterType<IPositionRankListFactory<PositionRankList>, PositionRankListFactory<PositionRankList>>();
            container.RegisterType<ISport, NFL>(STR_NFL);
            container.RegisterType<ISport, Soccer>(STR_SOCCER);

            container.RegisterType<ITeam, TampaBayBuccaneers>(STR_NFL);
            container.RegisterType<ITeam, RealMadrid>(STR_SOCCER);

            sportsComboBox.ItemsSource = new object []{ STR_NFL, STR_SOCCER };
            sportsComboBox.SelectedValue = currentSport;

            Reset();
        }

        private void Reset()
        {
            var team = container.Resolve<ITeam>(currentSport);
            playersList.ItemsSource = team.GetPlayers().OrderBy(p => p.Number);

            sport = container.Resolve<ISport>(currentSport);
            factory = container.Resolve<IPositionRankListFactory<PositionRankList>>();
            var title = currentSport == STR_NFL ? "Offense" : "Squad";

            container.RegisterType<IDepthCharts<PositionRankList>, DepthCharts<PositionRankList>>(new InjectionConstructor(new object[] { sport, title, factory }));

            depthCharts = container.Resolve<IDepthCharts<PositionRankList>>();
            ChartTitle.DataContext = depthCharts;

            OutputModel.Clear();

            RefreshGrid();
        }

        private class DataGridRowViewModel
        {
            public string Position { get; set; }
            public IPlayer Player1 { get; set; }
            public IPlayer Player2 { get; set; }
            public IPlayer Player3 { get; set; }
            public IPlayer Player4 { get; set; }
            public IPlayer Player5 { get; set; }
        }

        public class OutputViewModel
        {
            private StringBuilder sb = new StringBuilder();
            private TextBox _outputBox;

            public OutputViewModel(TextBox outputBox)
            {
                _outputBox = outputBox;
            }

            public string Output
            {
                get { return sb.ToString(); }
                set { }
            }

            public void Append(string message)
            {
                sb.AppendLine(message);
                _outputBox.Text = Output;
            }

            public void Clear()
            {
                sb.Clear();
                _outputBox.Text = "";
            }
        }

        private void RefreshGrid()
        {
            depthChartsGrid.ItemsSource = null;

            var viewModelList = new List<DataGridRowViewModel>();
            foreach (var position in sport.Positions)
            {
                var playersInPosition = depthCharts.GetFullPositionChart(position).ToList();

                var viewModel = new DataGridRowViewModel
                {
                    Position = position,
                    Player1 = playersInPosition.FirstOrDefault(),
                    Player2 = playersInPosition.Count() >= 2 ? playersInPosition[1] : null,
                    Player3 = playersInPosition.Count() >= 3 ? playersInPosition[2] : null,
                    Player4 = playersInPosition.Count() >= 4 ? playersInPosition[3] : null,
                    Player5 = playersInPosition.Count() >= 5 ? playersInPosition[4] : null,
                };

                viewModelList.Add(viewModel);
            }

            depthChartsGrid.ItemsSource = viewModelList;
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox parent = (ListBox)sender;
            playersList = parent;
            object data = GetDataFromListBox(playersList, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Copy);
                //isDraging = true;
            }
        }

        private static object GetDataFromListBox(ListBox source, Point point)
        {
            if (source.InputHitTest(point) is UIElement element)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }

        private Tuple<int, int> GetDataGridRowIndexColumnIndex(DependencyObject dep)
        {
            //DependencyObject dep = (DependencyObject)e.OriginalSource;

            //Stepping through the visual tree
            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            //Is the dep a cell or outside the bounds of Window1?
            if (dep == null | !(dep is DataGridCell))
            {
                return null;
            }
            else
            {
                DataGridCell cell = new DataGridCell();
                cell = (DataGridCell)dep;
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                {
                    return null;
                }
                int colIndex = cell.Column.DisplayIndex;

                DataGridRow row = dep as DataGridRow;
                int rowIndex = depthChartsGrid.ItemContainerGenerator.IndexFromContainer(row);

                Trace.WriteLine($"{rowIndex}, {colIndex}");
                return Tuple.Create(rowIndex, colIndex);
            }
        }

        private void DepthChartsGrid_Drop(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetData(typeof(Player)) is IPlayer player))
                return;

            Trace.WriteLine(player);

            var rowData = (e.OriginalSource as TextBlock).DataContext;
            if (rowData == null) return;

            var position = rowData.GetType().GetProperty("Position")?.GetValue(rowData, null).ToString();

            Trace.WriteLine(position);

            var indexTuple = GetDataGridRowIndexColumnIndex((DependencyObject)e.OriginalSource);
            if (indexTuple == null) return;

            var positionDepth = indexTuple.Item2 - 1; // need to deduct the first position string column
            if (positionDepth < 0) return;

            try
            {                
                depthCharts.AddPlayerToDepthChart(position, player, positionDepth);
                RefreshGrid();

                var msg = $"{position} - Inserted {{{player}}} at index: {positionDepth}";
                OutputModel.Append(msg);
            }
            catch (Exception ex)
            {                
                Trace.WriteLine(ex.Message);

                MessageBox.Show(ex.Message);
            }
        }

        // somehow the datagrid.SelectedItemChanged event didn't popularte the new selected item, has to use the mouse up event
        private void DepthChartsGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rowData = (e.OriginalSource as TextBlock)?.DataContext;
            if (rowData == null) return;

            currentSelectedPosition = rowData.GetType().GetProperty("Position")?.GetValue(rowData, null).ToString();

            Trace.WriteLine(currentSelectedPosition);

            var indexTuple = GetDataGridRowIndexColumnIndex((DependencyObject)e.OriginalSource);
            if (indexTuple == null) return;

            var player = rowData.GetType().GetProperty($"Player{indexTuple.Item2}")?.GetValue(rowData, null) as IPlayer;
            Trace.WriteLine(player);

            currentSelectedPlayer = player;

            if (currentSelectedPlayer != null)
            {
                btnPrintBackups.IsEnabled = btnRemove.IsEnabled = true;
                btnRemove.Content = $"Remove {player}";
            }
            else
            {
                btnPrintBackups.IsEnabled = btnRemove.IsEnabled = false;
                btnRemove.Content = "Remove Player";
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelectedPlayer == null) { return; }

            depthCharts.RemovePlayerFromDepthChart(currentSelectedPosition, currentSelectedPlayer);

            var msg = $"{currentSelectedPosition} - Removed {{{currentSelectedPlayer}}}";
            OutputModel.Append(msg);

            currentSelectedPosition = "";
            currentSelectedPlayer = null;

            btnPrintBackups.IsEnabled = btnRemove.IsEnabled = false;
            btnRemove.Content = "Remove Player";

            RefreshGrid();
        }

        private void BtnPrintBackups_Click(object sender, RoutedEventArgs e)
        {
            if (currentSelectedPlayer == null) { return; }

            var sb = new StringBuilder();
            Func<IEnumerable<IPlayer>, string> func = (list) =>
            {
                if (list.Count() == 0)
                {
                    sb.AppendLine(DepthCharts<PositionRankList>.STR_NO_LIST);
                }
                else
                {
                    var players = string.Join("\r\n", list.Select(player => $"{player}"));
                    sb.AppendLine(players);
                }

                return sb.ToString();
            };

            var output = depthCharts.PrintBackups(func, currentSelectedPosition, currentSelectedPlayer);
            OutputModel.Append(output);
        }

        private void OutputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            outputBox.ScrollToEnd();
        }

        private void BtnPrintFullCharts_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            Func<IDictionary<string, IEnumerable<IPlayer>>, string> func = (dict) =>
            {
                foreach (var position in dict.Keys)
                {
                    var list = dict[position];
                    if (list.Count() == 0) { continue; }

                    var players = string.Join(", ", list.Select(player => $"({player})"));
                    sb.AppendLine($"{position} - {players}");
                }

                if (sb.Length == 0)
                {
                    return DepthCharts<PositionRankList>.STR_NO_LIST;
                }

                return sb.ToString();
            };

            var output = depthCharts.PrintFullDepthCharts(func);
            OutputModel.Append(output);
        }

        private void SportsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Trace.WriteLine(sportsComboBox.SelectedItem);

            currentSport = sportsComboBox.SelectedItem.ToString();
            Reset();
        }

        private void DepthChartsGrid_DragOver(object sender, DragEventArgs e)
        {
            Trace.WriteLine("over");
        }
    }
}
