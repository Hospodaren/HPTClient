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

namespace GuaranteeReductionClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var singleRowCollection = new GuaranteeReductionCalculator.SingleRowCollection(3, 6);
            singleRowCollection.CalculateBestSetOfRows(72, 2, 1);

            //var singleRowCollection = new GuaranteeReductionCalculator.SingleRowCollection(3, 7);
            //singleRowCollection.CalculateBestSetOfRows(27, 4, 2);

            //singleRowCollection.CalculateBestSetOfRows(33, 3, 2);
            //var singleRow = new GuaranteeReductionCalculator.SingleRow(5, 3, 4);
            //int result = singleRow.RowDifference(21);

        }
    }
}
