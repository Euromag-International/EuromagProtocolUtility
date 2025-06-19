using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EuromagProtocolUtility
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UniversalConnect.Models;

    public enum BoardDataType
    {
        TYPE_UC = 0,
        TYPE_US = 1,
        TYPE_SS = 2,
        TYPE_UL = 3,
        TYPE_SL = 4,
        TYPE_FL = 5,
        TYPE_DB = 6,
        TYPE_STR = 7,
        TYPE_DATA = 8,
        TYPE_PLACE_HOLDER = 9,
        TYPE_ENUM = 10
    };

    [Flags]
    public enum ExecutionMaskId : uint
    {
        none = 0,
        autoset_ExcitationStabTime = 0x00000001U,
        autoset_DecimatorFlowrate = 0x00000002U,
        autoset_AutosetAwakeExcTime = 0x00000004U,

        exec_GeneralMeasureSamplingSetup = 0x00010000U,
        exec_GeneralMeasureConversionSetup = 0x00020000U,
        exec_ExcitationSet = 0x00040000U,
        exec_PipeWaterDetectionSetup = 0x00080000U,
        exec_InitDampingBuffer = 0x00100000U,
        exec_SetPulses = 0x00200000U,
        exec_MeasureConditioningSetup = 0x00400000U,
        exec_SetBatteryEnergyParameters = 0x00800000U,
        exec_SetInternalTemperatureOffset = 0x01000000U,
        exec_SetVisualizationNdec = 0x02000000U,
        exec_GeneralTempPressureProbeSetup = 0x04000000U,
        exec_SetLoggerOperation = 0x08000000U,
        exec_MenuGetPassword_Timeout = 0x10000000U
    };

    [Serializable()]
    public class VariableModel : IDataErrorInfo
    {
        public const uint INVALID_ID = uint.MaxValue;

        private static ReadOnlyDictionary<BoardDataType, uint> _fixedSizeVarSizes;
        public static ReadOnlyDictionary<BoardDataType, uint> FixedSizeVarSizes
        {
            get
            {
                if (_fixedSizeVarSizes == null)
                {
                    Dictionary<BoardDataType, uint> tmp = new Dictionary<BoardDataType, uint>();
                    tmp.Add(BoardDataType.TYPE_UC, 1);
                    tmp.Add(BoardDataType.TYPE_ENUM, 1);
                    tmp.Add(BoardDataType.TYPE_US, 2);
                    tmp.Add(BoardDataType.TYPE_SS, 2);
                    tmp.Add(BoardDataType.TYPE_FL, 4);
                    tmp.Add(BoardDataType.TYPE_SL, 4);
                    tmp.Add(BoardDataType.TYPE_UL, 4);
                    tmp.Add(BoardDataType.TYPE_DB, 8);
                    _fixedSizeVarSizes = new ReadOnlyDictionary<BoardDataType, uint>(tmp);
                }
                return _fixedSizeVarSizes;
            }

        }

        public VariableModel()
        {
            Type = BoardDataType.TYPE_UC;
            Size = 1;
            ID = INVALID_ID;
        }

        private uint _id;
        public uint ID
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("ID");
            }
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private String _description;
        public String Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private UInt32 _address;
        public UInt32 Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
                OnPropertyChanged("Span");
            }
        }

        private uint _externalId;
        public uint ExternalId
        {
            get { return _externalId; }
            set
            {
                _externalId = value;
                OnPropertyChanged("ExternalId");
            }
        }

        //private ExecutionMaskId _executionMask;
        //public ExecutionMaskId ExecutionMask
        //{
        //    get { return _executionMask; }
        //    set
        //    {
        //        _executionMask = value;
        //        OnPropertyChanged("ExecutionMask");
        //    }
        //}

        private UInt32 _executionMask;
        public UInt32 ExecutionMask
        {
            get { return _executionMask; }
            set
            {
                _executionMask = value;
                OnPropertyChanged("ExecutionMask");
            }
        }

        private BoardDataType _type;
        public BoardDataType Type
        {
            get { return _type; }
            set
            {
                _type = value;

                if (FixedSizeVarSizes.ContainsKey(_type))
                {
                    Size = FixedSizeVarSizes[_type];
                    IsSizeFixed = true;
                }
                else
                    IsSizeFixed = false;

                OnPropertyChanged("Type");
            }
        }

        private Boolean _isSizeFixed;
        public Boolean IsSizeFixed
        {
            get { return _isSizeFixed; }
            set
            {
                if (_isSizeFixed != value)
                {
                    _isSizeFixed = value;
                    OnPropertyChanged("IsSizeFixed");
                }
            }
        }

        private uint _size;
        public uint Size
        {
            get { return _size; }
            set
            {
                if (FixedSizeVarSizes.ContainsKey(_type))
                    _size = FixedSizeVarSizes[_type];
                else
                    _size = value;
                OnPropertyChanged("Size");
                OnPropertyChanged("Span");
            }
        }

        private String _value;
        public String Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private String _default;
        public String Default
        {
            get { return _default; }
            set
            {
                _default = value;
                OnPropertyChanged("Default");
            }
        }

        private String _minimum;
        public String Minimum
        {
            get { return _minimum; }
            set
            {
                _minimum = value;
                OnPropertyChanged("Minimum");
            }
        }

        private String _maximum;
        public String Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                OnPropertyChanged("Maximum");
            }
        }

        public AddressSpan Span
        {
            get
            {
                return new AddressSpan(Address, Address + (uint)Size - 1);
            }
        }

        private ObservableCollection<OptionModel> _optionsSet;
        public ObservableCollection<OptionModel> OptionsSet
        {
            get
            {
                if (_optionsSet == null)
                    _optionsSet = new ObservableCollection<OptionModel>();
                return _optionsSet;
            }
        }

        #region IDataErrorInfo & Validation Members

        #region Validation Delegate

        public delegate string ValidationDelegate(
            object sender, string propertyName);

        private List<ValidationDelegate> _validationDelegates = new List<ValidationDelegate>();

        public void AddValidationDelegate(ValidationDelegate func)
        {
            _validationDelegates.Add(func);
        }

        public void RemoveValidationDelegate(ValidationDelegate func)
        {
            if (_validationDelegates.Contains(func))
                _validationDelegates.Remove(func);
        }

        #endregion // Validation Delegate

        #region IDataErrorInfo for binding errors

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        public string GetValidationError(string propertyName)
        {
            string s = null;

            foreach (var func in _validationDelegates)
            {
                s = func(this, propertyName);
                if (s != null)
                    return s;
            }

            return s;
        }

        #endregion // IDataErrorInfo for binding errors

        #endregion // IDataErrorInfo & Validation Members

        #region ObservableObject

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }
}
