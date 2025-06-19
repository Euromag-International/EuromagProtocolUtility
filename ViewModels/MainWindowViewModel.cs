using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EuromagProtocolUtility.CommonResources;
using System.Windows.Data;
using UniversalConnect.Memory;
using UniversalConnect.StdCommands;
using UniversalConnect;

namespace EuromagProtocolUtility
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            CommResources.DeviceParametersView = new ObservableCollection<ParameterView>();
            CommResources.DeviceParametersFilterExec();

            CommResources.DeviceVariables = AllRAMVarsLister.getList().ToList();
            CommResources.DeviceVariablesView.AddRange(CommResources.DeviceVariables);

            CommResources.GsmParameters = AllGSMVarsLister.getList().ToList();
            foreach (IGSMvariable par in CommResources.GsmParameters)
            {
                IGSMvariable gSMvariable = par as IGSMvariable;
                CommResources.GsmParametersView.Add(gSMvariable);
            }

            CommResources.GsmVariables = AllGSMTestVarsLister.getList().ToList();
            foreach (IGSMvariable par in CommResources.GsmVariables)
            {
                IGSMvariable gSMvariable = par as IGSMvariable;
                CommResources.GsmVariablesView.Add(gSMvariable);
            }

            IList<string> GsmCommandTypeList = new List<string> { "Parametri", "Variabili di test" };
            CommResources.GsmCommandTypes = new CollectionView(GsmCommandTypeList);
            CommResources.GsmCommandType = "Parametri";

            RAMPageModel RamPage = new RAMPageModel();
            RamPage.Name = "Ram Measures";
            RamPage.Index = ReadVarBundle.RAM_MEASURE_BUNDLE;
            CommResources.DeviceRamBundlesView.Add(RamPage);

            RamPage = new RAMPageModel();
            RamPage.Name = "Ram Others";
            RamPage.Index = ReadVarBundle.RAM_OTHERS_BUNDLE;
            CommResources.DeviceRamBundlesView.Add(RamPage);

            RamPage = new RAMPageModel();
            RamPage.Name = "Ram Temperature An Pressure";
            RamPage.Index = ReadVarBundle.RAM_BUNDLE_T1_T2_PRESS;
            CommResources.DeviceRamBundlesView.Add(RamPage);

            RamPage = new RAMPageModel();
            RamPage.Name = "Ram Bluetooth/RS485";
            RamPage.Index = ReadVarBundle.RAM_BUNDLE_BT_RS485;
            CommResources.DeviceRamBundlesView.Add(RamPage);

            CommResources.CheckComPorts();
        }


        public Parameters DeviceParameters
        {
            get
            {
                return Parameters.Instance;
            }
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
