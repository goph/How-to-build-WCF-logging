using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Service
{
   
    
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            try
            {
                int x = 0;
                x = 1 / x;
                return string.Format("You entered: {0}", value);
            }
            catch (Exception ex)
            {
                FaultReason reason = new FaultReason(ex.Message);
                throw (new FaultException(reason));
            }
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
