using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using VGMToolbox.format;

namespace VGMToolbox.tools.hoot
{
    class HootXmlGenerator
    {
        public VGMToolbox.tools.hoot.game[] GetHootGames(string pPath) 
        {
            ArrayList hootGamesArrayList = new ArrayList();
            FileStream fs = File.OpenRead(pPath);
            Type dataType = FormatUtil.getObjectType(fs);

            if (dataType != null)
            {
                IFormat vgmData = (IFormat)Activator.CreateInstance(dataType);
                vgmData.Initialize(fs);

                if (!String.IsNullOrEmpty(vgmData.GetHootDriver()))
                {
                    VGMToolbox.tools.hoot.game hootGame = new VGMToolbox.tools.hoot.game();
                    
                    string setName = "GAME NAME";
                    if (!String.IsNullOrEmpty(vgmData.GetSongName()))
                    {
                        setName = vgmData.GetSongName();
                    }
                    hootGame.name = String.Format("[{0}] {1} ({2})",
                        vgmData.GetHootDriverAlias(), setName, "JP PLACE HOLDER");

                    hootGame.driver = new VGMToolbox.tools.hoot.driver();
                    hootGame.driver.type = vgmData.GetHootDriverType();
                    hootGame.driver.Value = vgmData.GetHootDriver();

                    hootGame.driveralias = new VGMToolbox.tools.hoot.driveralias();
                    hootGame.driveralias.type = vgmData.GetHootDriverAlias();
                    hootGame.driveralias.Value = "COMPANY";

                    hootGame.romlist = new VGMToolbox.tools.hoot.romlist();
                    hootGame.romlist.archive = "INSERT ARCHIVE NAME HERE";
                    hootGame.romlist.rom = new VGMToolbox.tools.hoot.rom[1];
                    hootGame.romlist.rom[0] = new VGMToolbox.tools.hoot.rom();
                    hootGame.romlist.rom[0].type = "code";
                    hootGame.romlist.rom[0].Value = Path.GetFileName(pPath);
                    
                    hootGame.titlelist = new VGMToolbox.tools.hoot.title[vgmData.GetTotalSongs()];
                    int j = 0;
                    for (int i = vgmData.GetStartingSong(); 
                        i < (vgmData.GetStartingSong() + vgmData.GetTotalSongs()); i++)
                    {
                        VGMToolbox.tools.hoot.title hootTitle = new VGMToolbox.tools.hoot.title();
                        hootTitle.code = "0x" + i.ToString("X2");
                        hootTitle.Value = "BGM #" + i.ToString("X2");
                        
                        hootGame.titlelist[j] = hootTitle;
                        j++;
                    }


                    hootGamesArrayList.Add(hootGame);
                }
            }

            return (VGMToolbox.tools.hoot.game[])hootGamesArrayList.ToArray(typeof(VGMToolbox.tools.hoot.game));
        }    
    }
}
