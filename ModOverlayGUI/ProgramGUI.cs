using ImGuiNET;
using ClickableTransparentOverlay;
using System;
using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;


namespace ModGUI
{
    public class ProgramGUI : Overlay
    {
        static String gameName = "Signalis";
        bool showGui = true;
        bool debugMod = true;
        bool help = true;

        bool dynamicHolster = false;
        bool[] showWeapon = { false,false,false,false};

        int elsterNormal = 0;
        String[] elsterString = { "Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon" };
        float playerSize = 1;
        float weaponSize = 1;

        bool auto_deselect = false;
        string input_str = "";
        string input_str2 = "";
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int key);
        public ProgramGUI()
        {
            // Initialization code for ImGui (if necessary)
            ImGui.CreateContext();
            ImGui.GetIO().Fonts.AddFontDefault();
        }
        protected override void Render()
        {
            if (Process.GetProcessesByName(gameName).Length == 0 && !debugMod) Close();

            if (GetAsyncKeyState(0x76) < 0)
            {
                showGui = !showGui;
                Thread.Sleep(150);
            }
            if (showGui) {
                ImGui.Begin("ModelManager",ImGuiWindowFlags.MenuBar);

                ImGui.ShowDemoWindow();
                if (!help)
                {
                    if (ImGui.Button("Apply Changes"))
                    {

                    }
                }

                if (ImGui.BeginTabBar("Manager"))
                {
                    ImGui.NewLine();
 
                    
                    if (ImGui.BeginTabItem("Help"))
                    {
                        help = true;
                        ImGui.Text("\nThank you for using my mod :D\n" +
                            "F7 to toggle the Overlay on and off\n" +
                            "By pressing [Apply Changes] the current changes should be applied into the game \n" +
                            "and the changes be saved into a XML file");
                        ImGui.EndTabItem();
                    }

                    
                    if (ImGui.BeginTabItem("Player Model"))
                    {
                        help = false;
                        ImGui.Text("\n");
                        ImGui.SeparatorText("Model Size");
                        ImGui.InputFloat("", ref playerSize, 0.01f, 1.0f, "%.3f");

                        ImGui.Text("NOTICE: Player size Model will affect gameplay\n -> bigger/ smaller hitbox will hit obstacles p.e.\n ");
                        ImGui.SeparatorText("Model Parts");
                        ImGui.ListBox($" ", ref elsterNormal, elsterString, elsterString.Length);//To Do BeginListBox
                        modelMenu();
                        ImGui.EndTabItem();
                    }

                    if(ImGui.BeginTabItem("Weapon Model"))
                    {
                        help = false;
                        ImGui.Text("\n");
                        ImGui.SliderFloat("Weapon Size", ref weaponSize, 0.1f, 5f);
                        ImGui.Text("NOTICE: Weapon size model will affect gameplay\n -> bigger will be harder to aim, the more closer the enemy are\n ");
                        ImGui.Separator();
                        if (debugMod) { ImGui.Checkbox("Dynamic / Manual Mode", ref dynamicHolster); }
                        ImGui.Text("Dynamic: Checks which Weapons are in the Inventory/ are equipped \nManual: Customize manually which Weapon should be shown");
                        ImGui.Separator();
                        ImGui.BeginDisabled(dynamicHolster);
                        ImGui.Text("\nManual Selection");
                        ImGui.Checkbox("Pistol Hip", ref showWeapon[0]);
                        ImGui.SameLine();
                        ImGui.Checkbox("Machete Hip", ref showWeapon[1]);
                        ImGui.Checkbox("Rifle Back", ref showWeapon[2]);
                        ImGui.Checkbox("Shotgun Hip", ref showWeapon[3]);
                        ImGui.EndDisabled();
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }

                ImGui.End();
            }
        }

        public void modelMenu()
        {
            //To Do
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.Checkbox("Pistol Hip", ref showWeapon[0]);
            ImGui.SameLine();
            ImGui.Checkbox("Machete Hip", ref showWeapon[1]);
            ImGui.Checkbox("Rifle Back", ref showWeapon[2]);
            ImGui.Checkbox("Shotgun Hip", ref showWeapon[3]);
            ImGui.EndGroup();   
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




