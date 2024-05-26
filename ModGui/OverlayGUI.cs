using EasyImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DearImguiSharp;
using System.Windows.Forms;
using EasyImGui.Core;
using System.Threading;
using System.Diagnostics;
using GameOverlay.Windows;
using EasyImGui.Helpers;

namespace ModGui
{
    public class OverlayGUI
    {
        static int game_pid = 0;   

        static void Main(string[] args)
        {
            Run();
        }

        public static void Run()
        {

            string gameName = "Signalis";
            Process[] processes = Process.GetProcessesByName(gameName);
            bool front = true;

            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    Console.WriteLine($"Game PID: {process.Id}");
                    game_pid = process.Id;

                }

                Process GameProc = Process.GetProcessById(game_pid);

                Overlay OverlayWindow = new Overlay() { EnableDrag = true, ResizableBorders = true, Fix_WM_NCLBUTTONDBLCLK = true, NoActiveWindow = true, ShowInTaskbar = false };

                //SingleImguiWindow.ImGuiWindowFlagsEx = DearImguiSharp.ImGuiWindowFlags.NoTitleBar;
                OverlayWindow.OnImGuiReady += (object sender, bool Status) =>
                {

                    if (Status)
                    {
                        OverlayWindow.Imgui.ConfigContex += delegate {

                            var style = ImGui.GetStyle();

                            ImVec4[] styleColors = style.Colors;
                            styleColors[(int)ImGuiCol.CheckMark].W = 1.0f;
                            styleColors[(int)ImGuiCol.CheckMark].X = 1.0f;
                            styleColors[(int)ImGuiCol.CheckMark].Y = 1.0f;
                            styleColors[(int)ImGuiCol.CheckMark].Z = 1.0f;

                            styleColors[(int)ImGuiCol.FrameBg].W = 0.0f;
                            styleColors[(int)ImGuiCol.FrameBg].X = 0.0f;
                            styleColors[(int)ImGuiCol.FrameBg].Y = 0.0f;
                            styleColors[(int)ImGuiCol.FrameBg].Z = 0.0f;

                            style.WindowRounding = 5.0f;
                            style.FrameRounding = 5.0f;
                            style.FrameBorderSize = 1.0f;



                            OverlayWindow.Imgui.IO.ConfigFlags |= (int)ImGuiConfigFlags.ViewportsEnable;

                            return true;
                        };


                        bool DrawImguiMenu = true;
                        bool check = true;

                        bool option1 = false;

                        SharpDX.Direct3D9.Device device = OverlayWindow.D3DDevice;
                        OverlayWindow.Imgui.ConfigContex += delegate {
                            var io = ImGui.GetIO();
                            io.ConfigFlags |=  (int) ImGuiConfigFlags.DockingEnable | (int)ImGuiConfigFlags.ViewportsEnable;

                            var style = ImGui.GetStyle();
                            style.Colors[(int)ImGuiCol.WindowBg].W = 0.3f; 

                            return true;
                        };

                        OverlayWindow.Imgui.Render += delegate {

                            if (GameProc.HasExited)
                                Environment.Exit(0);

                            OverlayWindow.FitTo(GameProc.MainWindowHandle, true);
                            if(front)OverlayWindow.PlaceAbove(GameProc.MainWindowHandle);


                            if (DrawImguiMenu == false) { OverlayWindow.Close(); return true; }

                            if (OverlayWindow.Imgui.Imgui_Ini == true && OverlayWindow.Imgui.IO != null)
                            {
                                // Overlay Dragger
                                bool IsFocusOnMainImguiWindow = (Form.ActiveForm == OverlayWindow);
                                if (IsFocusOnMainImguiWindow == true) { InputHook.Universal(OverlayWindow.Imgui.IO); }
                                OverlayWindow.EnableDrag = (DearImguiSharp.ImGui.IsAnyItemActive() == true) ? false : IsFocusOnMainImguiWindow;


                                DearImguiSharp.ImGui.Begin("SkinManager", ref front, (int)(ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing));


                                if (DearImguiSharp.ImGui.Button("Message", new DearImguiSharp.ImVec2() { X = OverlayWindow.ClientSize.Width - 15, Y = 20 }))
                                {

                                }
                                if (DearImguiSharp.ImGui.Button("Exit", new DearImguiSharp.ImVec2() { X = OverlayWindow.ClientSize.Width - 15, Y = 20 })) 
                                { 
                                    front = false;
                                    OverlayWindow.SendToBack();
                                }

                                DearImguiSharp.ImGui.Checkbox("test", ref check);

                                if (DearImguiSharp.ImGui.Button("Open Menu", new ImVec2() { X = 100, Y = 20 }))
                                {
                                    DearImguiSharp.ImGui.OpenPopupStr("MyMenu", 0);
                                }

                                if (DearImguiSharp.ImGui.BeginPopup("MyMenu", 0))
                                {
                                    if (DearImguiSharp.ImGui.Checkbox("Option 1", ref option1))
                                    {
                                    
                                    }
                                    if (DearImguiSharp.ImGui.BeginMenu("Option 2", true))
                                    {

                                    }
                                    if (DearImguiSharp.ImGui.BeginMenu("Option 3", true))
                                    {

                                    }
                                    DearImguiSharp.ImGui.EndPopup();
                                }

                            }
                            else
                            {
                                Console.WriteLine("...");
                            }

                            return true;
                        };


                    }
                };
            
                try { System.Windows.Forms.Application.Run(OverlayWindow); } catch {
                    Console.WriteLine("Failure"); 
                    Environment.Exit(0); }
                }
            else
            {
                Console.WriteLine("Game is not running.");
            }
        }

    }
}
