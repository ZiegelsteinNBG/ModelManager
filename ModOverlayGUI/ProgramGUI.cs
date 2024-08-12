using ImGuiNET;
using ClickableTransparentOverlay;
using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using ModOverlayGUI;
using System.Security.AccessControl;

namespace ModGUI
{
    public class ProgramGUI : Overlay
    {
        static String gameName = "Signalis";
        private bool showGui = true;
        private bool debugMod = false;
        private bool help = true;

        private float weaponSize;
        private bool dynamicHolster;
        private string[] weaponName;
        private bool[] showWeapon;

        private int idModel;
        private String[] elsterString;
        private float playerSize;
        private bool[] active;
        private float height;
        private int selectedIndex;
        private ModData[] modDataArray = new ModData[5];
        private ModDataSets modDataSets;

        private ModData modData = new ModData();
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int key);

        public ProgramGUI()
        {
            // Initialization code for ImGui
            ImGui.CreateContext();
            ImGui.GetIO().Fonts.AddFontDefault();
            modDataSets = ModDataManager.LoadModDataSet();
            //modData = ModDataManager.LoadModData();
            selectedIndex = modDataSets.aktiv;
            modDataArray = modDataSets.modDatas;
            modData = modDataSets.modDatas[selectedIndex];
            if (modData != null)
            {
                elsterString = new string[modData.modelData.Count];
                for (int i = 0; i < modData.modelData.Count; i++)
                {
                    elsterString[i] = modData.modelData[i].modelName;
                }
                weaponName = modData.weaponName;
                loadData();
            }
            modData.windowed = showGui;
            //ModDataManager.SaveModData(modData);
            modDataSets.modDatas[selectedIndex] = modData;
            ModDataManager.SaveModDataSets(modDataSets);
        }

        private void loadData()
        {
            if(modData != null)
            {
                dynamicHolster = modData.weaponShowCase;
                showWeapon = modData.weaponBool;
                playerSize = modData.playerModelSize;
                weaponSize = modData.weaponModelSize;
                height = modData.localHeight;
            }
        }

        private void writeData()
        {
            if (modData != null)
            {
                modData.weaponShowCase = dynamicHolster;
                showWeapon = modData.weaponBool;
                modData.playerModelSize = playerSize;
                modData.weaponModelSize = weaponSize;
                modData.localHeight = height;
            }
            modData.call++;
            ModDataManager.SaveModData(modData);
        }

