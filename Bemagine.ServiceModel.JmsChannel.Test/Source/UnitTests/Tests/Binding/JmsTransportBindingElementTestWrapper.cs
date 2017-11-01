//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Test.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Test
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System.ServiceModel.Channels;
    using Bemagine.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The JmsTransportBindingElement does not quite fit the mold for consitent testing of binding
    /// elements and extention elements from a IJmsTransportContext perspective. This class acts as 
    /// a proxy for testing JmsTransportBindingElement from the IJmsTransportContext perspective.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class JmsTransportBindingElementTestWrapper 
        : BindingElement, IJmsTransportContext
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private readonly JmsTransportBindingElement _bindingElement; 

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public JmsTransportBindingElementTestWrapper()
        {
             _bindingElement = new JmsTransportBindingElement("net.jmsq"); 
        }

        private JmsTransportBindingElementTestWrapper(BindingElement clone)
        {
            _bindingElement = (JmsTransportBindingElement) clone;
        }

        //----------------------------------------------------------------------------------------//
        // BindingElement interfaces
        //----------------------------------------------------------------------------------------//

        public override BindingElement Clone()
        {
            return new JmsTransportBindingElementTestWrapper(_bindingElement.Clone());
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return _bindingElement.GetProperty<T>(context);
        }

        public void CopyFrom(IJmsTransportContext rhs)
        {
            ((JmsTransportContext) _bindingElement).CopyFrom(rhs);
        }

        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public long MaxBufferPoolSize
        {
            get { return ((JmsTransportContext) _bindingElement).MaxBufferPoolSize;  }
            set { ((JmsTransportContext) _bindingElement).MaxBufferPoolSize = value; }
        }

        public long MaxReceivedMessageSize
        {
            get { return ((JmsTransportContext) _bindingElement).MaxReceivedMessageSize;  }
            set { ((JmsTransportContext) _bindingElement).MaxReceivedMessageSize = value;  }
        }

        public long MessageSizeCompressionThreshold
        {
            get { return ((JmsTransportContext)_bindingElement).MessageSizeCompressionThreshold; }
            set { ((JmsTransportContext)_bindingElement).MessageSizeCompressionThreshold = value; }
        }

        public bool PersistentDeliveryRequired
        {
            get { return ((JmsTransportContext)_bindingElement).PersistentDeliveryRequired; }
            set { ((JmsTransportContext)_bindingElement).PersistentDeliveryRequired = value; }
        }

        public string JmsConnectionFactoryType
        {
            get { return ((JmsTransportContext)_bindingElement).JmsConnectionFactoryType; }
            set { ((JmsTransportContext)_bindingElement).JmsConnectionFactoryType = value; }
        }

        public string UserName
        {
            get { return ((JmsTransportContext)_bindingElement).UserName; }
            set { ((JmsTransportContext)_bindingElement).UserName = value; }
        }

        public string Password
        {
            get { return ((JmsTransportContext)_bindingElement).Password; }
            set { ((JmsTransportContext)_bindingElement).Password = value; }
        }

        public string ExplicitResponseDestinationName
        {
            get { return ((JmsTransportContext)_bindingElement).ExplicitResponseDestinationName; }
            set { ((JmsTransportContext)_bindingElement).ExplicitResponseDestinationName = value; }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
