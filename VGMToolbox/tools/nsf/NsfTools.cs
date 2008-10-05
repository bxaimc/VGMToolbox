using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;

namespace VGMToolbox.tools.nsf
{
    class NsfTools
    {
        public static void NsfeToNsf(Nsfe pNsfeData, string pPath)
        {            
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string outputPath = Path.GetDirectoryName(pPath) + Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(pPath) + ".nsf";
            
            byte[] songName = new byte[32];
            pNsfeData.SongNameBytes.CopyTo(songName, 0);
            
            byte[] songArtist = new byte[32];
            pNsfeData.SongArtistBytes.CopyTo(songArtist, 0);
            
            byte[] songCopyright = new byte[32];
            pNsfeData.SongCopyrightBytes.CopyTo(songCopyright, 0);

            byte[] bankswitchInit = new byte[8];
            if (pNsfeData.BankSwitchInit != null && pNsfeData.BankSwitchInit.Length > 0)
            {
                pNsfeData.BankSwitchInit.CopyTo(bankswitchInit, 0);
            }

            byte[] expansionBytes = new byte[] { 0x00, 0x00, 0x00, 0x00};
            byte[] startingSong = new byte[] { 0x01 };

            BinaryWriter bw = new BinaryWriter(File.Create(outputPath));
            bw.Write(Nsf.ASCII_SIGNATURE);
            bw.Write(Nsf.CURRENT_VERSION_NUMBER);
            bw.Write(pNsfeData.TotalSongs);
            bw.Write(startingSong);
            bw.Write(pNsfeData.LoadAddress);
            bw.Write(pNsfeData.InitAddress);
            bw.Write(pNsfeData.PlayAddress);
            bw.Write(songName);
            bw.Write(songArtist);
            bw.Write(songCopyright);

            if (((pNsfeData.PalNtscBits[0] & Nsf.MASK_NTSC) == Nsf.MASK_NTSC) ||
               ((pNsfeData.PalNtscBits[0] & Nsf.MASK_PAL_NTSC) == Nsf.MASK_PAL_NTSC))
            {
                bw.Write(new byte[] { 0x1a, 0x41 });
            }
            else 
            {
                bw.Write(new byte[] { 0x00, 0x00 });
            }
            
            bw.Write(bankswitchInit);

            if (((pNsfeData.PalNtscBits[0] & Nsf.MASK_PAL) == Nsf.MASK_PAL) ||
               ((pNsfeData.PalNtscBits[0] & Nsf.MASK_PAL_NTSC) == Nsf.MASK_PAL_NTSC))            
            {
                bw.Write(new byte[] { 0x20, 0x4e });
            }
            else
            {
                bw.Write(new byte[] { 0x00, 0x00 });
            }
                                    
            bw.Write(pNsfeData.PalNtscBits);
            bw.Write(pNsfeData.ExtraChipsBits);
            bw.Write(expansionBytes);
            bw.Write(pNsfeData.Data);

            bw.Close();
        }
    }
}
