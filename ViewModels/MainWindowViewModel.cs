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
using System.Windows.Input;

namespace EuromagProtocolUtility
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            CommResources.FrameTypesView = new ObservableCollection<FrameTypeVis>();
            CommResources.BuildFrameTypeView();

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
           
            IList<int> BaudRateList = new List<int>();
            BaudRateList.Add(9600);
            BaudRateList.Add(19200);
            BaudRateList.Add(38400);
            BaudRateList.Add(57600);
            BaudRateList.Add(76800);
            BaudRateList.Add(115200);
            BaudRates = new CollectionView(BaudRateList);

            UserCommPort = Properties.Settings.Default.UserComPort;
            UserBaudrate = Properties.Settings.Default.UserBaudRate;
            DataChanged = false;

            CommResources.CheckComPorts();

            CommResources.OpenCom(UserCommPort, UserBaudrate);
        }

        public CollectionView BaudRates { get; set; }

        private int _userBaudrate;
        public int UserBaudrate
        {
            get { return _userBaudrate; }
            set
            {
                if (value != _userBaudrate)
                {
                    _userBaudrate = value;
                    DataChanged = true;
                    OnPropertyChanged("UserBaudrate");
                }
            }
        }

        private string _userCommPort;
        public string UserCommPort
        {
            get { return _userCommPort; }
            set
            {
                if (value != _userCommPort)
                {
                    _userCommPort = value;
                    DataChanged = true;
                    OnPropertyChanged("UserCommPort");
                }
            }
        }

        private bool _dataChanged;
        public bool DataChanged
        {
            get { return _dataChanged; }
            set
            {
                if (value != _dataChanged)
                {
                    _dataChanged = value;
                    OnPropertyChanged("DataChanged");
                }
            }
        }

        private ICommand _saveComCmd;
        public ICommand SaveComCmd
        {
            get
            {
                if (_saveComCmd == null)
                {
                    _saveComCmd = new RelayCommand(
                        param => SaveCom()
                    );
                }
                return _saveComCmd;
            }
        }

        private void SaveCom()
        {
            Properties.Settings.Default.UserComPort = UserCommPort;
            Properties.Settings.Default.UserBaudRate = UserBaudrate;

            if (CommResources.OpenCom(UserCommPort, UserBaudrate))
            {
                DataChanged = false;
                Properties.Settings.Default.Save();
            }
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
