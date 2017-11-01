//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.MockJmsProvider.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.MockJmsProvider
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using Bemagine.ServiceModel.Jms;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Mock implementation of a JMS destination that implements the IDestination interface.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal sealed class MockDestination : IDestination
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        public string Name { get; private set; }

        //----------------------------------------------------------------------------------------//
        // construction
        //----------------------------------------------------------------------------------------//

        internal MockDestination(string destinationName)
        {
            Name = destinationName;
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
