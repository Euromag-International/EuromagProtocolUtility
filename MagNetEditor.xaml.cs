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
using System.Windows.Shapes;
using UniversalConnect.Models;
using UniversalConnect.StdCommands;

namespace EuromagProtocolUtility
{
    /// <summary>
    /// Logica di interazione per MagNetEditor.xaml
    /// </summary>
    public partial class MagNetEditor : Window
    {
        public MagNetEditor()
        {
            InitializeComponent();

            this.DataContext = new MagNetEditorViewModel();
        }

        private void parametersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (parametersGrid.CurrentItem != null)
                CommResources.FillMagNetParameterInfo(parametersGrid.CurrentItem as MemoryModelMagNet.VariableModelMagNet);

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
