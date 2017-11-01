//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.Tests.
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.Tests
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.Threading;
    using System.Diagnostics;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Extension methods for System.Diagnostics.Debug.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class DebugEx
    {
        public static void WriteLine(string format, params object[] args)
        {
            string customMessage = String.Format(format, args);
            
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);

            string debugMessage = 
                String.Format(
                    "{0} Thread [{1,2}] [{2,-64}] -- {3}",
                    DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"),
                    Thread.CurrentThread.ManagedThreadId,
                    
                    String.Format("{0}.{1}", 
                        stackFrame.GetMethod().DeclaringType.Name,
                        stackFrame.GetMethod().Name),

                    customMessage);

            Debug.WriteLine(debugMessage);                
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
