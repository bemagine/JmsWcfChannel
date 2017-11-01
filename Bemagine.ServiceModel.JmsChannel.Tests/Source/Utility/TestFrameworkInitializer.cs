//------------------------------------------------------------------------------------------------//
//  The contents of this file are subject to the Mozilla Public License Version 1.1 
//  (the "License"); you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS" basis, WITHOUT 
//  WARRANTY OF ANY KIND, either express or implied. See the License for the specific 
//  language governing rights and limitations under the License.
//
//  The Original Code is Bemagine.ServiceModel.JmsChannel.Tests
//
//  The Initial Developer of the Original Code is Matthew Bologna, Bemagine.
//  Copyright (c) 2010-2012 Matthew Bologna, Bemagine. All rights reserved.
//------------------------------------------------------------------------------------------------//

namespace Bemagine.ServiceModel.JmsChannel.Tests
{
    //--------------------------------------------------------------------------------------------//
    // using directives
    //--------------------------------------------------------------------------------------------//

    using System;
    using System.IO;
    using System.Text;
    using System.Runtime.InteropServices;

    using Microsoft.Win32.SafeHandles;
    using NUnit.Framework;

    //--------------------------------------------------------------------------------------------//
    /// <summary>
    /// 
    /// </summary>
    //--------------------------------------------------------------------------------------------//

    [SetUpFixture]
    public sealed class TestFrameworkInitializer
    {
        //----------------------------------------------------------------------------------------//
        // constants
        //----------------------------------------------------------------------------------------//

        private const int StdOutHandleDescriptor = -11;
        private const int InternalCodePage = 437;

        //----------------------------------------------------------------------------------------//
        // DLL imports to support console logging during tests.
        //----------------------------------------------------------------------------------------//

        [DllImport(
            "kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)
        ]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(
            "kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)
        ]
        private static extern int AllocConsole();

        [DllImport(
            "kernel32.dll",
            EntryPoint = "FreeConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)
        ]
        private static extern bool FreeConsole();

        //----------------------------------------------------------------------------------------//
        // private properties
        //----------------------------------------------------------------------------------------//

        private static SafeFileHandle StandardOutputHandle { get; set; }
        private static FileStream StandardOutputStream { get; set; }
        private static Encoding StandardOutputEncoding { get; set; }
        private static StreamWriter StandardOutputWriter { get; set; }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        internal static void AttachConsole()
        {
            #if DEBUG
                if (StandardOutputWriter != null)
                {
                    Console.SetOut(StandardOutputWriter);
                    Console.Write("\n\n");
                }
            #endif
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [SetUp]
        public void OnAssemblySetUp()
        {
            #if DEBUG
                AllocConsole();

                StandardOutputHandle = 
                    new SafeFileHandle(GetStdHandle(StdOutHandleDescriptor), true);

                StandardOutputStream = new FileStream(StandardOutputHandle, FileAccess.Write);
                StandardOutputEncoding = System.Text.Encoding.GetEncoding(InternalCodePage);

                StandardOutputWriter = 
                    new StreamWriter(StandardOutputStream, StandardOutputEncoding)
                    {
                        AutoFlush = true
                    };

                Console.SetOut(StandardOutputWriter);
                Console.Title = "JmsChannel Test Console";
                Console.WindowWidth = 155;
                Console.WindowHeight = 50;
                Console.BufferWidth = 155;
                Console.BufferHeight = 9999;

                Console.WriteLine("This is a test of the emergency broadcast system.");
            #endif

            var fileInfo = new FileInfo("JmsChannel.Tests.log4net");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);
        }

        //----------------------------------------------------------------------------------------//
        /// <summary>
        /// 
        /// </summary>
        //----------------------------------------------------------------------------------------//

        [TearDown]
        public void OnAssemblyTeardown()
        {
            #if DEBUG
                AttachConsole();
                Console.WriteLine("Hit any key to exit.");
                Console.ReadKey();
                //System.Threading.Thread.Sleep(10000);
                FreeConsole();
            #endif
        }
    }
}

//------------------------------------------------------------------------------------------------//
// end of file
//------------------------------------------------------------------------------------------------//
