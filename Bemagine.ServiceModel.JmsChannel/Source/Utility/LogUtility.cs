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

    using log4net;

    using System.Threading;
    using System.Reflection;
    using System.Diagnostics;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// Unified logging facility that encapsulates logging implemenation details.
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    internal static class LogUtility
    {
        //----------------------------------------------------------------------------------------//
        // data members
        //----------------------------------------------------------------------------------------//

        private static readonly ILog Logger = 
            LogManager.GetLogger("Bemagine.ServiceModel.JmsChannel");

        //----------------------------------------------------------------------------------------//
        // public interface
        //----------------------------------------------------------------------------------------//

        public static void Fatal(string formatString, params object[] details)
        {
            if (Logger.IsFatalEnabled)
                Logger.FatalFormat(formatString, details);
        }

        public static void Error(string formatString, params object[] details)
        {
            if (Logger.IsErrorEnabled)
                Logger.ErrorFormat(formatString, details);
        }

        public static void Warning(string formatString, params object[] details)
        {
            if (Logger.IsWarnEnabled)
                Logger.WarnFormat(formatString, details);
        }

        public static void Information(string formatString, params object[] details)
        {
            if (Logger.IsInfoEnabled)
                Logger.InfoFormat(formatString, details);
        }
        
        public static void Debug(string formatString, params object[] details)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat(formatString, details);
        }

        public static void Tracer()
        {
            var trace = new StackTrace();

            if (trace.FrameCount >= 2)
            {
                StackFrame frame = trace.GetFrame(1);
                MethodBase method = frame.GetMethod();

                Debug("TRACER [0] [{1}::{2}]", Thread.CurrentThread.ManagedThreadId,
                    method.DeclaringType.Name, method.Name);
            }
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
