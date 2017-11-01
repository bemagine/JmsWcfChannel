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

    using NUnit.Framework;
    using Bemagine.ServiceModel.Channels;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [TestFixture]
    public sealed class ChannelManagerBaseExTests
    {
        //----------------------------------------------------------------------------------------//
        // tests
        //----------------------------------------------------------------------------------------//

        [Test]
        public void GetExistentPropertyFromChannelListenerAsChannelManagerBase()
        {
            Assert.IsNotNull( 
                (new MockChannelListener() as ChannelManagerBase).GetProperty<ExistentChannelProperty>()
            );
        }

        [Test]
        public void GetNonExistentPropertyFromChannelListenerAsChannelManagerBase()
        {
            Assert.IsNull(
                (new MockChannelListener() as ChannelManagerBase).GetProperty<NonExistentChannelProperty>()
            );
        }

        [Test]
        public void GetExistentPropertyFromChannelFactoryAsChannelManagerBase()
        {
            Assert.IsNotNull(
                (new MockChannelFactory() as ChannelManagerBase).GetProperty<ExistentChannelProperty>()
            );
        }

        [Test]
        public void GetNonExistentPropertyFromChannelFactoryAsChannelManagerBase()
        {
            Assert.IsNull(
                (new MockChannelFactory() as ChannelManagerBase).GetProperty<NonExistentChannelProperty>()
            );
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
