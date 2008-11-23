using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace VGMToolbox.format.sdat
{
    public class Smap
    {
        public const int EMPTY_FILE_ID = -1;
        
        private const string SEQ_HEADER = "# SEQ:";

        public struct SmapSeqStruct
        {
            public string label;
            public int number;
            public int fileID;
            public int bnk;
            public int vol;
            public int cpr;
            public int ppr;
            public int ply;
            public int hsize;
            public int size;
            public string name;
        }

        public Smap() { }

        public Smap(Sdat pSdat) 
        { 
            // SSEQ
            sseqSection = new SmapSeqStruct[pSdat.InfoSection.SdatInfoSseqs.Length];

            for (int i = 0; i < sseqSection.Length; i++)
            {
                sseqSection[i] = new SmapSeqStruct();
                sseqSection[i].number = i;

                if (pSdat.InfoSection.SdatInfoSseqs[i].fileId != null)
                {
                    int intFileId = BitConverter.ToInt16(pSdat.InfoSection.SdatInfoSseqs[i].fileId, 0);

                    if (i < pSdat.SymbSection.SymbSeqFileNames.Length)
                    {
                        sseqSection[i].label = pSdat.SymbSection.SymbSeqFileNames[i];
                        sseqSection[i].name =
                            pSdat.SymbSection.SymbSeqFileNames[i] + Sdat.SEQUENCE_FILE_EXTENSION;
                    }
                    else
                    {
                        sseqSection[i].label = "SSEQ" + intFileId.ToString("X4");
                        sseqSection[i].name = String.Format("SSEQ{0}{1}", intFileId.ToString("X4"), Sdat.SEQUENCE_FILE_EXTENSION);

                    }

                    sseqSection[i].fileID = BitConverter.ToInt16(pSdat.InfoSection.SdatInfoSseqs[i].fileId, 0);
                    sseqSection[i].bnk = BitConverter.ToInt16(pSdat.InfoSection.SdatInfoSseqs[i].bnk, 0);
                    sseqSection[i].vol = pSdat.InfoSection.SdatInfoSseqs[i].vol[0];
                    sseqSection[i].cpr = pSdat.InfoSection.SdatInfoSseqs[i].cpr[0];
                    sseqSection[i].ppr = pSdat.InfoSection.SdatInfoSseqs[i].ppr[0];
                    sseqSection[i].ply = pSdat.InfoSection.SdatInfoSseqs[i].ply[0];
                    // sseqSection[i].hsize; @TODO figure this out
                    sseqSection[i].size =
                        BitConverter.ToInt32(pSdat.FatSection.SdatFatRecs[BitConverter.ToInt16(pSdat.InfoSection.SdatInfoSseqs[i].fileId, 0)].nSize, 0);
                }
                else
                {
                    sseqSection[i].fileID = EMPTY_FILE_ID;
                }
            }
        }

        private SmapSeqStruct[] sseqSection;
        public SmapSeqStruct[] SseqSection { get { return sseqSection; } }

        private ArrayList sequenceArray = new ArrayList();
        public SmapSeqStruct[] SequenceArray { get { return (SmapSeqStruct[])sequenceArray.ToArray(typeof(SmapSeqStruct)); } }

        public Smap(string pSmapPath)
        {
            parseSmap(pSmapPath);
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

                textReader.Close();
                textReader.Dispose();
            }        
        }

        private void parseSeqLine(string pSeqLine)
        {
            SmapSeqStruct sequence = new SmapSeqStruct();
            sequence.label = pSeqLine.Substring(0, 28).Trim();
            sequence.number = Int16.Parse(pSeqLine.Substring(28, 6).Trim());
            if (pSeqLine.Length > 34)
            {
                sequence.fileID = Int16.Parse(pSeqLine.Substring(35, 6).Trim());
            }
            if (pSeqLine.Length > 83)
            {
                sequence.name = pSeqLine.Substring(84).Trim();
            }
            this.sequenceArray.Add(sequence);
        }
    }
}
