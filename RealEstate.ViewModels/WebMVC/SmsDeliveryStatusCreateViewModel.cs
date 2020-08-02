using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.ViewModels.WebMVC
{
    public class SmsDeliveryStatusCreateViewModel
    {
        public int SID { get; set; }
        public int DLR { get; set; }
        public long TO { get; set; }
        public string FROM { get; set; }
        public long TS { get; set; }
        public string SMSID { get; set; }
        public string Voicecom_ID { get; set; }
    }
}
