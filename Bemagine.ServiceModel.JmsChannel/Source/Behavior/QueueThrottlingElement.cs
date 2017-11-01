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
    using System.ServiceModel.Configuration;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The behavior extension element representing the queue throttling behavior configuration
    /// element.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class QueueThrottlingElement : BehaviorExtensionElement 
    {
    //--------------------------------------------------------------------------------------------//
    // data members and member properties
    //--------------------------------------------------------------------------------------------//

        private const string MaxConcurrentEndpointCallsPropertyName =
            "maxConcurrentEndpointCalls";

    //--------------------------------------------------------------------------------------------//
    // BehaviorExtentionElement overrides
    //--------------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Calls the base CopyFrom() method and copies QueueThrottlingElement elements from the
        /// source to the destination instances of QueueThrottlingElement.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
 	        base.CopyFrom(from);
            var element = (QueueThrottlingElement) from;
            MaxConcurrentEndpointCalls = element.MaxConcurrentEndpointCalls;
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a new instance of the QueueThrottlingBehavior;
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override object CreateBehavior()
        {
            var behavior = 
                new QueueThrottlingBehavior 
                { 
                    MaxConcurrentEndpointCalls = MaxConcurrentEndpointCalls
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
            get { return typeof(QueueThrottlingBehavior); }
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Gets / sets the maximum number of concurrent calls executing for a given endpoint. 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [ConfigurationProperty(MaxConcurrentEndpointCallsPropertyName, DefaultValue=32)]
        [IntegerValidator(MinValue=1)]
        public int MaxConcurrentEndpointCalls 
        { 
            get { return (int) base[MaxConcurrentEndpointCallsPropertyName]; } 
            set { base[MaxConcurrentEndpointCallsPropertyName] = value; }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