        private void writeDataSet()
        {
            if (modData != null)
            {
                modData.weaponShowCase = dynamicHolster;
                showWeapon = modData.weaponBool;
                modData.playerModelSize = playerSize;
                modData.weaponModelSize = weaponSize;
                modData.localHeight = height;
            }
            modData.call++;
            modDataSets.aktiv = selectedIndex;
            modDataSets.modDatas[selectedIndex] = modData;
            ModDataManager.SaveModDataSets(modDataSets);
        }
        protected override void Render()
        {
            if (Process.GetProcessesByName(gameName).Length == 0 && !debugMod) Close();
            
            if (GetAsyncKeyState(0x76) < 0)
            {
                showGui = !showGui;
                modData.windowed = showGui;
                modData.call++;
                ModDataManager.SaveModData(modData);
                Thread.Sleep(150);
            }
            
            if (showGui) {
                ImGui.Begin("ModelManager v1.1.0",ImGuiWindowFlags.MenuBar );

                //ImGui.ShowDemoWindow();
                if (!help)
                {
                    if (ImGui.Button("Apply Changes"))
                    {
                        //writeData();
                        writeDataSet(); 
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Reset Default"))
                    {
                        modData = ModDataManager.iniModData();
                        loadData();
                    }


                    if (ImGui.BeginCombo("Save files", $"Save {selectedIndex}"))
                    {
                        for (int i = 0; i < modDataArray.Length; i++)
                        {
                            // Use the item index as the label for simplicity, but you can customize this
                            string label = $"Save {i}";

                            bool isSelected = (i == selectedIndex);
                            if (ImGui.Selectable(label, isSelected))
                            {
                                selectedIndex = i; // Set the selected index when an item is clicked
                                modData = modDataArray[selectedIndex];
                                loadData();
                            }

                            // Set the initial focus when opening the combo (optional)
                            if (isSelected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }

                        ImGui.EndCombo();
                    }
                }
                    

                if (ImGui.BeginTabBar("Manager"))
                {
                    ImGui.NewLine();
 
                    if (ImGui.BeginTabItem("Help"))
                    {
                        help = true;
                        ImGui.Text("\nWhile this GUI is open, the game will be windowed to prevent it from \nbeing minimized while interacting with the GUI.\n");
                        ImGui.Separator();
                        ImGui.Text("\nPress F7 to toggle the overlay on and off.\n\n\n" +
                                       "[Reset Default] will reset the current save to default.\n[doesn't apply by default]\n\n\n"+
                                       "[Save files] allows you to save up to 5 different configurations.\n[doesn't apply by default]\n\n\n" +
                                       "By only pressing [Apply Changes], the current changes will be applied to the game\n" +
                                       "and the changes will be saved to an XML file.\n");
                        ImGui.Separator();
                        ImGui.Text("\nFor suggestions/ bug report please make a post on Nexusmods\nor contact me on Discord [ziegelstein]");
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Player Model"))
                    {
                        help = false;
                        ImGui.Text("\n");
                        ImGui.SeparatorText("Local Height Position");
                        if (ImGui.InputFloat("Height [-1.5|+1.5]", ref height, 0.01f, 1.0f, "%.3f"))
                        {
                            if (height < -1.5) height = -1.5f;
                            if (height > 1.5f) height = 1.5f;
                        }

                        ImGui.Text("NOTICE:\nONLY WORKS FOR FOLLOWING MODELS:\n\t->FKLR, MNHR, STAR, STRC, KLBR, Isa_Re\nUser needs to adjust height manually\n -> Intended for models like KLBR, STRC and STAR.\n");
                        ImGui.SeparatorText("Model Size");
                        if(ImGui.InputFloat("Size [0.25|3.0]", ref playerSize, 0.01f, 1.0f, "%.3f"))
                        {
                            if(playerSize < 0.25)playerSize = 0.25f;
                            if (playerSize > 3.0f) playerSize = 3.0f;
                        }

                        ImGui.Text("NOTICE: Player size model will affect gameplay\n -> Larger or smaller hitboxes will affect obstacle collisions.\n");
                        ImGui.SeparatorText("Model Parts");
                        ImGui.ListBox($" ", ref idModel, elsterString, elsterString.Length);
                        modelMenu(idModel);
                        ImGui.EndTabItem();
                    }

                    if(ImGui.BeginTabItem("Weapon Model"))
                    {
                        help = false;
                        ImGui.Text("\n");
                        ImGui.SeparatorText("Weapon Size");
                        if (ImGui.InputFloat("Size [0.25|3.0]", ref weaponSize, 0.01f, 1.0f, "%.3f"))
                        {
                            if (weaponSize < 0.25) weaponSize = 0.25f;
                            if (weaponSize > 3.0f) weaponSize = 3.0f;
                        }
                        ImGui.Text("NOTICE: Weapon size model will affect gameplay\n -> Larger weapons will be harder to aim, especially when enemies are closer.\n");
                        ImGui.SeparatorText("Visible Equip");
                        ImGui.Checkbox("Dynamic / Manual Mode", ref dynamicHolster);
                        if (dynamicHolster)
                        {
                            showWeapon = new bool[showWeapon.Length];
                        }
                        ImGui.Text("Dynamic: Checks which weapons are in the inventory or equipped.\nManual: Customize manually which weapon should be shown.");
                        ImGui.Separator();
                        ImGui.BeginDisabled(dynamicHolster);
                        ImGui.Text("\nManual Selection");
                        for (int i = 0; i < showWeapon.Length; i++)
                        {
                            ImGui.Checkbox(weaponName[i], ref showWeapon[i]);
                        }
                        ImGui.EndDisabled();
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
        }
  
        private void modelMenu(int id)
        {
            ModelData model = modData.modelData[id];
            active = modData.modelData[id].active;
            for (int i = 0; i < model.modelParts.Length; i++)
            {
                ImGui.Checkbox(model.modelParts[i], ref active[i]);
            }
            modData.modelData[id].active = active;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Overlay...");
            ProgramGUI programGUI = new ProgramGUI();
            Process[] processes = Process.GetProcessesByName(gameName);
            if (processes.Length > 0 || programGUI.debugMod)
            {
                if(!programGUI.debugMod)Console.WriteLine($"Game PID: {processes[0].Id}");
                programGUI.Start().Wait(); 
            }
            else
            {
                Console.WriteLine($"No processes found with the name: {gameName}");
            }
        }
    }
}