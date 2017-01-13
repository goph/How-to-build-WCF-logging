using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.Runtime.Serialization;

namespace WcfLogManager
{
    [DataContract]
    public class ApplicationFault
    {
        [DataMember]
        public string TestMessage { get; set; }
    }


    public static class Constants
    {
        public const string FaultAction = "Describe your fault action here";
    }
}
