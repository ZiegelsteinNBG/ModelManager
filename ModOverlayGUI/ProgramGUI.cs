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


        float size = 1;
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
            if(Process.GetProcessesByName(gameName).Length == 0)Close();
            if (GetAsyncKeyState(0x76) < 0)
            {
                showGui = !showGui;
                Thread.Sleep(150);  
            }
            if (showGui) { 
                ImGui.Begin("ModelManager", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.MenuBar);
                ImGui.Text("Signalis");

                ImGui.SliderFloat("Size", ref size, 0.25f, 2.5f);
                ImGui.Checkbox("Auto Deselect", ref auto_deselect);
                ImGui.BeginChild("Child window", new Vector2(300, 200));
                ImGui.Text("Child Window");
                ImGui.InputText("input", ref input_str, 16);
                ImGui.EndChild();

                ImGui.SameLine();
                ImGui.BeginChild("Child window2", new Vector2(300, 200));
                ImGui.Text("Child Window2");
                ImGui.InputText("input2", ref input_str2, 16);
                ImGui.EndChild();
                ImGui.End();
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Overlay...");
            Process[] processes = Process.GetProcessesByName(gameName);
            if (processes.Length > 0)//(processes.Length > 0)
            {
                ProgramGUI programGUI = new ProgramGUI();
                Console.WriteLine($"Game PID: {processes[0].Id}");
                programGUI.Start().Wait(); 
            }
            else
            {
                Console.WriteLine($"No processes found with the name: {gameName}");
            }
        }
    }
}




