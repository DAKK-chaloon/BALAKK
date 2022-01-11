using BALAKK;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon trayIcon;
        private string ProcessToCheck = "dofus";
        private string ProcessName = "Dofus";

        public MainWindow()
        {
            //InitializeComponent();

            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(Event);

            //KeyHandler.Start();
            //KeyHandler.MouseAction += new EventHandler(ChangeWindowEvent);

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = BALAKK.Properties.Resources.BALAKKIcon,
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };

            trayIcon.ContextMenuStrip.Items.Add("Quitter", null, new EventHandler(Exit));
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Déclaration des différentes fonctions qui sont utilisées
        /// </summary>
        /// <returns></returns>
        #region Fonctions

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        public struct POINT
        {
            public int X;
            public int Y;
        }

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10, // 0x0000000A
            ForceMinimized = 11, // 0x0000000B
        }

        private static int MakeLParam(int LoWord, int HiWord) => HiWord << 16 | LoWord & (int)ushort.MaxValue;

        public static string GetActiveWindowName()
        {
            try
            {
                IntPtr foregroundWindow = GetForegroundWindow();
                foreach (Process process in Process.GetProcesses())
                {
                    if (foregroundWindow == process.MainWindowHandle)
                        return process.ProcessName;
                }
            }
            catch
            {
            }
            return (string)null;
        }
        #endregion

        /// <summary>
        /// Morceau de code qui gère la getion de l'app dans la barre des taches
        /// </summary>
        #region Gestion de l'app en mode TrayIcon

        private void Event(object sender, EventArgs e)
        {
            bool flag = Keyboard.IsKeyDown((Key)120);
            ProcessName = GetActiveWindowName();
            if (ProcessName == "Dofus")
            {
                ProcessToCheck = "Dofus";
            }
            else if (ProcessName == "Dofus Retro")
            {
                ProcessToCheck = "Dofus Retro";
            }
            else
            {
                return;
            }

            System.Drawing.Point position = System.Windows.Forms.Cursor.Position;
            IntPtr ancestor = GetAncestor(WindowFromPoint(position.X, position.Y), 2U);
            POINT point;
            point.X = position.X;
            point.Y = position.Y;
            ref POINT local = ref point;
            ScreenToClient(ancestor, ref local);
            PostMessage(ancestor, 513U, 1, MakeLParam(point.X, point.Y));
            PostMessage(ancestor, 514U, 0, MakeLParam(point.X, point.Y));
            foreach (Process process in Process.GetProcessesByName(ProcessToCheck))
            {
                if (process.MainWindowHandle != ancestor && process.MainWindowHandle != (IntPtr)0)
                {
                    int rnd = new Random().Next(210, 340);
                    IntPtr mainWindowHandle = process.MainWindowHandle;
                    Console.WriteLine(mainWindowHandle + process.ProcessName);
                    if (!flag)
                    {
                        Task<int> task = Task.Run<int>((Func<Task<int>>)(async () =>
                        {
                            await Task.Delay(rnd);
                            return 42;
                        }));
                        try
                        {
                            task.Wait();
                        }
                        catch (AggregateException ex)
                        {
                            foreach (Exception innerException in ex.InnerExceptions)
                                Console.WriteLine("{0}: {1}", (object)innerException.GetType().Name, (object)innerException.Message);
                        }
                    }
                    PostMessage(mainWindowHandle, 513U, 1, MakeLParam(point.X, point.Y));
                    PostMessage(mainWindowHandle, 514U, 0, MakeLParam(point.X, point.Y));
                }
            }
        }

        //private void ChangeWindowEvent(object sender, EventArgs e)
        //{
        //    ProcessName = GetActiveWindowName();
        //    if (ProcessName == "Dofus")
        //    {
        //        ProcessToCheck = "Dofus";
        //    }
        //    else if (ProcessName == "Dofus Retro")
        //    {
        //        ProcessToCheck = "Dofus Retro";
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    System.Drawing.Point position = System.Windows.Forms.Cursor.Position;
        //    IntPtr ancestor = GetAncestor(WindowFromPoint(position.X, position.Y), 2U);
        //    Process[] ListProcess = Process.GetProcessesByName(ProcessToCheck);
        //    foreach (Process process in ListProcess)
        //    {
        //        if (process.MainWindowHandle == ancestor && process.MainWindowHandle != (IntPtr)0)
        //        {
        //            int processPosition = Array.IndexOf(ListProcess, process);
        //            Console.WriteLine(processPosition);
        //            if (processPosition != ListProcess.Length - 1)
        //            {
        //                SetForegroundWindow(ListProcess[processPosition + 1].MainWindowHandle);
        //            } else
        //            {
        //                SetForegroundWindow(ListProcess[0].MainWindowHandle);
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
