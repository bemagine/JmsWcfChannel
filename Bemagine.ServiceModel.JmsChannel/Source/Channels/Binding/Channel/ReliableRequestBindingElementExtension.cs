//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Channels
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The BindingElementExtentionElement that configures the behaviors of the 
    /// ReliableRequestChannel.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class ReliableRequestBindingElementExtension 
        : BindingElementExtensionElement, IReliableRequestContext
    {
        //----------------------------------------------------------------------------------------//
        // BindingElementExtensionElement overrides
        //----------------------------------------------------------------------------------------//

        public override Type BindingElementType
        {
            get { return typeof(ReliableRequestBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            return new ReliableRequestBindingElement();
        }

        //----------------------------------------------------------------------------------------//
        // ApplyConfiguration()
        //----------------------------------------------------------------------------------------//

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);

            var element = (ReliableRequestBindingElement) bindingElement;
            element.CopyFrom(this);
        }

        //----------------------------------------------------------------------------------------//
        // InitializeFrom()
        //----------------------------------------------------------------------------------------//

        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);

            var element = (IReliableRequestContext) bindingElement;
            element.CopyFrom(this);
        }

        //----------------------------------------------------------------------------------------//
        // CopyFrom()
        //----------------------------------------------------------------------------------------//

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            CopyFrom(from as IReliableRequestContext);
        }

        public void CopyFrom(IReliableRequestContext from)
        {
            this.CopyFromEx(from);
        }

        //----------------------------------------------------------------------------------------//
        // IReliableRequestContext properties
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty("heartbeartPeriodicity", DefaultValue = 8000)]
        [IntegerValidator(MinValue = 2000)]
        public int HeartbeatPeriodicity
        {
            get { return (int) base["heartbeartPeriodicity"]; }
            set { base["heartbeartPeriodicity"] = value; }
        }  
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
