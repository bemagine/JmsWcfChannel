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
    // using aliases
    //--------------------------------------------------------------------------------------------//

    using BPN = BindingPropertyNames;
    using BPD = BindingPropertyDefaults;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The abstract base BindingElementExtensionElement implementation that enables the use of 
    /// the JmsTransportBindingElement implementation from a machine or application configuration 
    /// file. 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public abstract class JmsTransportBindingElementExtension 
        : BindingElementExtensionElement, IJmsTransportContext
    {
        //----------------------------------------------------------------------------------------//
        // abstract interfaces
        //----------------------------------------------------------------------------------------//

        public abstract string Scheme { get; }

        //----------------------------------------------------------------------------------------//
        // BindingElementExtensionElement overrides
        //----------------------------------------------------------------------------------------//

        public override Type BindingElementType
        {
            get { return typeof(JmsTransportBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
          var bindingElement = new JmsTransportBindingElement(Scheme);
          ApplyConfiguration(bindingElement);
          return bindingElement;
        }

        //----------------------------------------------------------------------------------------//
        // ApplyConfiguration()
        //----------------------------------------------------------------------------------------//
        
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);

            JmsTransportContext element = (JmsTransportBindingElement) bindingElement;
            element.CopyFrom(this);
        }

        //----------------------------------------------------------------------------------------//
        // InitializeFrom()
        //----------------------------------------------------------------------------------------//

        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);

            JmsTransportContext element = (JmsTransportBindingElement) bindingElement;
            element.CopyFrom(this);
        }        

        //----------------------------------------------------------------------------------------//
        // CopyFrom()
        //----------------------------------------------------------------------------------------//

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            CopyFrom(from as IJmsTransportContext);
        }

        public void CopyFrom(IJmsTransportContext from)
        {
            JmsTransportContext.CopyFrom(this, from);
        }

        //----------------------------------------------------------------------------------------//
        // configuration properties
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        // User name and password
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.UserName, DefaultValue=BPD.UserName)]
        public string UserName 
        { 
            get { return (string) base[BPN.UserName];  }
            set { base[BPN.UserName] = value;  }
        }

        [ConfigurationProperty(BPN.Password, DefaultValue=BPD.Password)]
        public string Password 
        { 
            get { return (string) base[BPN.Password];  }
            set { base[BPN.Password] = value;  }
        }  
    
        //----------------------------------------------------------------------------------------//
        // PersistentDeliveryRequired
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(
            BPN.PersistentDeliveryRequired, 
            DefaultValue=BPD.PersistentDeliveryRequired)
        ]
        public bool PersistentDeliveryRequired 
        { 
            get { return (bool) base[BPN.PersistentDeliveryRequired];  }
            set { base[BPN.PersistentDeliveryRequired] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // MaxBufferPoolSize
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.MaxBufferPoolSize, DefaultValue=BPD.MaxBufferPoolSize)]
        [LongValidator(MinValue=BPD.MaxBufferPoolSizeMinimum)]
        public long MaxBufferPoolSize 
        { 
            get { return (long) base[BPN.MaxBufferPoolSize];  }
            set { base[BPN.MaxBufferPoolSize] = value;  }
        }            

        //----------------------------------------------------------------------------------------//
        // MaxReceivedMessageSize
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.MaxReceivedMessageSize, DefaultValue=BPD.MaxReceivedMessageSize)]
        [LongValidator(MinValue=BPD.MaxReceivedMessageSizeMinimum)]
        public long MaxReceivedMessageSize 
        { 
            get { return (long) base[BPN.MaxReceivedMessageSize];  }
            set { base[BPN.MaxReceivedMessageSize] = value;  }
        }

        //----------------------------------------------------------------------------------------//
        // MessageSizeCompressionThreshold
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.MessageSizeCompressionThreshold, DefaultValue=null)]
        public long MessageSizeCompressionThreshold 
        { 
            get 
            { 
                var messageSize = (long?) base[BPN.MessageSizeCompressionThreshold];  
                return (messageSize.HasValue) ? messageSize.Value : MaxReceivedMessageSize;
            }

            set { base[BPN.MessageSizeCompressionThreshold] = new Nullable<long>(value); }
        }

        //----------------------------------------------------------------------------------------//
        // ExplicitResponseDestinationName : See IQueueTransport properties for details.
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.ExplicitResponseQueueName, DefaultValue="")]
        public string ExplicitResponseDestinationName 
        { 
            get { return (string) base[BPN.ExplicitResponseQueueName]; } 
            set { base[BPN.ExplicitResponseQueueName] = value; }
        }
 
        //----------------------------------------------------------------------------------------//
        // JmsConnectionFactoryType property
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(BPN.JmsConnectionFactoryType)]
        public string JmsConnectionFactoryType 
        { 
            get { return (string) base[BPN.JmsConnectionFactoryType]; } 
            set { base[BPN.JmsConnectionFactoryType] = value; } 
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Configures instances of the JmsTransportBindingElement with a URI scheme that binds the
    /// transport to a JMS queue.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class JmsQueueTransportBindingElementExtension :
        JmsTransportBindingElementExtension
    {
        public override string  Scheme
        {
	        get { return "net.jmsq"; }
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Configures instances of the JmsTransportBindingElement with a URI scheme that binds the
    /// transport to a JMS topic.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class JmsTopicTransportBindingElementExtension :
        JmsTransportBindingElementExtension
    {
        public override string  Scheme
        {
	        get { return "net.jmst"; }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
