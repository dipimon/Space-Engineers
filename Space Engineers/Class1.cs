using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Gui;
using VRageMath;



namespace Space_Engineers
{
    public class Class1 : MyGridProgram
    {
        
        //IMyGridTerminalSystem GridTerminalSystem;


        void Main()
        {
            Echo(Me.CustomName + " was last run " + Runtime.TimeSinceLastRun /*ElapsedTime.TotalSeconds asadassdasd*/ + " seconds ago.");
            const string nameVent = "Air Vents";
            const string nameSound = "Sound Blocks";
            const string nameInnerHangar = "Inner Hangardoor";

            List<IMyTerminalBlock> airVent = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> lcds = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> soundBlocks = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> doors = new List<IMyTerminalBlock>();
            List<IMyTerminalBlock> innerHangar = new List<IMyTerminalBlock>();

            var LCDColour = Color.White;
            var LCDText = " Life Support System:\r\n\r\n";
            var pressureStatus = "Pressurized";

            airVent = GetBlocksFromGroup(nameVent);
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);
            soundBlocks = GetBlocksFromGroup(nameSound);
            innerHangar = GetBlocksFromGroup(nameInnerHangar);
            GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors);
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);

            bool hangar = false;

            // Check the air vents        
            for (int i = 0; i < airVent.Count; i++)
            {
                string pressureInfo = airVent[i].DetailedInfo;
                if (pressureInfo.IndexOf("Not pressurized") != -1)
                {
                    if(pressureStatus == "Pressurized") {
                        LCDText += "                ----------- ALERT ----------- \r\n\r\n";
                    }
                    if (airVent[i].CustomName.Contains("[HANGAR]"))
                    {
                        for(int j = 0; j < innerHangar.Count; j++)
                        {
                            IMyDoor door = (IMyDoor)innerHangar[j];
                            innerHangar[j].GetActionWithName("Open_Off").Apply(innerHangar[j]);
                        }
                        PlaySound(soundBlocks, "[HANGAR]");
                    } else if(airVent[i].CustomName.Contains("[ENGINE]"))
                    {
                        CloseDoor(doors, "[ENGINE]");
                        PlaySound(soundBlocks, "[ENGINE]", "[CONTROLL]");
                    } else if(airVent[i].CustomName.Contains("[LIVING]"))
                    {
                        CloseDoor(doors, "[LIVING]");
                        PlaySound(soundBlocks, "[LIVING]", "[CONTROLL]");
                    } else if(airVent[i].CustomName.Contains("[COMBAT]"))
                    {
                        CloseDoor(doors, "[COMBAT]");
                        PlaySound(soundBlocks, "[COMBAT]", "[CONTROLL]");
                    } else if(airVent[i].CustomName.Contains("[CONTROLL]"))
                    {
                        CloseDoor(doors, "[CONTROLL]");
                        PlaySound(soundBlocks, "[CONTROLL]");
                    }
                    if (!hangar && airVent[i].CustomName.Contains("[HANGAR]"))
                    {
                        pressureStatus = "Depressurized";
                        hangar = true;
                    }
                    else {
                        LCDText += airVent[i].CustomName + " depressurised \r\n";
                        //ventList.Add(airVent[i].CustomName + " depressurised \r\n");
                        pressureStatus = "Depressurized";
                    }
                }
            }
            if (pressureStatus == "Depressurized")
            {
                LCDColour = Color.Red;
                /*SetText(LCDList(lcds, "[VENTS]"), "", LCDColour, false);
                SetText(LCDList(lcds, "[VENTS]"), " Life Support System:\r\n\r\n", LCDColour, false);
                SetText(LCDList(lcds, "[VENTS]"), "                ----------- ALERT ----------- \r\n\r\n", LCDColour, true);
                foreach(string text in ventList)
                {
                    SetText(LCDList(lcds, "[VENTS]"), text, LCDColour, true);
                }*/
              

            }
            else {
              /*  SetText(LCDList(lcds, "[VENTS]"), " ", LCDColour, false);
                SetText(LCDList(lcds, "[VENTS]"), " Life Support System:\r\n\r\n", LCDColour, false);
                SetText(LCDList(lcds, "[VENTS]"), " All " + (airVent.Count-1) + " zones are currently pressurised\r\n\r\n", LCDColour, true);
                foreach(IMyTerminalBlock block in airVent)
                {
                    SetText(LCDList(lcds, "[VENTS]"), block.CustomName + ":   " + block.DetailedInfo.Substring(block.DetailedInfo.IndexOf("Room pressure")) + "\r\n", LCDColour, true);
                }*/
                LCDText += " All " + (airVent.Count-1) + " zones are currently pressurised\r\n\r\n";
                for(int i = 0; i < airVent.Count; i++)
                {
                    LCDText += airVent[i].CustomName + ":   " + airVent[i].DetailedInfo.Substring(airVent[i].DetailedInfo.IndexOf("Room pressure")) + "\r\n";
                }
            }
            //LCDText += " ";
            //SetText(LCDList(lcds, "[VENT]"), " Life Support System:\r\n\r\n" , LCDColour, false);
            SetText(LCDList(lcds,"[VENT]"), LCDText, LCDColour);
        }

    // Method for finding block groups           
    List<IMyTerminalBlock> GetBlocksFromGroup(string group)
        {
            var blockGroups = new List<IMyBlockGroup>();
            GridTerminalSystem.GetBlockGroups(blockGroups);
            for (int i = 0; i < blockGroups.Count; i++)
            {
                if (blockGroups[i].Name == group)
                {
                    return blockGroups[i].Blocks;
                }
            }
            throw new Exception("GetBlocksFromGroup: Group \"" + group + "\" not found");
        }

        // Method for writting to LCD Panels      
        void SetText(List<IMyTerminalBlock> blocks, string LCDText, Color color)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyTextPanel panel = (IMyTextPanel)blocks[i];
                //panel.WritePublicText("\n\n", false);
                panel.WritePublicText(LCDText, false);
                panel.SetValue("FontColor", color);
                panel.ShowTextureOnScreen();
                panel.ShowPublicTextOnScreen();
                //panel.UpdateVisual();
            }
        }

        void PlaySound(List<IMyTerminalBlock> soundBlock, string nameTag, string nameTag2 = null)
        {
            for (int i = 0; i < soundBlock.Count; i++)
            {
                if (soundBlock[i].CustomName.Contains(nameTag))
                {
                    soundBlock[i].GetActionWithName("PlaySound").Apply(soundBlock[i]);
                }
                if (nameTag2 != null)
                {
                    if(soundBlock[i].CustomName.Contains(nameTag2))
                    {
                        soundBlock[i].GetActionWithName("PlaySound").Apply(soundBlock[i]);
                    }
                }
            }
        }

        void CloseDoor(List<IMyTerminalBlock> closeDoor, string nameTag)
        {
            for (int i = 0; i < closeDoor.Count; i++)
            {
                IMyDoor door = (IMyDoor)closeDoor[i];
                if (door.CustomName.Contains(nameTag))
                {
                    door.GetActionWithName("Open_Off").Apply(door);
                }
            }
        }

        List<IMyTerminalBlock> /*IEnumerable<IMyTerminalBlock>*/ LCDList(List<IMyTerminalBlock> textLCDs,string nameTag)
        {
            List<IMyTerminalBlock> LCDs = new List<IMyTerminalBlock>();

            for (int i = 0; i < textLCDs.Count; i++)
            {
                IMyTextPanel lcd = (IMyTextPanel)textLCDs[i];
                if (lcd.CustomName.Contains(nameTag))
                {
                    LCDs.Add(lcd);
                    //yield return lcd;
                }
            }
            return LCDs;
        }


    }
}
