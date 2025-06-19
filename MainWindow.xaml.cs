using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalConnect;
using static EuromagProtocolUtility.CommonResources;
using UniversalConnect.Models;
using UniversalConnect.StdCommands;

namespace EuromagProtocolUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel();
        }

        private void parametersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (parametersGrid.CurrentItem != null)
                CommResources.FillParameterInfo(parametersGrid.CurrentItem as ParameterView);
        }

        private void ParFilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CommonResources.Instance.DeviceParametersFilter = ParFilterBox.Text;
            CommonResources.Instance.DeviceParametersFilterExec();
        }

        private void VariableListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VariableListGrid.CurrentItem != null)
                CommResources.FillVariableInfo(VariableListGrid.CurrentItem as ITargetVariable);
        }

        private void BundleListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BundleListGrid.CurrentItem != null)
                CommResources.FillBundleInfo(BundleListGrid.CurrentItem as PageModel);

        }

        private void ParamsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CommResources.ClearInfo();
        }

        private void BundleRamListGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BundleRamListGrid.CurrentItem != null)
                CommResources.FillBundleRamInfo(BundleRamListGrid.CurrentItem as RAMPageModel);

        }

        private void GSMParametersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GSMParametersGrid.CurrentItem != null)
                CommResources.FillGSMParameterInfo(GSMParametersGrid.CurrentItem as IGSMvariable);
        }

        private void GSMVariablesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GSMVariablesGrid.CurrentItem != null)
                CommResources.FillGSMParameterInfo(GSMVariablesGrid.CurrentItem as IGSMvariable);

        }

        public CommonResources CommResources
        {
            get
            {
                return CommonResources.Instance;
            }
        }
    }
}