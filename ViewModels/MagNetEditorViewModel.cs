using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalConnect.Memory;

namespace EuromagProtocolUtility
{
    public class MagNetEditorViewModel : ObservableObject
    {
        public MagNetEditorViewModel() 
        { 

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
