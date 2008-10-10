using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace VGMToolbox.tools.xsf
{
    class Smap
    {
        [DllImport("WinMM.dll")]
        private static extern long mciSendString(string strCommand,
            StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private const string SEQ_HEADER = "# SEQ:";

        public struct SmapSeqStruct
        {
            public string label;
            public int number;
            public string name;
        }        
        
        private ArrayList sequenceArray = new ArrayList();
        public SmapSeqStruct[] SequenceArray { get { return (SmapSeqStruct[])sequenceArray.ToArray(typeof(SmapSeqStruct)); } }

        public Smap(string pSmapPath)
        {
            parseSmap(pSmapPath);
            getMidiLength("foo");
        }

        private void parseSmap(string pSmapPath)
        {
            // Check Path
            if (File.Exists(pSmapPath))
            {
                string inputLine = String.Empty;
                
                TextReader textReader = new StreamReader(pSmapPath);
                while ((inputLine = textReader.ReadLine()) != null)
                {
                    // Check for SEQ header
                    if (inputLine.Trim().Equals(SEQ_HEADER))
                    {
                        // Skip columns headers
                        textReader.ReadLine();

                        // Read until EOF or End of SEQ section (blank line)
                        while (((inputLine = textReader.ReadLine()) != null) &&
                            !String.IsNullOrEmpty(inputLine.Trim()))
                        {
                            parseSeqLine(inputLine);
                        }
                    }
                }
            }        
        }

        private void parseSeqLine(string pSeqLine)
        {
            SmapSeqStruct sequence = new SmapSeqStruct();
            sequence.label = pSeqLine.Substring(0, 28).Trim();
            sequence.number = Int16.Parse(pSeqLine.Substring(28, 6).Trim());
            if (pSeqLine.Length > 83)
            {
                sequence.name = pSeqLine.Substring(84).Trim();
            }
            this.sequenceArray.Add(sequence);
        }

        private void getMidiLength(string pMidiPath)
        {
            StringBuilder strReturn = new StringBuilder(128);
            // IntPtr response = Marshal.AllocHGlobal(128);

            string path = "test.mid";
            string command;

            command = string.Format("open \"{0}\" type sequencer", path);
            long err = mciSendString(command, strReturn, 128, IntPtr.Zero);
            
            //mciSendString("set time format milliseconds", null, 0, IntPtr.Zero);            
            //long i = mciSendString("status length", strReturn, 20, IntPtr.Zero);
            int x = 20;

            
            
            

        }
    }
}
