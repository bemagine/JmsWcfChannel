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
//------------------------------------------------------------------------------------------------//
// !!!IMPORTANT!!!
//
// Adapted from Aaron Skonard's blog "WCF's NetDataContractSerializer".
//
// http://www.pluralsight-training.net/community/blogs/aaron/archive/2006/04/21/22284.aspx
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Behavior
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Xml;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;

    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Description;
    using System.ServiceModel.Configuration;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// A static utility class for registering the NetDataContractSerializerOperationBehavior for
    /// a given operation.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class NetDataContractFormatUtility
    {
        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Registers the NetDataContractSerializerOperationBehavior with the operation description
        /// specified. The registration process involves replacing the currently set behavior,
        /// DataContractSerializerOperationBehavior.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="maxItemsInObjectGraph"></param>
        /// <param name="ignoreExtensionDataObject"></param>
        /// <param name="validate"></param>
        //----------------------------------------------------------------------------------------//

        public static void RegisterContract(OperationDescription description,
            int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool validate)
        {
            var ndcsOperationBehavior =
                description.Behaviors.Find<NetDataContractSerializerOperationBehavior>();

            //------------------------------------------------------------------------------------//
            // If NetDataContractSerializerOperationBehavior has already been applied and the 
            // set the validate flag to true and exception is generated. The validate flag is
            // generally set to true when calling RegisterContract from a behaviors Validate()
            // method. Generally, the Validate() methods must call RegisterContract() with the
            // validate flag set. This is because there are a number of ways to configure a
            // OperationContract to use the NetDataContractSerializer. Namely, users can decorate
            // the OperationContracts with the [NetDataContractFormat] attribute or configure
            // as service or endpoint behaviors. The issue that arises is which configuration
            // to choose. Therefore, we generate an exception. 
            //
            // The behavioral Validate() method is the first opportunity to verify if the 
            // behaviors have been configured properly. Once verified, subsequent verification
            // is not required.
            //------------------------------------------------------------------------------------//

            if ((ndcsOperationBehavior != null) && (validate))
            {
                throw new InvalidOperationException(
                    "Registering the NetDataContractSerializerOperationBehavior multiple times "+
                    "is not permitted. You may have attempted to register this behavior at the "+
                    "Operation, Service, or Endpoint level.");
            }

            //------------------------------------------------------------------------------------//
            // Here we ensure that the NetDataContractSerializerOperationBehavior has been applied
            // to the operation.
            //------------------------------------------------------------------------------------//

            var dcsOperationBehavior =
                description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if ((dcsOperationBehavior != null) && (ndcsOperationBehavior == null))
            {
                int index = description.Behaviors.IndexOf(dcsOperationBehavior);
                description.Behaviors.Remove(dcsOperationBehavior);
                
                description.Behaviors.Insert(index, 
                    new NetDataContractSerializerOperationBehavior(
                        description, maxItemsInObjectGraph, ignoreExtensionDataObject));
            }
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The NetDataContractFormat is an operation behavior that configures the WCF ServiceModel
    /// layer to serialize message bodies using the NetDataContractSerializer. This serializer 
    /// ensures the object graph reference integrity that is serialized.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public class NetDataContractFormat : Attribute, IOperationBehavior
    {
        private readonly int _maxItemsInObjectGraph = int.MaxValue;
        private readonly bool _ignoreExtensionDataObject;

        public NetDataContractFormat()
        {
        }

        public NetDataContractFormat(int maxItemsInObjectGraph)
        {
            _maxItemsInObjectGraph = maxItemsInObjectGraph;
        }

        public NetDataContractFormat(int maxItemsInObjectGraph, bool ignoreExtensionDataObject)
        {
            _maxItemsInObjectGraph = maxItemsInObjectGraph;
            _ignoreExtensionDataObject = ignoreExtensionDataObject;
        }

        public void AddBindingParameters(OperationDescription description,
            BindingParameterCollection parameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            NetDataContractFormatUtility.RegisterContract(
                description, _maxItemsInObjectGraph, _ignoreExtensionDataObject, false);
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            NetDataContractFormatUtility.RegisterContract(
                description, _maxItemsInObjectGraph, _ignoreExtensionDataObject, false);
        }

        public void Validate(OperationDescription description)
        {
            NetDataContractFormatUtility.RegisterContract(
                description, _maxItemsInObjectGraph, _ignoreExtensionDataObject, true);
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The WCF configuration element that represents the NetDataContractSerializerBehavior.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class NetDataContractSerializerElement : BehaviorExtensionElement
    {
        //----------------------------------------------------------------------------------------//
        // BehaviorExtentionElement overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Calls the base CopyFrom() method and copies QueueThrottlingElement elements from the
        /// source to the destination instances of QueueThrottlingElement.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
 	        base.CopyFrom(from);
            var element = (NetDataContractSerializerElement) from;
            MaxItemsInObjectGraph = element.MaxItemsInObjectGraph;
            IgnoreExtensionDataObject = element.IgnoreExtensionDataObject;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a new instance of the QueueThrottlingBehavior;
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override object CreateBehavior()
        {
            var behavior = 
                new NetDataContractSerializerBehavior 
                { 
                    MaxItemsInObjectGraph = MaxItemsInObjectGraph,
                    IgnoreExtensionDataObject = IgnoreExtensionDataObject
                };

            return behavior;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the type of QueueThrottlingBehavior.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override Type BehaviorType
        {
            get { return typeof(NetDataContractSerializerBehavior); }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum number of items in the serialized object graph.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty("maxItemsInObjectGraph", DefaultValue=65535)]
        [IntegerValidator(MinValue=65535)]
        public int MaxItemsInObjectGraph
        { 
            get { return (int) base["maxItemsInObjectGraph"]; } 
            set { base["maxItemsInObjectGraph"] = value; }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets a boolean value that specifies whether data specified by an extension of a
        /// type (and therefore not in the data contract) is ignored.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty("ignoreExtensionDataObject", DefaultValue=false)]
        public bool IgnoreExtensionDataObject
        { 
            get { return (bool) base["ignoreExtensionDataObject"]; } 
            set { base["ignoreExtensionDataObject"] = value; }
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The NetDataContractSerializerBehavior registers the NetDataContractSerializerOperationBehavior
    /// with all endpoint operations. This may be configured as service or endpoint behavior, but
    /// not both.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class NetDataContractSerializerBehavior : IServiceBehavior, IEndpointBehavior
    {
        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public int MaxItemsInObjectGraph { get; set; }
        public bool IgnoreExtensionDataObject { get; set; } 

        //----------------------------------------------------------------------------------------//
        // IServiceBehavior interface implementation
        //----------------------------------------------------------------------------------------//

        public void Validate(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
                RegisterContract(endpoint, true);
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        { 
            foreach (var endpoint in serviceDescription.Endpoints)
                RegisterContract(endpoint, false);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
        }
        
        //----------------------------------------------------------------------------------------//
        // IEndpointBehavior interface implementation
        //----------------------------------------------------------------------------------------//

        public void Validate(ServiceEndpoint endpoint)
        { 
            RegisterContract(endpoint, true);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, 
            BindingParameterCollection bindingParameters)
        {            
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, 
            EndpointDispatcher endpointDispatcher)
        {
            RegisterContract(endpoint, false);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        { 
        } 
        
        //----------------------------------------------------------------------------------------//
        // private interface
        //----------------------------------------------------------------------------------------//

        private void RegisterContract(ServiceEndpoint endpoint, bool validate)
        {
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                NetDataContractFormatUtility.RegisterContract(
                    description, MaxItemsInObjectGraph, IgnoreExtensionDataObject, validate);
            }
        }
    }

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// NetDataContractSerializerOperationBehavior creates an instance of the serializer
    /// (NetDataContractSerializer) used to serialize the WCF message body.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public class NetDataContractSerializerOperationBehavior 
        : DataContractSerializerOperationBehavior
    {
        //----------------------------------------------------------------------------------------//
        // member properties
        //----------------------------------------------------------------------------------------//

        public FormatterAssemblyStyle AssemblyFormat { get; set; }
        private StreamingContextStates AllowableContextStates { get; set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription) 
            : base(operationDescription) 
        {
            MaxItemsInObjectGraph = int.MaxValue;
            IgnoreExtensionDataObject = false;
            AssemblyFormat = FormatterAssemblyStyle.Full;

            InitializeStreamingContextStates();
        }

        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription,
            int maxItemsInObjectGraph) : base(operationDescription)
        {
            MaxItemsInObjectGraph = maxItemsInObjectGraph;
            IgnoreExtensionDataObject = false;
            AssemblyFormat = FormatterAssemblyStyle.Full;

            InitializeStreamingContextStates();
        }

        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription,
            int maxItemsInObjectGraph, bool ignoreExtensionDataObject) : base(operationDescription)
        {
            MaxItemsInObjectGraph = maxItemsInObjectGraph;
            IgnoreExtensionDataObject = ignoreExtensionDataObject;
            AssemblyFormat = FormatterAssemblyStyle.Full;

            InitializeStreamingContextStates();
        }

        private void InitializeStreamingContextStates()
        {
            AllowableContextStates = StreamingContextStates.All;
        }

        //----------------------------------------------------------------------------------------//
        // DataContractSerializerOperationBehavior overrides
        //----------------------------------------------------------------------------------------//

        public override XmlObjectSerializer CreateSerializer(Type type, string rootName, 
            string rootNamespace, IList<Type> knownTypes)
        {
            var ndcs = 
                new NetDataContractSerializer(
                    rootName, 
                    rootNamespace, 
                    new StreamingContext(AllowableContextStates), 
                    MaxItemsInObjectGraph,
                    IgnoreExtensionDataObject, 
                    AssemblyFormat, 
                    null);

            return ndcs;
        } 

        public override XmlObjectSerializer CreateSerializer(Type type, 
            XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IList<Type> knownTypes)
        {
            var ndcs = 
                new NetDataContractSerializer(
                    rootName, 
                    rootNamespace, 
                    new StreamingContext(AllowableContextStates), 
                    MaxItemsInObjectGraph,
                    IgnoreExtensionDataObject, 
                    AssemblyFormat, 
                    null);

            return ndcs;
        }
    }    
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
