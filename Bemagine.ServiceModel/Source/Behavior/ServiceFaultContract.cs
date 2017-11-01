//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Behavior
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The ServiceFaultContract is an implementation of IContractBehavior that sets a fault
    /// contract to all non-one-way operation contracts supported by the Service contract this
    /// attribute is applied.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class ServiceFaultContract : Attribute, IContractBehavior
    {
        //----------------------------------------------------------------------------------------//
        // IContractBehavior Members
        //----------------------------------------------------------------------------------------//

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, 
            BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, 
            ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, 
            DispatchRuntime dispatch)
        {           
        }

        public void Validate(ContractDescription description, ServiceEndpoint endpoint)
        {
            foreach (OperationDescription operationDescription in description.Operations)
            {
                if (!operationDescription.IsOneWay)
                    ApplyFaultContract(operationDescription);
            }
        }

        //----------------------------------------------------------------------------------------//
        // private interfaces
        //----------------------------------------------------------------------------------------//

        private void ApplyFaultContract(OperationDescription description)
        {
            string action = Action ?? DetailType.Name;

            var faultDescription = 
                new FaultDescription(action)
                {
                    DetailType = DetailType, Name = DetailType.Name
                };

            description.Faults.Add(faultDescription);
        }
   
        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        private Type DetailType { get; set; }
        private string Action { get; set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public ServiceFaultContract(Type detailType)
        {
            if (detailType == null)
                throw new ArgumentNullException("detailType");

            DetailType = detailType;
        }

        public ServiceFaultContract(Type detailType, string action)
        {
            if (detailType == null)
                throw new ArgumentNullException("detailType");

            if (detailType == null)
                throw new ArgumentNullException("action");

            DetailType = detailType;
            Action = action;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
