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
    using System.ServiceModel.Configuration;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// The behavior extension element representing the async JMS operation context behavior 
    /// configuration element.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    public sealed class AsyncOperationElement : BehaviorExtensionElement 
    {
        //----------------------------------------------------------------------------------------//
        // BehaviorExtentionElement overrides
        //----------------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a new instance of the QueueThrottlingBehavior;
        /// </summary>
        //----------------------------------------------------------------------------------------//

        protected override object CreateBehavior()
        {
            return new AsyncEmsOperationBehavior();
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns the type of QueueThrottlingBehavior.
        /// </summary>
        //----------------------------------------------------------------------------------------//

        public override Type BehaviorType
        {
            get { return typeof(AsyncEmsOperationBehavior); }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
