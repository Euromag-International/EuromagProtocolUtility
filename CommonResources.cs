using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UniversalConnect.Memory.Parameters;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using UniversalConnect.CommunicationFrames;
using UniversalConnect.Memory;
using UniversalConnect.Models;
using UniversalConnect.StdCommands;
using UniversalConnect;

namespace EuromagProtocolUtility
{
    public class CommonResources : ObservableObject
    {
        public CommonResources()
        {
            ControlsEnable = false;
            WriteFrameDataVisibility = Visibility.Collapsed;
            ReadFrameDataVisibility = Visibility.Collapsed;
            ParOptionsListVisibility = Visibility.Collapsed;
            GsmParamVisibility = Visibility.Visible;
            GsmVariableVisibility = Visibility.Collapsed;
            GsmParamSelected = true;
            ParDescription = string.Empty;
        }        

        private static CommonResources _instance;
        public static CommonResources Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CommonResources();
                return _instance;
            }
        }

        #region Connessione

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

        private commPortHandler _portHandler;
        public commPortHandler portHandler
        {
            get { return _portHandler; }
            set
            {
                _portHandler = value;
                OnPropertyChanged("portHandler");
            }
        }

        IList<string> SerialPortsList = new List<string>();

        private CollectionView _serialPortsCom;
        public CollectionView SerialPortsCom
        {
            get { return _serialPortsCom; }
            set
            {
                if (value != _serialPortsCom)
                {
                    _serialPortsCom = value;
                    OnPropertyChanged("SerialPortsCom");
                }
            }
        }

        public bool CheckComPorts()
        {
            string[] ports = SerialPort.GetPortNames();

            if ((ports != null) && (ports.Length != 0))
            {
                SerialPortsList.Clear();
                foreach (string port in ports)
                    if (SerialPortsList.Contains(port) == false)
                        SerialPortsList.Add(port);

                SerialPortsCom = new CollectionView(SerialPortsList);
                return true;
            }
            else
                return false;
        }

        public bool OpenCom(string _comPort, int _baudRate)
        {
            UserCommPort = _comPort;
            UserBaudrate = _baudRate;

            if ((UserCommPort == null)||(UserCommPort == ""))
            {
                MessageBox.Show("Selezionare una porta COM", "COM Port Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (UserBaudrate == 0)
            {
                MessageBox.Show("Selezionare un BaudRate", "COM Port Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (portHandler != null)
            {
                if (portHandler.IsOpen)
                    portHandler.close();
            }

            portHandler = new commPortHandler(UserCommPort, UserBaudrate);
            portHandler.FlowContr = false;

            if (portHandler.open())
            {
                CommPortReady = true;
                UniversalConnect.Utility.PortMonitor.Init();
                UniversalConnect.Utility.PortMonitor.MonitorOn = true;
                return true;
            }
            else
            {
                MessageBox.Show(portHandler.PortName + " is unavailable", "COM Port Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private bool _commPortReady;
        public bool CommPortReady
        {
            get { return _commPortReady; }
            set
            {
                if (value != _commPortReady)
                {
                    _commPortReady = value;
                    OnPropertyChanged("CommPortReady");
                }
            }
        }

        private ICommand _sendCommandCmd;
        public ICommand SendCommandCmd
        {
            get
            {
                if (_sendCommandCmd == null)
                {
                    _sendCommandCmd = new RelayCommand(
                        param => SendCommand()
                    );
                }
                return _sendCommandCmd;
            }
        }

        private void SendCommand()
        {
            switch (FrameTypeCode)
            {
                case FrameType.F_EEPROM_READ:
                    ReadEEPROMpar();
                    break;
                case FrameType.F_EEPROM_WRITE:
                    WriteEEPROMpar();
                    break;
                case FrameType.F_VARS_BUNDLE_READ:
                    ReadVarBundles();
                    break;
                case FrameType.F_VAR_READ:
                    ReadVars();
                    break;
                case FrameType.F_REDIRECT_PAR_RD_REQ:
                    ReadGsmPars();
                    break;
                case FrameType.F_REDIRECT_PAR_WR_REQ:
                    WriteGsmPars();
                    break;
                case FrameType.F_REDIRECT_TEST_REQ:
                    if (GsmTestWriteSelected)
                        WriteGsmTest();
                    else
                        ReadGsmTests();
                    break;
                case FrameType.F_OPERATION_REQUEST:
                    SetOperation();
                    break;
            }
        }

        private void WriteGsmTest()
        {
            MonitorInBufferLoc.Clear();

            WriteGSMTest writeGSMTest = new WriteGSMTest(portHandler);

            switch (SelectedGsmPar.DataType)
            {
                case TargetDataType.TYPE_UC:
                    writeGSMTest.Variable = GsmBytePar;
                    break;
                case TargetDataType.TYPE_US:
                    writeGSMTest.Variable = GsmUint16Par;
                    break;
                case TargetDataType.TYPE_SS:
                    writeGSMTest.Variable = GsmInt16Par;
                    break;
                case TargetDataType.TYPE_UL:
                    writeGSMTest.Variable = GsmUInt32Par;
                    break;
                case TargetDataType.TYPE_SL:
                    break;
                case TargetDataType.TYPE_FL:
                    writeGSMTest.Variable = GsmFloatPar;
                    break;
                case TargetDataType.TYPE_DB:
                    writeGSMTest.Variable = GsmDoublePar;
                    break;
                case TargetDataType.TYPE_STR:
                    writeGSMTest.Variable = GsmStrPar;
                    break;
                case TargetDataType.TYPE_DATA:
                    break;
                case TargetDataType.TYPE_ENUM:
                    writeGSMTest.Variable = GsmEnumPar;
                    break;
            }

            writeGSMTest.Completed += WriteGSMTest_Completed;
            writeGSMTest.send();

        }

        private void WriteGSMTest_Completed(object sender, CommandCompletedEventArgs e)
        {
            WriteGSMTest Cmd = sender as WriteGSMTest;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= WriteGSMTest_Completed;

                MessageBox.Show("Scrittura Riuscita: " + Cmd.Variable.ValAsString, "WriteGSMTest", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Scrittura Non Riuscita", "WriteGSMTest", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteGsmPars()
        {
            MonitorInBufferLoc.Clear();

            WriteGSMPar writeGSMPar = new WriteGSMPar(portHandler);

            switch (SelectedGsmPar.DataType)
            {
                case TargetDataType.TYPE_UC:
                    writeGSMPar.Variable = GsmBytePar;
                    break;
                case TargetDataType.TYPE_US:
                    writeGSMPar.Variable = GsmUint16Par;
                    break;
                case TargetDataType.TYPE_SS:
                    writeGSMPar.Variable = GsmInt16Par;
                    break;
                case TargetDataType.TYPE_UL:
                    writeGSMPar.Variable = GsmUInt32Par;
                    break;
                case TargetDataType.TYPE_SL:
                    break;
                case TargetDataType.TYPE_FL:
                    writeGSMPar.Variable = GsmFloatPar;
                    break;
                case TargetDataType.TYPE_DB:
                    writeGSMPar.Variable = GsmDoublePar;
                    break;
                case TargetDataType.TYPE_STR:
                    writeGSMPar.Variable = GsmStrPar;
                    break;
                case TargetDataType.TYPE_DATA:
                    break;
                case TargetDataType.TYPE_ENUM:
                    writeGSMPar.Variable = GsmEnumPar;
                    break;
            }

            writeGSMPar.Completed += WriteGSMPar_Completed;
            writeGSMPar.send();

        }

        private void WriteGSMPar_Completed(object sender, CommandCompletedEventArgs e)
        {
            WriteGSMPar Cmd = sender as WriteGSMPar;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= WriteGSMPar_Completed;

                MessageBox.Show("Scrittura Riuscita: " + Cmd.Variable.ValAsString, "WriteGSMPar", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Scrittura Non Riuscita", "WriteGSMPar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetOperation()
        {
            MonitorInBufferLoc.Clear();

            Operation operation = new Operation(portHandler);
            operation.OperationCode = OperationToSend;
            operation.Completed += Operation_Completed;
            operation.send();
        }

        private void Operation_Completed(object sender, CommandCompletedEventArgs e)
        {
            Operation Cmd = sender as Operation;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= Operation_Completed;

                MessageBox.Show("Comando Inviato", "Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Comando Fallito", "Operation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadGsmTests()
        {
            MonitorInBufferLoc.Clear();

            ReadGSMTest readGSMTest = new ReadGSMTest(portHandler);
            readGSMTest.Variable = SelectedGsmPar;
            readGSMTest.Completed += ReadGSMTest_Completed;
            readGSMTest.send();
        }

        private void ReadGSMTest_Completed(object sender, CommandCompletedEventArgs e)
        {
            ReadGSMTest Cmd = sender as ReadGSMTest;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= ReadGSMTest_Completed;

                MessageBox.Show("Lettura Riuscita: " + Cmd.Variable.ValAsString, "ReadGSMTest", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Lettura Non Riuscita", "ReadGSMTest", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadGsmPars()
        {
            MonitorInBufferLoc.Clear();

            ReadGSMPar readGSMPar = new ReadGSMPar(portHandler);
            readGSMPar.Variable = SelectedGsmPar;
            readGSMPar.Completed += ReadGSMPar_Completed;
            readGSMPar.send();
        }

        private void ReadGSMPar_Completed(object sender, CommandCompletedEventArgs e)
        {
            ReadGSMPar Cmd = sender as ReadGSMPar;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= ReadRAM_Completed;

                MessageBox.Show("Lettura Riuscita: " + Cmd.Variable.ValAsString, "ReadGSMPar", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Lettura Non Riuscita", "ReadGSMPar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadVars()
        {
            MonitorInBufferLoc.Clear();

            ReadRAM readRAM = new ReadRAM(portHandler);
            readRAM.Variable = SelectedVar;
            readRAM.Completed += ReadRAM_Completed;
            readRAM.send();
        }

        private void ReadRAM_Completed(object sender, CommandCompletedEventArgs e)
        {
            ReadRAM Cmd = sender as ReadRAM;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= ReadRAM_Completed;

                MessageBox.Show("Lettura Riuscita: " + Cmd.Variable.ValAsString, "ReadRAM", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Lettura Non Riuscita", "ReadRAM", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadVarBundles()
        {
            MonitorInBufferLoc.Clear();

            ReadVarBundle readVarBundle = new ReadVarBundle(portHandler);
            readVarBundle.BundleId = BundleIndex;
            readVarBundle.Completed += ReadVarBundle_Completed;
            readVarBundle.send();
        }

        private void ReadVarBundle_Completed(object sender, CommandCompletedEventArgs e)
        {
            ReadVarBundle Cmd = sender as ReadVarBundle;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= ReadVarBundle_Completed;

                MessageBox.Show("Lettura Riuscita", "ReadVarBundle", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                if (Cmd.BundleId >= ReadVarBundle.RAM_BUNDLE_BASE)
                {
                    DeviceRamBundlesListView.Clear();
                    for (int i = 0; i < Cmd.BundleVarablesList.Count; i++)
                    {
                        RAMVariableModel var = new RAMVariableModel();
                        var.Description = Cmd.BundleVarablesList[i].Name;
                        var.ValAsString = Cmd.BundleVarablesList[i].ValAsString;
                        DeviceRamBundlesListView.Add(var);
                    }
                }
                else
                {
                    DeviceEEpromBundlesListView.Clear();
                    for (int i = 0; i < Cmd.BundleVarablesList.Count; i++)
                    {
                        ParameterView var = new ParameterView();
                        var.Description = Cmd.BundleVarablesList[i].Name;
                        var.ValAsString = Cmd.BundleVarablesList[i].ValAsString;
                        DeviceEEpromBundlesListView.Add(var);
                    }
                }

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Lettura Non Riuscita", "ReadVarBundle", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReadEEPROMpar()
        {
            MonitorInBufferLoc.Clear();

            ReadEEPROM readEEPROM = new ReadEEPROM(portHandler);
            readEEPROM.Variable = DeviceParameters.DeviceParameterList[SelectedPar.Index - 1];
            readEEPROM.Completed += ReadEEPROM_Completed;
            readEEPROM.send();
        }

        private void ReadEEPROM_Completed(object sender, CommandCompletedEventArgs e)
        {
            ReadEEPROM Cmd = sender as ReadEEPROM;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= ReadEEPROM_Completed;

                MessageBox.Show("Lettura Riuscita: " + Cmd.Variable.ValAsString, "ReadEEPROM", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Lettura Non Riuscita", "ReadEEPROM", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void WriteEEPROMpar()
        {
            MonitorInBufferLoc.Clear();

            WriteEEPROM writeEEPROM = new WriteEEPROM(portHandler);

            ITargetWritable var = DeviceParameters.DeviceParameterList[SelectedPar.Index - 1];

            switch (var.DataType)
            {
                case TargetDataType.TYPE_UC:
                    writeEEPROM.Variable = BytePar;
                    break;
                case TargetDataType.TYPE_US:
                    writeEEPROM.Variable = Uint16Par;
                    break;
                case TargetDataType.TYPE_SS:
                    writeEEPROM.Variable = Int16Par;
                    break;
                case TargetDataType.TYPE_UL:
                    writeEEPROM.Variable = UInt32Par;
                    break;
                case TargetDataType.TYPE_SL:
                    break;
                case TargetDataType.TYPE_FL:
                    writeEEPROM.Variable = FloatPar;
                    break;
                case TargetDataType.TYPE_DB:
                    writeEEPROM.Variable = DoublePar;
                    break;
                case TargetDataType.TYPE_STR:
                    writeEEPROM.Variable = StrPar;
                    break;
                case TargetDataType.TYPE_DATA:
                    break;
                case TargetDataType.TYPE_ENUM:
                    writeEEPROM.Variable = EnumPar;
                    break;
            }

            writeEEPROM.Completed += WriteEEPROM_Completed;
            writeEEPROM.send();
        }

        private void WriteEEPROM_Completed(object sender, CommandCompletedEventArgs e)
        {
            WriteEEPROM Cmd = sender as WriteEEPROM;

            if (Cmd == null)
                return;

            if ((e.Error == null) &&
               (((CommandResult)(e.Result)).Outcome == CommandResultOutcomes.CommandSuccess))
            {
                Cmd.Completed -= WriteEEPROM_Completed;

                MessageBox.Show("Scrittura Riuscita: " + Cmd.Variable.ValAsString, "WriteEEPROM", MessageBoxButton.OK, MessageBoxImage.Information);
                MonitorInBufferLoc.AddRange(UniversalConnect.Utility.PortMonitor.MonitorInBuffer);
                UniversalConnect.Utility.PortMonitor.MonitorInBuffer.Clear();

                CommandFrame = "";

                foreach (byte _byte in MonitorInBufferLoc)
                {
                    string value = _byte.ToString("X");
                    if (value.Length == 1)
                        CommandFrame += "0";

                    CommandFrame += _byte.ToString("X");
                }

                WriteFrameDataVisibility = Visibility.Visible;
                ReadFrameDataVisibility = Visibility.Collapsed;

                ViewCommandFrame(CommandFrame);

            }
            else
            {
                MessageBox.Show("Scrittura Non Riuscita", "WriteEEPROM", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<byte> _monitorInBufferLoc;
        public List<byte> MonitorInBufferLoc
        {
            get
            {
                if (_monitorInBufferLoc == null)
                    _monitorInBufferLoc = new List<byte>();
                return _monitorInBufferLoc;
            }
            set
            {
                if (value != _monitorInBufferLoc)
                {
                    _monitorInBufferLoc = value;
                    OnPropertyChanged("MonitorInBufferLoc");
                }
            }
        }

        #endregion


        public CollectionView GsmCommandTypes { get; set; }

        public enum FrameType
        {
            F_WAIT = 0x01,   //Frame di risposta, indica che l'operaione richiesta è in corso, inviare POKE
            F_REFUSED = 0x02,   //Frame di risposta, indica che la scheda ha rifiutato il comando
            F_UNKNOWN = 0x03,   //Frame di risposta, indica che la scheda non ha riconosciuto il comando
            F_EXPIRED = 0x04,   //Frame di risposta, l'operazione richiesta è scaduta
            F_POKE = 0x08,   //Frame di invio da mandare dopo una risposta WAIT, serve a richiedere lo stato della scheda

            // user's frames
            F_EEPROM_READ = 0x10,	//Lettura parametri EEPROM, la risposta è sempre 0x10            
            F_EEPROM_WRITE = 0x11,	//Scrittura parametri EEPROM
            F_EEPROM_WRITE_OK = 0x21,   //Risposta a 0x11 se tutto OK
            F_EEPROM_WRITE_VCHANGED = 0x22,	//indica che il valore scritto eccedeva i limiti ed é stato modificato
            F_EEPROM_WRITE_ERROR = 0x23,	//parametro non scritto a causa di un errore di limiti
            F_EEPROM_WRITE_FAILED = 0x24,   //parametro non scritto a casua di un errore dell'EEPROM

            F_EEPROM_PAGE_READ = 0x18,   //Lettura pagina EEPROM, la risposta se tutto ok è 0x18
            F_EEPROM_PAGE_READ_ERROR = 0x19,   //Errore di lettura pagina EEEPROM
            F_EEPROM_PAGE_WRITE = 0x28,   //Scrittur pagina EEPROM, la risposta se tutto ok è 0x28
            F_EEPROM_PAGE_WRITE_ERROR = 0x29,   //Errore scrittura pagina EEPROM

            F_VAR_READ = 0x30,	//Lettura variabile RAM

            F_LOG_READ = 0x40,	//Richiesta lettura righe di log
            F_LOG_INVALID_ROW = 0x41,   //Risposta di errore a 0x40
            F_LOG_LOCKED = 0x42,   //Risposta di errore a 0x40
            F_LOG_READ_ERROR = 0x43,   //Risposta di errore a 0x40

            F_LOG_ERASE = 0x46,   //Comando di erase log
            F_LOG_ERASE_ERROR = 0x47,   //Risposta di errore a 0x46

            F_LOG_READ_LAST = 0x48,   //Lettura ultima riga di log

            F_LOG_START_FAST_DOWN = 0x4A,   //Comando start fast download log
            F_LOG_START_FAST_OK = 0x4B,   //Risposta a 0x4A

            F_GET_EVENT = 0x50,   //Richiesta log eventi
            F_GET_EVENT_ERROR = 0x51,   //Risposta di errore a 0x50
            F_ERASE_EVENT_LOG = 0x5A,   //Richiesta erase log eventi
            F_ERASE_EVENT_LOG_ERROR = 0x5B,   //Risposta di errore a 0x5A

            F_TARGET_RESET_REQUEST = 0x70,   //Richiesta reset eventi
            F_TARGET_RESET_RESPONSE = 0x71,   //Risposta a 0x70

            F_SET_DATETIME_REQUEST = 0x78,   //Impostazione data/ora
            F_SET_DATETIME_RESPONSE = 0x79,   //Risposta a 0x78
            F_GET_DATETIME_REQUEST = 0x7A,   //Richiesta data/ora
            F_GET_DATETIME_RESPONSE = 0x7B,   //Risposta a 0x7A

            F_OPERATION_REQUEST = 0x80,   //Invio comandi speciali, nel campo address va inserito:
                                          //0x00 - none
                                          //0x01 - Calibrazione dello zero
                                          //0x02 - Reset Parziale positivo
                                          //0x03 - Reset Parziale negativo
                                          //0x04 - Salvataggio configurazione utente
                                          //0x05 - Caricamento configurazione utente
                                          //0x06 - Caricamento default di fabbrica
                                          //0x07 - Attivazione deviazione standard campioni zero
                                          //0x08 - Disattivazione deviazione standard campioni zero
                                          //0xAD - Salvataggio parametri di fabbrica
                                          //0xC5 - Ripristino default parametri
                                          //0xF2 - Reset Totalizzatore positivo
                                          //0xF3 - Reset Totalizzatore negativo
                                          //0xDB - Attiva modalità di configurazione GSM
                                          //0x20 - Attiva stream dati via RS485
                                          //0x22 - Attiva stream dati via Bluetooth

            F_OPERATION_RESP_OK = 0x81,   //Risposta a 0x80
            F_OPERATION_RESP_ERR = 0x82,   //Risposta di errore 0x80

            F_VARS_BUNDLE_READ = 0x90,   //Lettura bundle variabili/parametri
            F_VARS_BUNDLE_OK = 0x91,   //Risposta a 0x90
            F_VARS_BUNDLE_READ_ERR = 0x92,   //Risposta di errore a 0x90

            F_TEST_OUTPUT_SET = 0xA0,   //Attiva le uscite digitali, nel campo data va scritto un UInt16:
                                        //0x0001 - Impulsi positivi
                                        //0x0002 - Impulsi negativi
                                        //0x0004 - Led rosso
                                        //0x0008 - Led giallo
                                        //Campo bit field, se 1 attivo, se 0 disattivo
            F_TEST_OUTPUT_RESP_OK = 0xA1, //Risposta a 0xA0

            F_TEST_INPUT_READ_REQ = 0xA2, //Lettura stato ingressi digitali
            F_TEST_INPUT_READ_RESP = 0xA3, //Risposta a 0xA2, nel campo data ritorna una UInt16:
                                           //0x0001 - Bottone 1
                                           //0x0002 - Bottone 2
                                           //0x0004 - Bottone 3
                                           //0x0008 - Bottone 4
                                           //0x0010 - Reed
                                           //0x0020 - Ingresso digitale
                                           //Campo bit field, se 1 attivo, se 0 disattivo

            F_TEST_STATUS_READ_REQ = 0xA4, //Lettura stato ingressi convertitore, nel campo data ritorna una UInt16:
            F_TEST_STATUS_READ_RESP = 0xA5, //Risposta a 0xA4, nel campo data ritorna una UInt32:
                                            //EE_DETECTED_BIT         = 0x00000001U;
                                            //SEE_DETECTED_BIT        = 0x00000002U;
                                            //SEE_SECURED_BIT         = 0x00000004U;
                                            //FLASH_DETECTED_BIT      = 0x00000008U;
                                            //TESTBENCH_DETECTED_BIT  = 0x00000010U;
                                            //AUX_PIN_TEST_PASSED_BIT = 0x00000020U;
                                            //EXCITATION_FAILURE_BIT  = 0x00010000U;
                                            //EMPTY_PIPE_BIT          = 0x00020000U;
                                            //FLOW_MAX_BIT            = 0x00040000U;
                                            //FLOW_MIN_BIT            = 0x00080000U;
                                            //PULSES_OVERLAP_BIT      = 0x00100000U;
                                            //ADC_OVER_RANGE_BIT      = 0x00200000U;
                                            //INPUT_STAGE_BIT         = 0x00400000U;
                                            //ELECTRODE_DRY_BIT       = 0x00800000U;
                                            //LOW_VOLTAGE_BIT         = 0x01000000U;
                                            //HIGH_TEMP_BIT           = 0x02000000U;
                                            //LOW_TEMP_BIT            = 0x04000000U;
                                            //FIRMWARE_CRC32_BIT      = 0x08000000U;
                                            //INPUT_COMMON_SATURATED_BIT       = 0x10000000U;
                                            //INPUT_DIFFERENTIAL_SATURATED_BIT = 0x20000000U;
                                            //EEPROM_CRC16_BIT                 = 0x40000000U;
                                            //PCB_HUMID_BIT                    = 0x80000000U;
                                            //Campo bit field, se 1 attivo, se 0 disattivo

            F_TEST_DISPLAY_SEG = 0xA6, //Test segmenti display
            F_TEST_DISPLAY_SEG_RESP_OK = 0xA7, //Risposta a 0xA6

            F_ENTER_SUSPEND_REQUEST = 0xB0, //Manda il convertitore in modalità sospensione
            F_ENTER_SUSPEND_RESPONSE = 0xB1, //Risposta a 0xB1

            F_4_20_MA_SIMUL_MODE_ENABLE_REQ = 0xB2, //Abilita modalità simulazione
            F_4_20_MA_SIMUL_MODE_ENABLE_RESP = 0xB3, //Risposta a 0xB2

            F_4_20_MA_SIMUL_MODE_STATUS_REQ = 0xB4, //Richiede lo stato della modalità di simulazione
            F_4_20_MA_SIMUL_MODE_STATUS_RESP = 0xB5, //Risposta a 0xB4

            F_4_20_MA_SIMUL_MODE_GET_FLOW_LEV_REQ = 0xB6, //Richiede il valore attuale di portata simulata in m/s/10 (100 = 10.0 m/s)
            F_4_20_MA_SIMUL_MODE_GET_FLOW_GET_RESP = 0xB7, //Risposta a 0xB6

            F_4_20_MA_SIMUL_MODE_SET_FLOW_LEV_REQ = 0xB8, //Imposta il valore di portata simulata in m/s/10 (100 = 10.0 m/s)
            F_4_20_MA_SIMUL_MODE_SET_FLOW_LEV_RESP = 0xB9, //Risposta a 0xB8

            F_VERIFY_MODE_ENABLE_REQ = 0xC0, //Imposta la modlità verifica
            F_VERIFY_MODE_ENABLE_RESP = 0xC1, //Risposta a 0xC1
            F_VERIFY_MODE_STATUS_REQ = 0xC2, //Richiede lo stato della modalità verifica (Disattiva/Attiva) 
            F_VERIFY_MODE_STATUS_RESP = 0xC3, //Risposta a 0xC3

            F_GET_DEVICE_CERTIFICATION = 0xC4,  //Richiede stato certificazione Standard/MID
            F_GET_DEVICE_CERTIFICATION_OK = 0xC5,  //Risposta a 0xC4

            F_CHECK_EEPROM_JUMPER = 0xC6,     //Richiede lo stato del jummper della EEPROM Safe
            F_CHECK_EEPROM_JUMPER_OK = 0xC7,     //Risposta a 0xC7

            F_RESET_MAIN_PWR_INTERR_INFO = 0xC8,     //Richiedo il log dello stato di interruzione alimentazione
            F_RESET_MAIN_PWR_INTERR_DONE = 0xC9,     //Risposta a 0xC9

            F_GET_ERRORS = 0xCA,    //Richiede lo stato degli errori attivi
            F_GET_ERRORS_RESPONSE = 0xCB,    //Risposta a 0xCA

            F_CLEAR_ERRORS = 0xCC,    //Richiede cancellazione errori attivi
            F_CLEAR_ERRORS_RESPONSE = 0xCD,    //Risposta a 0xCC

            F_REDIRECT_PAR_RD_REQ = 0xD1,    //parameter read request from configuration software to GSM
            F_REDIRECT_PAR_RD_RESP = 0xD2,    //response to 0xD1 from GSM to configuration software
            F_REDIRECT_PAR_WR_REQ = 0xD3,    //parameter write request from configuration software to GSM
            F_REDIRECT_PAR_WR_RESP = 0xD4,    //response to 0xD3 from GSM to configuration software
            F_GSM_STATUS_WRITE = 0xD5,    //status info transmitted from the GSM to be stored in RAM
            F_GSM_STATUS_RESP = 0xD6,    //response to 0xD5
            F_GSM_ID_WRITE = 0xD7,    //GSM identification info transmitted from the GSM to be stored in RAM
            F_GSM_ID_RESP = 0xD8,    //response to 0xD7
            F_REDIRECT_TEST_REQ = 0xD9,    //sent from configuration software to set the modem in test/debug mode
            F_REDIRECT_TEST_RESP = 0xD0,    //response to 0xD9
            F_GSM_STATUS_READ_REQ = 0xDA,    //status info stored in RAM read request
            F_GSM_STATUS_READ_RESP = 0xDB,    //response to 0xDA
            F_ERROR_PAR_RD_REQ = 0xE1,    //error response frame over 0xD1 request
            F_ERROR_PAR_WR_REQ = 0xE2,    //error response frame over 0xD3 request
            F_ERROR_GSM_STATUS_WR = 0xE3,    //error response frame over 0xD5 request
            F_ERROR_GSM_ID_WR = 0xE4,    //error response frame over 0xD7 request
            F_ERROR_GSM_ENTER_TEST = 0xE5     //error response frame over 0xD9 request
        }

        private ObservableCollection<ParameterView> _deviceParametersView;
        public ObservableCollection<ParameterView> DeviceParametersView
        {
            get
            {
                if (_deviceParametersView == null)
                    _deviceParametersView = new ObservableCollection<ParameterView>();

                return _deviceParametersView;
            }
            set
            {
                if (_deviceParametersView != value)
                {
                    _deviceParametersView = value;
                    OnPropertyChanged("DeviceParametersView");
                }
            }
        }

        private ObservableCollection<ParameterView> _deviceParametersViewFiltered;
        public ObservableCollection<ParameterView> DeviceParametersViewFiltered
        {
            get
            {
                if (_deviceParametersViewFiltered == null)
                    _deviceParametersViewFiltered = new ObservableCollection<ParameterView>();

                return _deviceParametersViewFiltered;
            }
            set
            {
                if (_deviceParametersViewFiltered != value)
                {
                    _deviceParametersViewFiltered = value;
                    OnPropertyChanged("DeviceParametersViewFiltered");
                }
            }
        }

        public class RAMPageModel
        {
            public string Name { get; set; }
            public UInt32 Index { get; set; }
        }

        private ObservableCollection<RAMPageModel> _deviceRamBundlesView;
        public ObservableCollection<RAMPageModel> DeviceRamBundlesView
        {
            get
            {
                if (_deviceRamBundlesView == null)
                    _deviceRamBundlesView = new ObservableCollection<RAMPageModel>();

                return _deviceRamBundlesView;
            }
            set
            {
                if (_deviceRamBundlesView != value)
                {
                    _deviceRamBundlesView = value;
                    OnPropertyChanged("DeviceRamBundlesView");
                }
            }
        }

        private ObservableCollection<PageModel> _deviceEEpromBundlesView;
        public ObservableCollection<PageModel> DeviceEEpromBundlesView
        {
            get
            {
                if (_deviceEEpromBundlesView == null)
                    _deviceEEpromBundlesView = new ObservableCollection<PageModel>();

                return _deviceEEpromBundlesView;
            }
            set
            {
                if (_deviceEEpromBundlesView != value)
                {
                    _deviceEEpromBundlesView = value;
                    OnPropertyChanged("DeviceEEpromBundlesView");
                }
            }
        }


        private ObservableCollection<ParameterView> _deviceEEpromBundlesListView;
        public ObservableCollection<ParameterView> DeviceEEpromBundlesListView
        {
            get
            {
                if (_deviceEEpromBundlesListView == null)
                    _deviceEEpromBundlesListView = new ObservableCollection<ParameterView>();

                return _deviceEEpromBundlesListView;
            }
            set
            {
                if (_deviceEEpromBundlesListView != value)
                {
                    _deviceEEpromBundlesListView = value;
                    OnPropertyChanged("DeviceEEpromBundlesListView");
                }
            }
        }

        private List<ITargetWritable> _deviceParameterList;
        public List<ITargetWritable> DeviceParameterList
        {
            get
            {
                if (_deviceParameterList == null)
                    _deviceParameterList = new List<ITargetWritable>();
                return _deviceParameterList;
            }
            private set
            {
                if (_deviceParameterList != value)
                {
                    _deviceParameterList = value;
                    OnPropertyChanged("DeviceParameterList");
                }
            }
        }

        private string _gsmCommandType;
        public string GsmCommandType
        {
            get { return _gsmCommandType; }
            set
            {
                if (_gsmCommandType != value)
                {
                    _gsmCommandType = value;

                    switch (_gsmCommandType)
                    {
                        case "Parametri":
                            GsmParamVisibility = Visibility.Visible;
                            GsmVariableVisibility = Visibility.Collapsed;
                            GsmParamSelected = true;
                            break;
                        case "Variabili di test":
                            GsmParamVisibility = Visibility.Collapsed;
                            GsmVariableVisibility = Visibility.Visible;
                            GsmParamSelected = false;
                            break;
                    }

                    OnPropertyChanged("GsmCommandType");
                }
            }
        }

        private Visibility _gsmParamVisibility;
        public Visibility GsmParamVisibility
        {
            get { return _gsmParamVisibility; }
            set
            {
                if (_gsmParamVisibility != value)
                {
                    _gsmParamVisibility = value;
                    OnPropertyChanged("GsmParamVisibility");
                }
            }
        }

        private Visibility _gsmVariableVisibility;
        public Visibility GsmVariableVisibility
        {
            get { return _gsmVariableVisibility; }
            set
            {
                if (_gsmVariableVisibility != value)
                {
                    _gsmVariableVisibility = value;
                    OnPropertyChanged("GsmVariableVisibility");
                }
            }
        }

        private bool _gsmParamSelected;
        public bool GsmParamSelected
        {
            get { return _gsmParamSelected; }
            set
            {
                if (_gsmParamSelected != value)
                {
                    _gsmParamSelected = value;
                    OnPropertyChanged("GsmParamSelected");
                }
            }
        }

        private bool _gsmTestWriteSelected;
        public bool GsmTestWriteSelected
        {
            get { return _gsmTestWriteSelected; }
            set
            {
                if (_gsmTestWriteSelected != value)
                {
                    _gsmTestWriteSelected = value;
                    OnPropertyChanged("GsmParamSelected");
                }
            }
        }


        private string _deviceParametersFilter;
        public string DeviceParametersFilter
        {
            get { return _deviceParametersFilter; }
            set
            {
                if (_deviceParametersFilter != value)
                {
                    _deviceParametersFilter = value;
                    OnPropertyChanged("DeviceParametersFilter");
                }
            }
        }

        public void DeviceParametersFilterExec()
        {
            DeviceParametersViewFiltered.Clear();

            if (DeviceParametersFilter == null)
                DeviceParametersFilter = "";

            foreach (ParameterView par in DeviceParametersView)
            {
                if (par.Description != null)
                {
                    if (par.Description.Contains(DeviceParametersFilter, StringComparison.OrdinalIgnoreCase))
                        DeviceParametersViewFiltered.Add(par);
                }
            }
        }


        private List<ITargetVariable> _deviceVariables;
        public List<ITargetVariable> DeviceVariables
        {
            get
            {
                if (_deviceVariables == null)
                    _deviceVariables = new List<ITargetVariable>();

                return _deviceVariables;
            }
            set
            {
                if (_deviceVariables != value)
                {
                    _deviceVariables = value;
                    OnPropertyChanged("DeviceVariables");
                }
            }
        }

        private ObservableRangeCollection<ITargetVariable> _deviceVariablesView;
        public ObservableRangeCollection<ITargetVariable> DeviceVariablesView
        {
            get
            {
                if (_deviceVariablesView == null)
                    _deviceVariablesView = new ObservableRangeCollection<ITargetVariable>();

                return _deviceVariablesView;
            }
            set
            {
                if (_deviceVariablesView != value)
                {
                    _deviceVariablesView = value;
                    OnPropertyChanged("DeviceVariablesView");
                }
            }
        }

        public class RAMVariableModel
        {
            public string Description { get; set; }
            public UInt32 Index { get; set; }
            public string ValAsString { get; set; }
        }

        private ObservableRangeCollection<RAMVariableModel> _deviceRamBundlesListView;
        public ObservableRangeCollection<RAMVariableModel> DeviceRamBundlesListView
        {
            get
            {
                if (_deviceRamBundlesListView == null)
                    _deviceRamBundlesListView = new ObservableRangeCollection<RAMVariableModel>();

                return _deviceRamBundlesListView;
            }
            set
            {
                if (_deviceRamBundlesListView != value)
                {
                    _deviceRamBundlesListView = value;
                    OnPropertyChanged("DeviceRamBundlesListView");
                }
            }
        }

        private List<ITargetWritable> _gsmVariables;
        public List<ITargetWritable> GsmVariables
        {
            get
            {
                if (_gsmVariables == null)
                    _gsmVariables = new List<ITargetWritable>();

                return _gsmVariables;
            }
            set
            {
                if (_gsmVariables != value)
                {
                    _gsmVariables = value;
                    OnPropertyChanged("GsmVariables");
                }
            }
        }

        private ObservableRangeCollection<ITargetWritable> _gsmVariablesView;
        public ObservableRangeCollection<ITargetWritable> GsmVariablesView
        {
            get
            {
                if (_gsmVariablesView == null)
                    _gsmVariablesView = new ObservableRangeCollection<ITargetWritable>();

                return _gsmVariablesView;
            }
            set
            {
                if (_gsmVariablesView != value)
                {
                    _gsmVariablesView = value;
                    OnPropertyChanged("GsmVariablesView");
                }
            }
        }

        private List<ITargetWritable> _gsmParameters;
        public List<ITargetWritable> GsmParameters
        {
            get
            {
                if (_gsmParameters == null)
                    _gsmParameters = new List<ITargetWritable>();

                return _gsmParameters;
            }
            set
            {
                if (_gsmParameters != value)
                {
                    _gsmParameters = value;
                    OnPropertyChanged("GsmParameters");
                }
            }
        }

        private ObservableRangeCollection<IGSMvariable> _gsmParametersView;
        public ObservableRangeCollection<IGSMvariable> GsmParametersView
        {
            get
            {
                if (_gsmParametersView == null)
                    _gsmParametersView = new ObservableRangeCollection<IGSMvariable>();

                return _gsmParametersView;
            }
            set
            {
                if (_gsmParametersView != value)
                {
                    _gsmParametersView = value;
                    OnPropertyChanged("GsmParametersView");
                }
            }
        }


        private ICommand _buildCsvFileCmd;
        public ICommand BuildCsvFileCmd
        {
            get
            {
                if (_buildCsvFileCmd == null)
                {
                    _buildCsvFileCmd = new RelayCommand(
                        param => this.BuildCsvFile()
                    );
                }
                return _buildCsvFileCmd;
            }
        }

        private void BuildCsvFile()
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.OverwritePrompt = true;
            saveDlg.DefaultExt = ".csv";

            saveDlg.FileName = "ProtocolMap_" + DeviceParameters.RunningKey.ToString() + ".csv";
            saveDlg.Filter = "CSV File (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveDlg.ShowDialog() == true)
            {
                string _file = saveDlg.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(_file))
                    {
                        string TitleString = "****** Lista Parametri EEprom *****";
                        writer.WriteLine(TitleString);

                        string HeaderLineStr = "Indice File XML" +
                                               ";" +
                                               "Nome" +
                                               ";" +
                                               "Descrizione" +
                                               ";" +
                                               "Tipo Dato" +
                                               ";" +
                                               "Indirizzo EEprom" +
                                               ";" +
                                               "Indirizzo Protocollo" +
                                               ";" +
                                               "Valore di Default" +
                                               ";" +
                                               "Valore Massimo" +
                                               ";" +
                                               "Valore Minimo";

                        writer.WriteLine(HeaderLineStr);

                        foreach (ParameterView _par in DeviceParametersView)
                        {
                            FillParameterInfo(_par);

                            string ParLineStr = ParFileID +
                                                ";" +
                                                ParName +
                                                ";" +
                                                ParDescription +
                                                ";" +
                                                ParDataType +
                                                ";" +
                                                ParEEPRomId +
                                                ";" +
                                                ParProtocolId +
                                                ";" +
                                                ParDefault +
                                                ";" +
                                                ParMax +
                                                ";" +
                                                ParMin;

                            writer.WriteLine(ParLineStr);
                        }

                        TitleString = "****** Lista Parametri RAM *****";
                        writer.WriteLine(TitleString);

                        HeaderLineStr = "Nome" +
                                        ";" +
                                        "Tipo Dato" +
                                        ";" +
                                        "Indirizzo Protocollo";

                        writer.WriteLine(HeaderLineStr);

                        foreach (ITargetVariable _var in DeviceVariablesView)
                        {
                            FillVariableInfo(_var);

                            string VarLineStr = VarName +
                                                ";" +
                                                VarType +
                                                ";" +
                                                VarAddress;

                            writer.WriteLine(VarLineStr);
                        }

                        TitleString = "****** Lista Parametri GSM *****";
                        writer.WriteLine(TitleString);

                        HeaderLineStr = "Nome" +
                                        ";" +
                                        "Tipo Dato" +
                                        ";" +
                                        "Indirizzo Protocollo";

                        writer.WriteLine(HeaderLineStr);

                        foreach (IGSMvariable _var in GsmParametersView)
                        {
                            FillGSMParameterInfo(_var);

                            string GsmParLineStr = ParGsmName +
                                                   ";" +
                                                   ParGsmDataType.ToString() +
                                                   ";" +
                                                   ParGsmAddress.ToString();

                            writer.WriteLine(GsmParLineStr);
                        }

                        TitleString = "****** Lista Variabili GSM *****";
                        writer.WriteLine(TitleString);

                        HeaderLineStr = "Nome" +
                                        ";" +
                                        "Tipo Dato" +
                                        ";" +
                                        "Indirizzo Protocollo";

                        writer.WriteLine(HeaderLineStr);

                        foreach (IGSMvariable _var in GsmVariablesView)
                        {
                            FillGSMParameterInfo(_var);

                            string GsmParLineStr = ParGsmName +
                                                   ";" +
                                                   ParGsmDataType.ToString() +
                                                   ";" +
                                                   ParGsmAddress.ToString();

                            writer.WriteLine(GsmParLineStr);
                        }

                        writer.Close();

                        MessageBox.Show("File creato correttamente", "Salvataggio File CSV", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Errore Salvataggio File CSV", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private ICommand _buildCFileCmd;
        public ICommand BuildCFileCmd
        {
            get
            {
                if (_buildCFileCmd == null)
                {
                    _buildCFileCmd = new RelayCommand(
                        param => this.BuildCFile()
                    );
                }
                return _buildCFileCmd;
            }
        }

        uint MB_PAR_UINT8_TABLE_DIM = 0;
        uint MB_PAR_ENUM_TABLE_DIM = 0;
        uint MB_PAR_INT8_TABLE_DIM = 0;
        uint MB_PAR_UINT16_TABLE_DIM = 0;
        uint MB_PAR_INT16_TABLE_DIM = 0;
        uint MB_PAR_UINT32_TABLE_DIM = 0;
        uint MB_PAR_INT32_TABLE_DIM = 0;
        uint MB_PAR_PLACE_H_TABLE_DIM = 0;
        uint MB_PAR_FLOAT_TABLE_DIM = 0;
        uint MB_PAR_DOUBLE_TABLE_DIM = 0;
        uint MB_PAR_STRING_TABLE_DIM = 0;
        uint MB_PAR_DATA_TABLE_DIM = 0;

        private void BuildCFile()
        {
            MB_PAR_UINT8_TABLE_DIM = 0;
            MB_PAR_ENUM_TABLE_DIM = 0;
            MB_PAR_INT8_TABLE_DIM = 0;
            MB_PAR_UINT16_TABLE_DIM = 1;  // Nell'indice 0 c'è EE_KEY
            MB_PAR_INT16_TABLE_DIM = 0;
            MB_PAR_UINT32_TABLE_DIM = 0;
            MB_PAR_INT32_TABLE_DIM = 0;
            MB_PAR_PLACE_H_TABLE_DIM = 0;
            MB_PAR_FLOAT_TABLE_DIM = 0;
            MB_PAR_DOUBLE_TABLE_DIM = 0;
            MB_PAR_STRING_TABLE_DIM = 0;
            MB_PAR_DATA_TABLE_DIM = 0;

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.OverwritePrompt = true;
            saveDlg.DefaultExt = ".c";

            saveDlg.FileName = "MB_Database.c";
            saveDlg.Filter = "C File (*.c)|*.c|All files (*.*)|*.*";

            if (saveDlg.ShowDialog() == true)
            {
                string _file = saveDlg.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(_file))
                    {
                        string DummyString = "const mb_reindex_pars MB_Pars_Index_Table[MB_PARS_TABLE_SIZE] =";
                        writer.WriteLine(DummyString);

                        DummyString = "{";
                        writer.WriteLine(DummyString);

                        DummyString = "    {";
                        writer.WriteLine(DummyString);

                        DummyString = "    // EE_KEY = 0";
                        writer.WriteLine(DummyString);

                        DummyString = "        EE_KEY_TYPE,";
                        writer.WriteLine(DummyString);

                        DummyString = "         0 //MB_Par_uint16_Table";
                        writer.WriteLine(DummyString);

                        foreach (ParameterView _par in DeviceParametersView)
                        {
                            PrintParameter(_par, writer);
                        }

                        DummyString = "    }";
                        writer.WriteLine(DummyString);

                        DummyString = "};";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "//Tabelle locali parametri scheda di misura *************************************";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_UINT8_TABLE_DIM " + MB_PAR_UINT8_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_UINT8_TABLE_DIM != 0)
                        {
                            DummyString = "_uint8_t MB_Par_uint8_Table[MB_PAR_UINT8_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_uint8_t MB_Par_uint8_Table[MB_PAR_UINT8_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_ENUM_TABLE_DIM " + MB_PAR_ENUM_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_ENUM_TABLE_DIM != 0)
                        {
                            DummyString = "_ENUM MB_Par_enum_Table[MB_PAR_ENUM_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_ENUM MB_Par_enum_Table[MB_PAR_ENUM_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_INT8_TABLE_DIM " + MB_PAR_INT8_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_INT8_TABLE_DIM != 0)
                        {
                            DummyString = "int8_t MB_Var_int8_Table[MB_PAR_INT8_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//int8_t MB_Var_int8_Table[MB_PAR_INT8_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_UINT16_TABLE_DIM " + MB_PAR_UINT16_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_UINT16_TABLE_DIM != 0)
                        {
                            DummyString = "_UINT16_t MB_Par_uint16_Table[MB_PAR_UINT16_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_UINT16_t MB_Par_uint16_Table[MB_PAR_UINT16_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_INT16_TABLE_DIM " + MB_PAR_INT16_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_INT16_TABLE_DIM != 0)
                        {
                            DummyString = "_INT16_t MB_Par_int16_Table[MB_PAR_INT16_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_INT16_t MB_Par_int16_Table[MB_PAR_INT16_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_UINT32_TABLE_DIM " + MB_PAR_UINT32_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_UINT32_TABLE_DIM != 0)
                        {
                            DummyString = "_UINT32_t MB_Par_uint32_Table[MB_PAR_UINT32_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_UINT32_t MB_Par_uint32_Table[MB_PAR_UINT32_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_PLACE_H_TABLE_DIM " + MB_PAR_PLACE_H_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_PLACE_H_TABLE_DIM != 0)
                        {
                            DummyString = "_PLACE_H MB_Par_place_h_Table[MB_PAR_PLACE_H_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_PLACE_H MB_Par_place_h_Table[MB_PAR_PLACE_H_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_FLOAT_TABLE_DIM " + MB_PAR_FLOAT_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_FLOAT_TABLE_DIM != 0)
                        {
                            DummyString = "_FLOAT_t MB_Par_float_Table[MB_PAR_FLOAT_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_FLOAT_t MB_Par_float_Table[MB_PAR_FLOAT_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_DOUBLE_TABLE_DIM " + MB_PAR_DOUBLE_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_DOUBLE_TABLE_DIM != 0)
                        {
                            DummyString = "_DOUBLE MB_Par_double_Table[MB_PAR_DOUBLE_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_DOUBLE MB_Par_double_Table[MB_PAR_DOUBLE_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_STRING_TABLE_DIM " + MB_PAR_STRING_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_STRING_TABLE_DIM != 0)
                        {
                            DummyString = "_string MB_Par_string_Table[MB_PAR_STRING_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_string MB_Par_string_Table[MB_PAR_STRING_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "#define MB_PAR_DATA_TABLE_DIM " + MB_PAR_DATA_TABLE_DIM.ToString();
                        writer.WriteLine(DummyString);

                        if (MB_PAR_DATA_TABLE_DIM != 0)
                        {
                            DummyString = "_DATA MB_Par_data_Table[MB_PAR_DATA_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }
                        else
                        {
                            DummyString = "//_DATA MB_Par_data_Table[MB_PAR_DATA_TABLE_DIM];";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "const char* MB_Parameters_Names[MB_PARS_TABLE_SIZE] =";
                        writer.WriteLine(DummyString);

                        DummyString = "{";
                        writer.WriteLine(DummyString);

                        DummyString = "     {\"EE_KEY\"},";
                        writer.WriteLine(DummyString);

                        foreach (ParameterView _par in DeviceParametersView)
                        {
                            DummyString = "     {\"" + _par.Name + "\"},";
                            writer.WriteLine(DummyString);
                        }

                        DummyString = "};";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        MB_PAR_UINT8_TABLE_DIM = 0;
                        MB_PAR_ENUM_TABLE_DIM = 0;
                        MB_PAR_INT8_TABLE_DIM = 0;
                        MB_PAR_UINT16_TABLE_DIM = 1;  // Nell'indice 0 c'è EE_KEY
                        MB_PAR_INT16_TABLE_DIM = 0;
                        MB_PAR_UINT32_TABLE_DIM = 0;
                        MB_PAR_INT32_TABLE_DIM = 0;
                        MB_PAR_PLACE_H_TABLE_DIM = 0;
                        MB_PAR_FLOAT_TABLE_DIM = 0;
                        MB_PAR_DOUBLE_TABLE_DIM = 0;
                        MB_PAR_STRING_TABLE_DIM = 0;
                        MB_PAR_DATA_TABLE_DIM = 0;

                        DummyString = "void MB_Parameters_Init( void )";
                        writer.WriteLine(DummyString);

                        DummyString = "{";
                        writer.WriteLine(DummyString);

                        DummyString = "    MB_Parameters.EE_KEY = &MB_Par_uint16_Table[0];";
                        writer.WriteLine(DummyString);

                        foreach (ParameterView _par in DeviceParametersView)
                        {
                            PrintParameterInit(_par, writer);
                        }

                        DummyString = "}";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "";
                        writer.WriteLine(DummyString);

                        DummyString = "struct MB_Parameters";
                        writer.WriteLine(DummyString);

                        DummyString = "{";
                        writer.WriteLine(DummyString);

                        DummyString = "    _UINT16_t* EE_KEY;";
                        writer.WriteLine(DummyString);

                        foreach (ParameterView _par in DeviceParametersView)
                        {
                            Print_MB_Parameters(_par, writer);
                        }

                        DummyString = "}MB_Parameters;";
                        writer.WriteLine(DummyString);

                        writer.Close();

                        MessageBox.Show("File creato correttamente", "Salvataggio File CSV", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Errore Salvataggio File CSV", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PrintParameter(ParameterView _par, StreamWriter writer)
        {
            FillParameterInfo(_par);

            string DummyString = "    },";
            writer.WriteLine(DummyString);

            DummyString = "    {";
            writer.WriteLine(DummyString);

            DummyString = "    // " + ParName + " = " + ParFileID;
            writer.WriteLine(DummyString);

            DummyString = "       " + ParName + "_TYPE,";
            writer.WriteLine(DummyString);

            switch (_par.Info.DataType)
            {
                case TargetDataType.TYPE_UC:
                    DummyString = "        " + MB_PAR_UINT8_TABLE_DIM.ToString() + " //MB_Par_uint8_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT8_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_US:
                    DummyString = "        " + MB_PAR_UINT16_TABLE_DIM.ToString() + " //MB_Par_uint16_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT16_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_SS:
                    DummyString = "        " + MB_PAR_INT16_TABLE_DIM.ToString() + " //MB_Par_int16_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_INT16_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_UL:
                    DummyString = "        " + MB_PAR_UINT32_TABLE_DIM.ToString() + " //MB_Par_uint32_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT32_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_SL:
                    DummyString = "        " + MB_PAR_INT32_TABLE_DIM.ToString() + " //MB_Par_int32_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_INT32_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_FL:
                    DummyString = "        " + MB_PAR_FLOAT_TABLE_DIM.ToString() + " //MB_Par_float_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_FLOAT_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_DB:
                    DummyString = "        " + MB_PAR_DOUBLE_TABLE_DIM.ToString() + " //MB_Par_double_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_DOUBLE_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_STR:
                    DummyString = "        " + MB_PAR_STRING_TABLE_DIM.ToString() + " //MB_Par_string_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_STRING_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_DATA:
                    DummyString = "        " + MB_PAR_DATA_TABLE_DIM.ToString() + " //MB_Par_data_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_DATA_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_PLACE_HOLDER:
                    DummyString = "        " + MB_PAR_PLACE_H_TABLE_DIM.ToString() + " //MB_Par_place_h_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_PLACE_H_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_ENUM:
                    DummyString = "        " + MB_PAR_ENUM_TABLE_DIM.ToString() + " //MB_Par_enum_Table";
                    writer.WriteLine(DummyString);
                    MB_PAR_ENUM_TABLE_DIM++;
                    break;
            }
        }

        private void PrintParameterInit(ParameterView _par, StreamWriter writer)
        {
            string DummyString;

            switch (_par.Info.DataType)
            {
                case TargetDataType.TYPE_UC:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_uint8_Table[" + MB_PAR_UINT8_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT8_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_US:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_uint16_Table[" + MB_PAR_UINT16_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT16_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_SS:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_int16_Table[" + MB_PAR_INT16_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_INT16_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_UL:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_uint32_Table[" + MB_PAR_UINT32_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_UINT32_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_SL:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_int32_Table[" + MB_PAR_INT32_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_INT32_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_FL:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_float_Table[" + MB_PAR_FLOAT_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_FLOAT_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_DB:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_double_Table[" + MB_PAR_DOUBLE_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_DOUBLE_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_STR:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_string_Table[" + MB_PAR_STRING_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_STRING_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_DATA:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_data_Table[" + MB_PAR_DATA_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_DATA_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_PLACE_HOLDER:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_place_h_Table[" + MB_PAR_PLACE_H_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_PLACE_H_TABLE_DIM++;
                    break;
                case TargetDataType.TYPE_ENUM:
                    DummyString = "    MB_Parameters." + _par.Name + " = &MB_Par_enum_Table[" + MB_PAR_ENUM_TABLE_DIM.ToString() + "];";
                    writer.WriteLine(DummyString);
                    MB_PAR_ENUM_TABLE_DIM++;
                    break;
            }
        }

        private void Print_MB_Parameters(ParameterView _par, StreamWriter writer)
        {
            string DummyString;

            switch (_par.Info.DataType)
            {
                case TargetDataType.TYPE_UC:
                    DummyString = "    _uint8_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_US:
                    DummyString = "    _UINT16_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_SS:
                    DummyString = "    _INT16_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_UL:
                    DummyString = "    _UINT32_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_SL:
                    DummyString = "    _INT32_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_FL:
                    DummyString = "    _FLOAT_t* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_DB:
                    DummyString = "    _DOUBLE* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_STR:
                    DummyString = "    _string* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_DATA:
                    DummyString = "    _DATA* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_PLACE_HOLDER:
                    DummyString = "    _PLACE_H* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
                case TargetDataType.TYPE_ENUM:
                    DummyString = "    _ENUM* " + _par.Name + ";";
                    writer.WriteLine(DummyString);
                    break;
            }
        }

        private ICommand _openXMLFileCmd;
        public ICommand OpenXMLFileCmd
        {
            get
            {
                if (_openXMLFileCmd == null)
                {
                    _openXMLFileCmd = new RelayCommand(
                        param => this.OpenXMLFile()
                    );
                }
                return _openXMLFileCmd;
            }
        }

        private void OpenXMLFile()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.DefaultExt = ".xml";
            openDlg.Filter = "XML Data File (*.xml)|*.xml|All files (*.*)|*.*";

            if (openDlg.ShowDialog() == true)
            {
                XmlFileName = Path.GetFileName(openDlg.FileName);

                DeviceParametersView.Clear();
                DeviceParametersView = DeviceParameters.LoadXmlFile(openDlg.FileName);
                DeviceParametersFilterExec();

                DeviceParameterList = DeviceParameters.DeviceParameterList;

                MemoryModel DatabaseMemoryModel = DeviceParameters.GetDeviceMemoryModel(openDlg.FileName);

                DeviceEEpromBundlesView.Clear();

                if (DatabaseMemoryModel.Pages.Count != 0)
                {
                    foreach (PageModel page in DatabaseMemoryModel.Pages)
                    {
                        if ((page.Description != null) && (page.Description.Length > 0))
                        {
                            DeviceEEpromBundlesView.Add(page);
                        }
                    }
                }
                else if (DatabaseMemoryModel.Pages_MUT7000.Count != 0)
                {
                    foreach (PageModel page in DatabaseMemoryModel.Pages_MUT7000)
                    {
                        if ((page.Description != null) && (page.Description.Length > 0))
                        {
                            DeviceEEpromBundlesView.Add(page);
                        }
                    }
                }

                if (openDlg.FileName.Contains("MC406"))
                    DeviceParameters.RunningKey = Parameters.RunningKeys.MC406_Key;

                if (openDlg.FileName.Contains("MC7000"))
                    DeviceParameters.RunningKey = Parameters.RunningKeys.MC7000_Key;

                if (openDlg.FileName.Contains("MC808"))
                    DeviceParameters.RunningKey = Parameters.RunningKeys.MC808_Key;

                ControlsEnable = true;
            }
        }

        public void ClearInfo()
        {
            ParFileID = 0;
            ParName = "";
            ParDescription = "";
            ParDataType = TargetDataType.TYPE_UC;
            ParDefault = "";
            ParMax = "";
            ParMin = "";
            ParEEPRomId = 0;
            ParProtocolId = 0;
            ParOptionsListVisibility = Visibility.Collapsed;
            ParOptionsList.Clear();
        }

        public void FillParameterInfo(ParameterView _selectedPar)
        {
            ParFileID = _selectedPar.Index;
            ParName = _selectedPar.Name;
            ParDescription = _selectedPar.Description;
            ParDataType = _selectedPar.Info.DataType;
            ParDefault = _selectedPar.Default;
            ParMax = _selectedPar.Max;
            ParMin = _selectedPar.Min;

            ParOptionsListVisibility = Visibility.Collapsed;
            ParOptionsList.Clear();

            int ParIndex = ParFileID - 1;

            TargetDataType ParType = DeviceParameters.DeviceParameterList[ParIndex].DataType;

            try
            {
                switch (ParType)
                {
                    case TargetDataType.TYPE_UC:
                        TypeByte NewBytePar = (TypeByte)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewBytePar.MemID;
                        ParProtocolId = NewBytePar.ProtocolID;
                        break;
                    case TargetDataType.TYPE_ENUM:
                        TypeEnum NewEnumPar = (TypeEnum)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewEnumPar.MemID;
                        ParProtocolId = NewEnumPar.ProtocolID;
                        ParOptionsListVisibility = Visibility.Visible;
                        ParOptionsList.Clear();
                        if (NewEnumPar.OptionList != null)
                        {
                            foreach (var kvp in NewEnumPar.OptionList)
                            {
                                ParOptionsList.Add(new OptionView
                                {
                                    Value = kvp.Key.ToString(),
                                    Description = kvp.Value
                                });
                            }
                        }
                        break;
                    case TargetDataType.TYPE_US:
                        TypeUint16 NewUint16Par = (TypeUint16)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewUint16Par.MemID;
                        ParProtocolId = NewUint16Par.ProtocolID;
                        break;
                    case TargetDataType.TYPE_FL:
                        TypeFloat NewFloatPar = (TypeFloat)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewFloatPar.MemID;
                        ParProtocolId = NewFloatPar.ProtocolID;
                        break;
                    case TargetDataType.TYPE_DB:
                        TypeDouble NewDoublePar = (TypeDouble)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewDoublePar.MemID;
                        ParProtocolId = NewDoublePar.ProtocolID;
                        break;
                    case TargetDataType.TYPE_SS:
                        TypeInt16 NewInt16Par = (TypeInt16)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewInt16Par.MemID;
                        ParProtocolId = NewInt16Par.ProtocolID;
                        break;
                    case TargetDataType.TYPE_STR:
                        TypeString NewStringPar = (TypeString)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewStringPar.MemID;
                        ParProtocolId = NewStringPar.ProtocolID;
                        break;
                    case TargetDataType.TYPE_UL:
                        TypeUint32 NewUint32Par = (TypeUint32)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewUint32Par.MemID;
                        ParProtocolId = NewUint32Par.ProtocolID;
                        break;
                    case TargetDataType.TYPE_PLACE_HOLDER:
                        TypePlaceHolder NewPlaceHolderPar = (TypePlaceHolder)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewPlaceHolderPar.MemID;
                        ParProtocolId = NewPlaceHolderPar.ProtocolID;
                        break;
                    case TargetDataType.TYPE_DATA:
                        TypeData NewDataPar = (TypeData)DeviceParameters.DeviceParameterList[ParIndex];
                        ParEEPRomId = NewDataPar.MemID;
                        ParProtocolId = NewDataPar.ProtocolID;
                        break;
                }
            }
            catch
            {
                ParEEPRomId = 0;
                ParProtocolId = 0;
            }

            SelectedPar = _selectedPar;
        }

        public void FillGSMParameterInfo(IGSMvariable gsmParam)
        {
            ParGsmID = gsmParam.ID;
            ParGsmName = gsmParam.Name;
            ParGsmDataType = gsmParam.DataType;
            ParGsmAddress = (uint)(gsmParam.Address);
            SelectedGsmPar = gsmParam;
        }

        private IGSMvariable _selectedGsmPar;
        public IGSMvariable SelectedGsmPar
        {
            get { return _selectedGsmPar; }
            set
            {
                if (value != _selectedGsmPar)
                {
                    _selectedGsmPar = value;
                    OnPropertyChanged("SelectedGsmPar");
                }
            }
        }

        private uint _parGsmID;
        public uint ParGsmID
        {
            get { return _parGsmID; }
            set
            {
                if (value != _parGsmID)
                {
                    _parGsmID = value;
                    OnPropertyChanged("ParGsmID");
                }
            }
        }

        private string _parGsmName;
        public string ParGsmName
        {
            get { return _parGsmName; }
            set
            {
                if (value != _parGsmName)
                {
                    _parGsmName = value;
                    OnPropertyChanged("ParGsmName");
                }
            }
        }

        private string _parGsmValue;
        public string ParGsmValue
        {
            get { return _parGsmValue; }
            set
            {
                if (value != _parGsmValue)
                {
                    _parGsmValue = value;
                    OnPropertyChanged("ParGsmValue");
                }
            }
        }

        private TargetDataType _parGsmDataType;
        public TargetDataType ParGsmDataType
        {
            get { return _parGsmDataType; }
            set
            {
                if (value != _parGsmDataType)
                {
                    _parGsmDataType = value;
                    OnPropertyChanged("ParGsmDataType");
                }
            }
        }


        private uint _parGsmAddress;
        public uint ParGsmAddress
        {
            get { return _parGsmAddress; }
            set
            {
                if (value != _parGsmAddress)
                {
                    _parGsmAddress = value;
                    OnPropertyChanged("ParGsmAddress");
                }
            }
        }


        private ICommand _readGsmParCmd;
        public ICommand ReadGsmParCmd
        {
            get
            {
                if (_readGsmParCmd == null)
                {
                    _readGsmParCmd = new RelayCommand(
                        param => this.ReadGsmPar()
                    );
                }
                return _readGsmParCmd;
            }
        }

        private void ReadGsmPar()
        {
            if (ParGsmName != string.Empty)
            {
                WriteFrameDataVisibility = Visibility.Collapsed;
                ReadFrameDataVisibility = Visibility.Visible;

                CommandFrame = string.Empty;

                if (GsmParamSelected)
                    CommandFrame = GetReadGsmParCommandFrame(SelectedGsmPar);
                else
                {
                    GsmTestWriteSelected = false;
                    CommandFrame = GetReadGsmVarCommandFrame(SelectedGsmPar);
                }

                ViewCommandFrame(CommandFrame);
            }
            else
                ClearCommandFrame();
        }

        public string GetReadGsmParCommandFrame(IGSMvariable par)
        {
            ReadGSMPar ReadCmd = new ReadGSMPar();
            ICommunicationFrame ReadComFrame = null;
            string ReadFrame = null;

            if (par != null)
            {
                ReadCmd.Variable = par;
                ReadComFrame = ReadCmd.GetCommandFrame();
                ReadFrame = ReadComFrame.getFrameStr();
            }
            return ReadFrame;
        }

        public string GetReadGsmVarCommandFrame(IGSMvariable par)
        {
            ReadGSMTest ReadCmd = new ReadGSMTest();
            ICommunicationFrame ReadComFrame = null;
            string ReadFrame = null;

            if (par != null)
            {
                ReadCmd.Variable = par;
                ReadComFrame = ReadCmd.GetCommandFrame();
                ReadFrame = ReadComFrame.getFrameStr();
            }
            return ReadFrame;
        }


        private ICommand _readParCmd;
        public ICommand ReadParCmd
        {
            get
            {
                if (_readParCmd == null)
                {
                    _readParCmd = new RelayCommand(
                        param => this.ReadPar()
                    );
                }
                return _readParCmd;
            }
        }

        void ReadPar()
        {
            if (ParDescription != string.Empty)
            {
                WriteFrameDataVisibility = Visibility.Collapsed;
                ReadFrameDataVisibility = Visibility.Visible;

                CommandFrame = string.Empty;
                CommandFrame = GetReadCommandFrame(SelectedPar);

                ViewCommandFrame(CommandFrame);
            }
            else
                ClearCommandFrame();
        }

        private void ViewCommandFrame(string CommandFrame)
        {
            if (CommandFrame != null)
            {
                CommandFrameList = GetFrameBytesList(CommandFrame);

                CommandFrameHex = "";
                foreach (string HexByte in CommandFrameList)
                {
                    CommandFrameHex += "0x";
                    CommandFrameHex += HexByte;
                    CommandFrameHex += " ";
                }

                Key = CommandFrameList[0];

                DeviceKey = (Parameters.RunningKeys)(Convert.ToByte(Key, 16));

                HeaderLenght = CommandFrameList[1];
                FrameTypeStr = CommandFrameList[2];

                byte Frame = Convert.ToByte(CommandFrameList[2], 16);
                FrameTypeCode = (FrameType)Frame;

                DataType = CommandFrameList[3];
                byte Type = Convert.ToByte(CommandFrameList[3], 16);
                FrameDataType = (TargetDataType)Type;

                DataLenght = CommandFrameList[5] + CommandFrameList[4];
                DataLenghtValue = Convert.ToUInt16(DataLenght, 16);

                Address = CommandFrameList[9] + CommandFrameList[8] + CommandFrameList[7] + CommandFrameList[6];
                AddressVal = Convert.ToUInt32(Address, 16);

                Crc = CommandFrameList[CommandFrameList.Count - 1] + CommandFrameList[CommandFrameList.Count - 2];
            }
        }

        public void ClearCommandFrame()
        {
            CommandFrameHex = "";
            Key = "";
            DeviceKey = RunningKeys.MC608_Key;
            HeaderLenght = "";
            FrameTypeStr = "";
            FrameTypeCode = FrameType.F_UNKNOWN;
            DataType = "";
            DataLenght = "";
            Address = "";
            AddressVal = 0;
            Crc = "";
        }

        public string GetReadCommandFrame(ParameterView par)
        {
            ITargetVariable targetVariable = par.Info;
            ReadEEPROM ReadCmd = new ReadEEPROM();
            ICommunicationFrame ReadComFrame = null;
            string ReadFrame = null;

            if (targetVariable != null)
            {
                ReadCmd.Variable = targetVariable;
                ReadComFrame = ReadCmd.GetCommandFrame();
                ReadFrame = ReadComFrame.getFrameStr();
            }
            return ReadFrame;
        }

        public string GetBundleCommandFrame(uint _bundleID)
        {
            ReadVarBundle ReadCmd = new ReadVarBundle();
            ICommunicationFrame ReadComFrame = null;
            string ReadFrame = null;

            ReadCmd.BundleId = _bundleID;
            ReadComFrame = ReadCmd.GetCommandFrame();
            ReadFrame = ReadComFrame.getFrameStr();

            return ReadFrame;
        }


        private ICommand _enterGSMConfigurationModeCmd;
        public ICommand EnterGSMConfigurationModeCmd
        {
            get
            {
                if (_enterGSMConfigurationModeCmd == null)
                {
                    _enterGSMConfigurationModeCmd = new RelayCommand(
                        param => this.EnterGSMConfigurationMode()
                    );
                }
                return _enterGSMConfigurationModeCmd;
            }
        }

        private ICommand _exitGSMConfigurationModeCmd;
        public ICommand ExitGSMConfigurationModeCmd
        {
            get
            {
                if (_exitGSMConfigurationModeCmd == null)
                {
                    _exitGSMConfigurationModeCmd = new RelayCommand(
                        param => this.ExitGSMConfigurationMode()
                    );
                }
                return _exitGSMConfigurationModeCmd;
            }
        }

        private BoardOperation OperationToSend;

        private void ExitGSMConfigurationMode()
        {
            Operation gsmDebugMode = new Operation(portHandler);
            ICommunicationFrame gsmDebugModeFrame = null;
            string CommandFrame = null;

            gsmDebugMode.OperationCode = BoardOperation.operationExitGSMConfigurationMode;

            gsmDebugModeFrame = gsmDebugMode.GetCommandFrame();
            CommandFrame = gsmDebugModeFrame.getFrameStr();

            if (CommandFrame != null)
                ViewCommandFrame(CommandFrame);

            OperationToSend = gsmDebugMode.OperationCode;
        }

        private void EnterGSMConfigurationMode()
        {
            Operation gsmDebugMode = new Operation(portHandler);
            ICommunicationFrame gsmDebugModeFrame = null;
            string CommandFrame = null;

            gsmDebugMode.OperationCode = BoardOperation.operationEnterGSMConfigurationMode;

            gsmDebugModeFrame = gsmDebugMode.GetCommandFrame();
            CommandFrame = gsmDebugModeFrame.getFrameStr();

            if (CommandFrame != null)
                ViewCommandFrame(CommandFrame);

            OperationToSend = gsmDebugMode.OperationCode;
        }


        private ICommand _writeGsmParCmd;
        public ICommand WriteGsmParCmd
        {
            get
            {
                if (_writeGsmParCmd == null)
                {
                    _writeGsmParCmd = new RelayCommand(
                        param => this.WriteGsmPar()
                    );
                }
                return _writeGsmParCmd;
            }
        }

        private void WriteGsmPar()
        {
            if (ParGsmName != string.Empty)
            {
                if ((ParGsmValue != null) && (ParGsmValue.Length != 0))
                {
                    if (SelectedGsmPar.DataType != TargetDataType.TYPE_STR)
                    {
                        if (IsNumber(ParGsmValue) == false)
                        {
                            ClearCommandFrame();
                            MessageBox.Show("Inserisci un valore numerico", "Valore Errato", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    WriteFrameDataVisibility = Visibility.Visible;
                    ReadFrameDataVisibility = Visibility.Collapsed;

                    CommandFrame = string.Empty;
                    SelectedGsmPar.ValAsString = ParGsmValue;

                    if (GsmParamSelected)
                        CommandFrame = GetWriteGsmParCommandFrame(SelectedGsmPar);
                    else
                    {
                        GsmTestWriteSelected = true;
                        CommandFrame = GetWriteGsmVarCommandFrame(SelectedGsmPar);
                    }

                    if (CommandFrame != null)
                        ViewCommandFrame(CommandFrame);

                }
                else
                {
                    ClearCommandFrame();
                    MessageBox.Show("Inserisci un valore valido", "Valore Errato", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private WritableEnumTargetVariable GsmEnumPar;
        private WritableByteTargetVariable GsmBytePar;
        private WritableUInt16TargetVariable GsmUint16Par;
        private WritableFloatTargetVariable GsmFloatPar;
        private WritableDoubleTargetVariable GsmDoublePar;
        private WritableInt16TargetVariable GsmInt16Par;
        private WritableStringTargetVariable GsmStrPar;
        private WritableUInt32TargetVariable GsmUInt32Par;

        public string GetWriteGsmParCommandFrame(IGSMvariable par)
        {
            WriteGSMPar WriteCmd = new WriteGSMPar();
            ICommunicationFrame WriteComFrame = null;
            string WriteFrame = null;

            if (par != null)
            {
                switch (par.DataType)
                {
                    case TargetDataType.TYPE_ENUM:
                        GsmEnumPar = par as WritableEnumTargetVariable;
                        if ((GsmEnumPar != null) && (par.ValAsString != ""))
                        {
                            GsmEnumPar.Value = EnumTechUnitConverter(GsmEnumPar, par.ValAsString);
                            WriteCmd.Variable = EnumPar;
                        }
                        break;
                    case TargetDataType.TYPE_UC:
                        GsmBytePar = par as WritableByteTargetVariable;
                        if ((GsmBytePar != null) && (par.ValAsString != ""))
                        {
                            GsmBytePar.Value = Convert.ToByte(par.ValAsString);
                            WriteCmd.Variable = GsmBytePar;
                        }
                        break;
                    case TargetDataType.TYPE_US:
                        GsmUint16Par = par as WritableUInt16TargetVariable;
                        if ((GsmUint16Par != null) && (par.ValAsString != ""))
                        {
                            GsmUint16Par.Value = Convert.ToUInt16(par.ValAsString);
                            WriteCmd.Variable = GsmUint16Par;
                        }
                        break;
                    case TargetDataType.TYPE_FL:
                        GsmFloatPar = par as WritableFloatTargetVariable;
                        if ((GsmFloatPar != null) && (par.ValAsString != ""))
                        {
                            GsmFloatPar.Value = ParseFloatInvariant(par.ValAsString);
                            WriteCmd.Variable = FloatPar;
                        }
                        break;
                    case TargetDataType.TYPE_DB:
                        GsmDoublePar = par as WritableDoubleTargetVariable;
                        if ((GsmDoublePar != null) && (par.ValAsString != ""))
                        {
                            GsmDoublePar.Value = ParseDoubleInvariant(par.ValAsString);
                            WriteCmd.Variable = GsmDoublePar;
                        }
                        break;
                    case TargetDataType.TYPE_SS:
                        GsmInt16Par = par as WritableInt16TargetVariable;
                        if ((GsmInt16Par != null) && (par.ValAsString != ""))
                        {
                            GsmInt16Par.Value = Convert.ToInt16(par.ValAsString);
                            WriteCmd.Variable = GsmInt16Par;
                        }
                        break;
                    case TargetDataType.TYPE_STR:
                        GsmStrPar = par as WritableStringTargetVariable;
                        if ((GsmStrPar != null) && (par.ValAsString != null))
                        {
                            GsmStrPar.Value = ParGsmValue;
                            WriteCmd.Variable = GsmStrPar;
                        }
                        break;
                    case TargetDataType.TYPE_UL:
                        GsmUInt32Par = par as WritableUInt32TargetVariable;
                        if ((GsmUInt32Par != null) && (par.ValAsString != ""))
                        {
                            GsmUInt32Par.Value = Convert.ToUInt32(par.ValAsString);
                            WriteCmd.Variable = GsmUInt32Par;
                        }
                        break;
                    default:
                        return null;
                }

                WriteComFrame = WriteCmd.GetCommandFrame();
                WriteFrame = WriteComFrame.getFrameStr();
                return WriteFrame;
            }
            else
                return null;

        }

        public string GetWriteGsmVarCommandFrame(IGSMvariable par)
        {
            WriteGSMTest WriteCmd = new WriteGSMTest();
            ICommunicationFrame WriteComFrame = null;
            string WriteFrame = null;

            if (par != null)
            {
                switch (par.DataType)
                {
                    case TargetDataType.TYPE_ENUM:
                        GsmEnumPar = par as WritableEnumTargetVariable;
                        if ((GsmEnumPar != null) && (par.ValAsString != ""))
                        {
                            GsmEnumPar.Value = EnumTechUnitConverter(GsmEnumPar, par.ValAsString);
                            WriteCmd.Variable = EnumPar;
                        }
                        break;
                    case TargetDataType.TYPE_UC:
                        GsmBytePar = par as WritableByteTargetVariable;
                        if ((GsmBytePar != null) && (par.ValAsString != ""))
                        {
                            GsmBytePar.Value = Convert.ToByte(par.ValAsString);
                            WriteCmd.Variable = GsmBytePar;
                        }
                        break;
                    case TargetDataType.TYPE_US:
                        GsmUint16Par = par as WritableUInt16TargetVariable;
                        if ((GsmUint16Par != null) && (par.ValAsString != ""))
                        {
                            GsmUint16Par.Value = Convert.ToUInt16(par.ValAsString);
                            WriteCmd.Variable = GsmUint16Par;
                        }
                        break;
                    case TargetDataType.TYPE_FL:
                        GsmFloatPar = par as WritableFloatTargetVariable;
                        if ((GsmFloatPar != null) && (par.ValAsString != ""))
                        {
                            GsmFloatPar.Value = ParseFloatInvariant(par.ValAsString);
                            WriteCmd.Variable = FloatPar;
                        }
                        break;
                    case TargetDataType.TYPE_DB:
                        GsmDoublePar = par as WritableDoubleTargetVariable;
                        if ((GsmDoublePar != null) && (par.ValAsString != ""))
                        {
                            GsmDoublePar.Value = ParseDoubleInvariant(par.ValAsString);
                            WriteCmd.Variable = GsmDoublePar;
                        }
                        break;
                    case TargetDataType.TYPE_SS:
                        GsmInt16Par = par as WritableInt16TargetVariable;
                        if ((GsmInt16Par != null) && (par.ValAsString != ""))
                        {
                            GsmInt16Par.Value = Convert.ToInt16(par.ValAsString);
                            WriteCmd.Variable = GsmInt16Par;
                        }
                        break;
                    case TargetDataType.TYPE_STR:
                        GsmStrPar = par as WritableStringTargetVariable;
                        if ((GsmStrPar != null) && (par.ValAsString != null))
                        {
                            GsmStrPar.Value = ParGsmValue;
                            WriteCmd.Variable = GsmStrPar;
                        }
                        break;
                    case TargetDataType.TYPE_UL:
                        GsmUInt32Par = par as WritableUInt32TargetVariable;
                        if ((GsmUInt32Par != null) && (par.ValAsString != ""))
                        {
                            GsmUInt32Par.Value = Convert.ToUInt32(par.ValAsString);
                            WriteCmd.Variable = GsmUInt32Par;
                        }
                        break;
                    default:
                        return null;
                }

                WriteComFrame = WriteCmd.GetCommandFrame();
                WriteFrame = WriteComFrame.getFrameStr();
                return WriteFrame;
            }
            else
                return null;

        }

        public static bool IsNumber(string input)
        {
            return double.TryParse(
                input.Replace(',', '.'), // accetta sia virgola che punto
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out _);
        }

        private ICommand _writeParCmd;
        public ICommand WriteParCmd
        {
            get
            {
                if (_writeParCmd == null)
                {
                    _writeParCmd = new RelayCommand(
                        param => this.WritePar()
                    );
                }
                return _writeParCmd;
            }
        }

        void WritePar()
        {
            if (ParDescription != string.Empty)
            {
                if ((ParValue != null) && (ParValue.Length != 0))
                {
                    WriteFrameDataVisibility = Visibility.Visible;
                    ReadFrameDataVisibility = Visibility.Collapsed;

                    CommandFrame = string.Empty;

                    ITargetWritable Par = SelectedPar.Info;

                    if (Par.DataType != TargetDataType.TYPE_STR)
                    {
                        if (IsNumber(ParValue) == false)
                        {
                            ClearCommandFrame();
                            MessageBox.Show("Inserisci un valore numerico", "Valore Errato", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    SelectedPar.ValAsString = ParValue;
                    CommandFrame = GetWriteCommandFrame(Par);

                    ViewCommandFrame(CommandFrame);
                }
                else
                {
                    ClearCommandFrame();
                    MessageBox.Show("Inserisci un valore valido", "Valore Errato", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private TypeEnum EnumPar;
        private TypeByte BytePar;
        private TypeUint16 Uint16Par;
        private TypeFloat FloatPar;
        private TypeDouble DoublePar;
        private TypeInt16 Int16Par;
        private TypeString StrPar;
        private TypeUint32 UInt32Par;


        public string GetWriteCommandFrame(ITargetWritable targetWritable)
        {
            WriteEEPROM WriteCmd = new WriteEEPROM();
            ICommunicationFrame WriteComFrame = null;
            string WriteFrame = null;

            if (targetWritable != null)
            {
                switch (targetWritable.DataType)
                {
                    case TargetDataType.TYPE_ENUM:
                        EnumPar = targetWritable as Parameters.TypeEnum;
                        if ((EnumPar != null) && (ParValue != ""))
                        {
                            EnumPar.Value = EnumTechUnitConverter(EnumPar, ParValue);
                            WriteCmd.Variable = EnumPar;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_UC:
                        BytePar = targetWritable as Parameters.TypeByte;
                        if ((BytePar != null) && (ParValue != ""))
                        {
                            BytePar.Value = Convert.ToByte(ParValue);
                            WriteCmd.Variable = BytePar;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_US:
                        Uint16Par = targetWritable as Parameters.TypeUint16;
                        if ((Uint16Par != null) && (ParValue != ""))
                        {
                            Uint16Par.Value = Convert.ToUInt16(ParValue);
                            WriteCmd.Variable = Uint16Par;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_FL:
                        FloatPar = targetWritable as Parameters.TypeFloat;
                        if ((FloatPar != null) && (ParValue != ""))
                        {
                            FloatPar.Value = ParseFloatInvariant(ParValue);
                            WriteCmd.Variable = FloatPar;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_DB:
                        DoublePar = targetWritable as Parameters.TypeDouble;
                        if ((DoublePar != null) && (ParValue != ""))
                        {
                            DoublePar.Value = ParseDoubleInvariant(ParValue);
                            WriteCmd.Variable = DoublePar;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_SS:
                        Int16Par = targetWritable as Parameters.TypeInt16;
                        if ((Int16Par != null) && (ParValue != ""))
                        {
                            Int16Par.Value = Convert.ToInt16(ParValue);
                            WriteCmd.Variable = Int16Par;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_STR:
                        StrPar = targetWritable as Parameters.TypeString;
                        if ((StrPar != null) && (ParValue != null))
                        {
                            StrPar.Value = ParValue;
                            WriteCmd.Variable = StrPar;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    case TargetDataType.TYPE_UL:
                        UInt32Par = targetWritable as Parameters.TypeUint32;
                        if ((UInt32Par != null) && (ParValue != ""))
                        {
                            UInt32Par.Value = Convert.ToUInt32(ParValue);
                            WriteCmd.Variable = UInt32Par;
                            WriteComFrame = WriteCmd.GetCommandFrame();
                            WriteFrame = WriteComFrame.getFrameStr();
                        }
                        break;
                    default:
                        return null;
                }
                return WriteFrame;
            }
            else
                return null;
        }

        public static float ParseFloatInvariant(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            // Normalizza il separatore decimale
            string normalized = input.Replace(',', '.');

            // Usa InvariantCulture per accettare solo il punto come separatore
            if (float.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                return result;

            throw new FormatException($"Impossibile convertire '{input}' in float.");
        }

        public static double ParseDoubleInvariant(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            // Normalizza il separatore decimale
            string normalized = input.Replace(',', '.');

            // Usa InvariantCulture per accettare solo il punto come separatore
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                return result;

            throw new FormatException($"Impossibile convertire '{input}' in double.");
        }

        List<string> GetFrameBytesList(string _frame)
        {
            if (_frame != null)
            {
                return Enumerable.Range(0, (_frame.Length + 1) / 2)
                    .Select(i => _frame.Substring(i * 2, Math.Min(2, _frame.Length - i * 2))).ToList();
            }
            else
                return new List<string>();
        }
        public byte EnumTechUnitConverter(WritableEnumTargetVariable _enumPar, string ValAsString)
        {
            byte BackupValue;
            BackupValue = _enumPar.Value;
            int Count = _enumPar.Options.Count;
            for (int i = 0; i <= Count; i++)
            {
                try
                {
                    _enumPar.Value = (byte)i;
                    if (_enumPar.ValAsString.Equals(ValAsString))
                    {
                        _enumPar.Value = BackupValue;
                        return (byte)i;
                    }
                }
                catch { }
            }
            return 0;
        }

        private string _xmlFileName;
        public string XmlFileName
        {
            get { return _xmlFileName; }
            set
            {
                if (value != _xmlFileName)
                {
                    _xmlFileName = value;
                    OnPropertyChanged("XmlFileName");
                }
            }
        }

        private bool _controlsEnable;
        public bool ControlsEnable
        {
            get { return _controlsEnable; }
            set
            {
                if (value != _controlsEnable)
                {
                    _controlsEnable = value;
                    OnPropertyChanged("ControlsEnable");
                }
            }
        }


        private int _parFileID;
        public int ParFileID
        {
            get { return _parFileID; }
            set
            {
                if (value != _parFileID)
                {
                    _parFileID = value;
                    OnPropertyChanged("ParFileID");
                }
            }
        }

        private string _parName;
        public string ParName
        {
            get { return _parName; }
            set
            {
                if (value != _parName)
                {
                    _parName = value;
                    OnPropertyChanged("ParName");
                }
            }
        }

        private string _parDescription;
        public string ParDescription
        {
            get { return _parDescription; }
            set
            {
                if (value != _parDescription)
                {
                    _parDescription = value;
                    OnPropertyChanged("ParDescription");
                }
            }
        }


        private TargetDataType _parDataType;
        public TargetDataType ParDataType
        {
            get { return _parDataType; }
            set
            {
                if (value != _parDataType)
                {
                    _parDataType = value;
                    OnPropertyChanged("ParDataType");
                }
            }
        }

        private uint _parEEPRomId;
        public uint ParEEPRomId
        {
            get { return _parEEPRomId; }
            set
            {
                if (value != _parEEPRomId)
                {
                    _parEEPRomId = value;
                    OnPropertyChanged("ParEEPRomId");
                }
            }
        }

        private uint _parProtocolId;
        public uint ParProtocolId
        {
            get { return _parProtocolId; }
            set
            {
                if (value != _parProtocolId)
                {
                    _parProtocolId = value;
                    OnPropertyChanged("ParProtocolId");
                }
            }
        }

        private string _parDefault;
        public string ParDefault
        {
            get { return _parDefault; }
            set
            {
                if (value != _parDefault)
                {
                    _parDefault = value;
                    OnPropertyChanged("ParDefault");
                }
            }
        }

        private string _parMax;
        public string ParMax
        {
            get { return _parMax; }
            set
            {
                if (value != _parMax)
                {
                    _parMax = value;
                    OnPropertyChanged("ParMax");
                }
            }
        }

        private string _parMin;
        public string ParMin
        {
            get { return _parMin; }
            set
            {
                if (value != _parMin)
                {
                    _parMin = value;
                    OnPropertyChanged("ParMin");
                }
            }
        }

        private string _parValue;
        public string ParValue
        {
            get { return _parValue; }
            set
            {
                if (value != _parValue)
                {
                    _parValue = value;
                    OnPropertyChanged("ParValue");
                }
            }
        }

        private Visibility _parOptionsListVisibility;
        public Visibility ParOptionsListVisibility
        {
            get { return _parOptionsListVisibility; }
            set
            {
                if (value != _parOptionsListVisibility)
                {
                    _parOptionsListVisibility = value;
                    OnPropertyChanged("ParOptionsListVisibility");
                }
            }
        }

        private uint _bundleIndex;
        public uint BundleIndex
        {
            get { return _bundleIndex; }
            set
            {
                if (value != _bundleIndex)
                {
                    _bundleIndex = value;
                    OnPropertyChanged("BundleIndex");
                }
            }
        }

        private uint _bundleSpanStart;
        public uint BundleSpanStart
        {
            get { return _bundleSpanStart; }
            set
            {
                if (value != _bundleSpanStart)
                {
                    _bundleSpanStart = value;
                    OnPropertyChanged("BundleSpanStart");
                }
            }
        }

        private uint _bundleSpanStop;
        public uint BundleSpanStop
        {
            get { return _bundleSpanStop; }
            set
            {
                if (value != _bundleSpanStop)
                {
                    _bundleSpanStop = value;
                    OnPropertyChanged("BundleSpanStop");
                }
            }
        }

        private UInt32 _bundleRamIndex;
        public UInt32 BundleRamIndex
        {
            get { return _bundleRamIndex; }
            set
            {
                if (value != _bundleRamIndex)
                {
                    _bundleRamIndex = value;
                    OnPropertyChanged("BundleRamIndex");
                }
            }
        }

        public class OptionView
        {
            public string Value { get; set; }
            public string Description { get; set; }
        }

        private ObservableCollection<OptionView> _parOptionsList;
        public ObservableCollection<OptionView> ParOptionsList
        {
            get
            {
                if (_parOptionsList == null)
                    _parOptionsList = new ObservableCollection<OptionView>();
                return _parOptionsList;
            }
            set
            {
                if (value != _parOptionsList)
                {
                    _parOptionsList = value;
                    OnPropertyChanged("ParOptionsList");
                }
            }
        }

        private ParameterView _selectedPar;
        public ParameterView SelectedPar
        {
            get { return _selectedPar; }
            set
            {
                if (value != _selectedPar)
                {
                    _selectedPar = value;
                    OnPropertyChanged("SelectedPar");
                }
            }
        }

        private string _commandFrame;
        public string CommandFrame
        {
            get { return _commandFrame; }
            set
            {
                if (value != _commandFrame)
                {
                    _commandFrame = value;
                    OnPropertyChanged("CommandFrame");
                }
            }
        }

        private List<string> _commandFrameList;
        public List<string> CommandFrameList
        {
            get { return _commandFrameList; }
            set
            {
                if (value != _commandFrameList)
                {
                    _commandFrameList = value;
                    OnPropertyChanged("CommandFrameList");
                }
            }
        }



        private string _commandFrameHex;
        public string CommandFrameHex
        {
            get { return _commandFrameHex; }
            set
            {
                if (value != _commandFrameHex)
                {
                    _commandFrameHex = value;
                    OnPropertyChanged("CommandFrameHex");
                }
            }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set
            {
                if (value != _key)
                {
                    _key = value;
                    OnPropertyChanged("Key");
                }
            }
        }


        private Parameters.RunningKeys _deviceKey;
        public Parameters.RunningKeys DeviceKey
        {
            get { return _deviceKey; }
            set
            {
                if (value != _deviceKey)
                {
                    _deviceKey = value;
                    OnPropertyChanged("DeviceKey");
                }
            }
        }



        private string _headerLenght;
        public string HeaderLenght
        {
            get { return _headerLenght; }
            set
            {
                if (value != _key)
                {
                    _headerLenght = value;
                    OnPropertyChanged("HeaderLenght");
                }
            }
        }

        private string _frameTypeStr;
        public string FrameTypeStr
        {
            get { return _frameTypeStr; }
            set
            {
                if (value != _frameTypeStr)
                {
                    _frameTypeStr = value;
                    OnPropertyChanged("FrameTypeStr");
                }
            }
        }

        private FrameType _frameTypeCode;
        public FrameType FrameTypeCode
        {
            get { return _frameTypeCode; }
            set
            {
                if (value != _frameTypeCode)
                {
                    _frameTypeCode = value;
                    OnPropertyChanged("FrameTypeCode");
                }
            }
        }

        private TargetDataType _frameDataType;
        public TargetDataType FrameDataType
        {
            get { return _frameDataType; }
            set
            {
                if (value != _frameDataType)
                {
                    _frameDataType = value;
                    OnPropertyChanged("FrameDataType");
                }
            }
        }


        private string _dataType;
        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (value != _dataType)
                {
                    _dataType = value;
                    OnPropertyChanged("DataType");
                }
            }
        }

        private string _dataLenght;
        public string DataLenght
        {
            get { return _dataLenght; }
            set
            {
                if (value != _dataLenght)
                {
                    _dataLenght = value;
                    OnPropertyChanged("DataLenght");
                }
            }
        }

        private UInt16 _dataLenghtVal;
        public UInt16 DataLenghtVal
        {
            get { return _dataLenghtVal; }
            set
            {
                if (value != _dataLenghtVal)
                {
                    _dataLenghtVal = value;
                    OnPropertyChanged("DataLenghtVal");
                }
            }
        }

        private ushort _dataLenghtValue;
        public ushort DataLenghtValue
        {
            get { return _dataLenghtValue; }
            set
            {
                if (value != _dataLenghtValue)
                {
                    _dataLenghtValue = value;
                    OnPropertyChanged("DataLenghtValue");
                }
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (value != _address)
                {
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        private UInt32 _addressVal;
        public UInt32 AddressVal
        {
            get { return _addressVal; }
            set
            {
                if (value != _addressVal)
                {
                    _addressVal = value;
                    OnPropertyChanged("AddressVal");
                }
            }
        }

        private string _crc;
        public string Crc
        {
            get { return _crc; }
            set
            {
                if (value != _crc)
                {
                    _crc = value;
                    OnPropertyChanged("Crc");
                }
            }
        }


        private Visibility _writeFrameDataVisibility;
        public Visibility WriteFrameDataVisibility
        {
            get { return _writeFrameDataVisibility; }
            set
            {
                if (value != _writeFrameDataVisibility)
                {
                    _writeFrameDataVisibility = value;
                    OnPropertyChanged("WriteFrameDataVisibility");
                }
            }
        }

        private Visibility _readFrameDataVisibility;
        public Visibility ReadFrameDataVisibility
        {
            get { return _readFrameDataVisibility; }
            set
            {
                if (value != _readFrameDataVisibility)
                {
                    _readFrameDataVisibility = value;
                    OnPropertyChanged("ReadFrameDataVisibility");
                }
            }
        }

        private string _varName;
        public string VarName
        {
            get { return _varName; }
            set
            {
                if (value != _varName)
                {
                    _varName = value;
                    OnPropertyChanged("VarName");
                }
            }
        }

        private string _varAddress;
        public string VarAddress
        {
            get { return _varAddress; }
            set
            {
                if (value != _varAddress)
                {
                    _varAddress = value;
                    OnPropertyChanged("VarAddress");
                }
            }
        }

        private string _varType;
        public string VarType
        {
            get { return _varType; }
            set
            {
                if (value != _varType)
                {
                    _varType = value;
                    OnPropertyChanged("VarType");
                }
            }
        }

        private ITargetVariable _selectedVar;
        public ITargetVariable SelectedVar
        {
            get { return _selectedVar; }
            set
            {
                if (value != _selectedVar)
                {
                    _selectedVar = value;
                    OnPropertyChanged("SelectedVar");
                }
            }
        }

        public void FillVariableInfo(ITargetVariable _selectedVar)
        {
            VarName = _selectedVar.Name;


            IRAMvariable RamVariable = _selectedVar as IRAMvariable;
            if (RamVariable != null)
            {
                uint addres = EnumToUInt<RAMAddresses>(RamVariable.Address);
                VarAddress = addres.ToString();
            }

            VarType = _selectedVar.DataType.ToString();

            SelectedVar = _selectedVar;
        }

        public static uint EnumToUInt<TEnum>(TEnum value) where TEnum : Enum
        {
            return Convert.ToUInt32(value);
        }

        private ICommand _readVarCmd;
        public ICommand ReadVarCmd
        {
            get
            {
                if (_readVarCmd == null)
                {
                    _readVarCmd = new RelayCommand(
                        param => this.ReadVar()
                    );
                }
                return _readVarCmd;
            }
        }

        void ReadVar()
        {
            if (VarName != string.Empty)
            {
                WriteFrameDataVisibility = Visibility.Collapsed;
                ReadFrameDataVisibility = Visibility.Visible;

                CommandFrame = string.Empty;
                CommandFrame = GetReadVarCommandFrame(SelectedVar);

                ViewCommandFrame(CommandFrame);
            }
            else
                ClearCommandFrame();
        }

        public string GetReadVarCommandFrame(ITargetVariable var)
        {
            ReadRAM ReadCmd = new ReadRAM();
            ICommunicationFrame ReadComFrame = null;
            string ReadFrame = null;

            if (var != null)
            {
                ReadCmd.Variable = var;
                ReadComFrame = ReadCmd.GetCommandFrame();
                ReadFrame = ReadComFrame.getFrameStr();
            }
            return ReadFrame;
        }

        public void FillBundleInfo(PageModel page)
        {
            BundleIndex = page.Index;
            BundleSpanStart = page.Span.Start;
            BundleSpanStop = page.Span.Stop;

            DeviceEEpromBundlesListView.Clear();
            foreach (ParameterView _par in DeviceParametersView)
            {
                FillParameterInfo(_par);

                if (ParEEPRomId != 0)
                {
                    if ((ParEEPRomId >= page.Span.Start) && (ParEEPRomId < page.Span.Stop))
                    {
                        DeviceEEpromBundlesListView.Add(_par);
                    }
                }
            }

            if (DeviceEEpromBundlesListView.Count != 0)
            {
                string CommandFrame = GetBundleCommandFrame(page.Index);
                ViewCommandFrame(CommandFrame);
            }
            else
                ClearCommandFrame();
        }

        public void FillBundleRamInfo(RAMPageModel RamPage)
        {
            BundleIndex = RamPage.Index;

            DeviceRamBundlesListView.Clear();
            RAMVariableModel RamVariable = new RAMVariableModel();

            switch (RamPage.Index)
            {
                case ReadVarBundle.RAM_MEASURE_BUNDLE:
                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Filtered flow rate [m/s]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Filtered flow rate [current units]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Filtered flow rate [full scale %]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Coils' current [mA]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Total positive flowrate accumulator [m^3]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Total negative flowrate accumulator [m^3]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Partial positive flowrate accumulator [m^3]";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Partial negative flowrate accumulator [m^3]";
                    DeviceRamBundlesListView.Add(RamVariable);
                    break;

                case ReadVarBundle.RAM_OTHERS_BUNDLE:
                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Events log count";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Data log's rows number";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Firmware version number";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Firmware revision number";
                    DeviceRamBundlesListView.Add(RamVariable);
                    break;

                case ReadVarBundle.RAM_BUNDLE_T1_T2_PRESS:
                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Temperature Probe 1";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Temperature Probe 2";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Pressure Probe";
                    DeviceRamBundlesListView.Add(RamVariable);
                    break;

                case ReadVarBundle.RAM_BUNDLE_BT_RS485:
                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Bluetooth/Modbus board firmware version number";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Bluetooth/Modbus board firmware revision number";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Bluetooth/Modbus board hardware revision number";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Bluetooth/Modbus board status";
                    DeviceRamBundlesListView.Add(RamVariable);

                    RamVariable = new RAMVariableModel();
                    RamVariable.Description = "Bluetooth/Modbus Board Serial Number";
                    DeviceRamBundlesListView.Add(RamVariable);
                    break;

            }

            string CommandFrame = GetBundleCommandFrame(RamPage.Index);
            ViewCommandFrame(CommandFrame);
        }


        public Parameters DeviceParameters
        {
            get
            {
                return Parameters.Instance;
            }
        }
    }
}
