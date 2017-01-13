using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using System.Diagnostics;

using System.ServiceModel.Configuration;

namespace WcfLogManager
{
    public class ServiceErrorHandler : ServiceErrorHandlerBehaviorExtensionElement, IErrorHandler, IServiceBehavior
    {
        #region IErrorHandler Members
        private ServiceDescription _ServiceDescription;
        public bool HandleError(Exception error)
        {
            // Commun mechanism to log all services errors
            StringBuilder errorMessage = new StringBuilder(1024);
            errorMessage.Append(string.Format("Application : {0} \n", _ServiceDescription.Name));
            errorMessage.Append("--------------------\n");
            errorMessage.Append(string.Format("An error has been occured on application {0}",_ServiceDescription.ConfigurationName));
            
            errorMessage.Append("\n The occured error is :");
            errorMessage.Append(string.Concat("Message : ", error.Message, "\n"));
            errorMessage.Append(string.Concat("Source : ", error.Source, "\n"));
            errorMessage.Append(string.Concat("StackTrace : ", error.StackTrace, "\n"));

            // TO DO ==>  Log your errors here

            return true;  // error has been handled.
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, 
            ref System.ServiceModel.Channels.Message fault)
        {
            if (!(error is FaultException))
            {
                MessageFault msgFault = null;

                ApplicationFault faultDetail = new ApplicationFault();
                faultDetail.TestMessage = error.Message;
                FaultException<ApplicationFault> fex =
                    new FaultException<ApplicationFault>(faultDetail, error.StackTrace,
                        new FaultCode("ApplicationFault"));
                msgFault = fex.CreateMessageFault();

                fault = Message.CreateMessage(version, msgFault, Constants.FaultAction);
            }
        }
        #endregion



        #region IServiceBehavior Members
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, 
            System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, 
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            return;
        }
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispacher in serviceHostBase.ChannelDispatchers)
            {
                channelDispacher.ErrorHandlers.Add(this);
            }
        }
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            _ServiceDescription = serviceDescription;
            foreach (var ServiceEndpoint in serviceDescription.Endpoints)
            {
                if (ServiceEndpoint.Contract.Name != "IMetadataExchange")
                {
                    foreach (var opDesc in ServiceEndpoint.Contract.Operations)
                    {
                        if (opDesc.Faults.Count == 0)
                        {
                            string msg =  string.Format("ServiceErrorHandlerBehavior requires a FaultContract(typeof(ApplicationFault))"+
                                "on each operation contract. The {0} contains no FaultContracts.", opDesc.Name);
                            throw new InvalidOperationException(msg);
                        }

                        var fcExists = from fc in opDesc.Faults where fc.DetailType == typeof(ApplicationFault) select fc;
                        if (fcExists.Count() == 0)
                        {
                            string msg = string.Format("ServiceErrorHandlerBehavior requires a FaultContract(typeof(ApplicationFault))"+
                                "on each operation contract.");
                            throw new InvalidOperationException(msg);
                        }
                    }
                }
            }
        }
        #endregion
    }




    public class ServiceErrorHandlerBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ServiceErrorHandler); }
        }

        protected override object CreateBehavior()
        {
            return new ServiceErrorHandler();
        }

    }
}



