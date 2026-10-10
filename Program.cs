using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SharpScreenGrab
{
    class Program
    {
        private static int secondsToScreenCap = 5;
        static void Main(string[] args)
        {
            Credit();
            var arguments = new Dictionary<string, string>();
            foreach (var argument in args)
            {
                var idx = argument.IndexOf(':');
                if (idx > 0)
                {
                    arguments[argument.Substring(0, idx).Remove(0, 1)] = argument.Substring(idx + 1);
                }
                else
                {
                    arguments[argument.Remove(0, 1)] = string.Empty;
                }
            }
            if (arguments.ContainsKey("help"))
            {
                Usage();
                return;
            }
            if (arguments.ContainsKey("endtimeseconds"))
            {
                int end =Convert.ToInt32(arguments["endtimeseconds"]);
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.Elapsed <= TimeSpan.FromSeconds(end))
                {
                    Thread.Sleep(secondsToScreenCap * 1000);
                    Run(arguments);
                }
            }
            else
            {
                Run(arguments);
            }
        }
        private static void Run(Dictionary<string, string> arguments)
        {
            if (arguments["outformat"] == "img")
            {
                if (!arguments.ContainsKey("outfolder"))
                {
                    Console.WriteLine("[*] /outfolder is required when saving to disk!");
                    return;
                }

                try
                {
                    var fileName = Path.GetRandomFileName();
                    var filePath = arguments["outfolder"] + "\\" + fileName;
                    if (arguments.ContainsKey("native"))
                    {
                        Console.WriteLine("[*] using Windows API");
                        var image = CaptureWindow(U32.GetDesktopWindow());
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        Console.WriteLine("[*] using .NET");
                        var image = CaptureWindowCLR();
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    Console.WriteLine("[*] saved to " + filePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[!] Exception. " + e.Message.ToString());
                }

                return;
            }
            else if (arguments["outformat"] == "b64")
            {
                try
                {
                    if (arguments.ContainsKey("native"))
                    {
                        Console.WriteLine("[*] using Windows 32 API");
                        var image = CaptureWindow(U32.GetDesktopWindow());
                        var base64 = GetCompressedImage(image);
                        Console.WriteLine(base64);
                    }
                    else
                    {
                        Console.WriteLine("[*] using .NET");
                        var bitmap = CaptureWindowCLR();
                        var base64 = GetCompressedImage(bitmap);
                        Console.WriteLine(base64);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[!] Exception. " + e.Message.ToString());
                }
                return;
            }
            else
            {
                Usage();
                return;
            }
        }

        private static string GetCompressedImage(Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var bytes = stream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                return base64;
            }
        }

        private static Bitmap CaptureWindowCLR()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                return bmp;
            }
        }

        private static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = U32.GetWindowDC(handle);
            U32.RECT windowRECT = new U32.RECT();
            U32.GetWindowRect(handle, ref windowRECT);
            int width = windowRECT.right - windowRECT.left;
            int height = windowRECT.bottom - windowRECT.top;

            IntPtr hdcDest = G32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = G32.CreateCompatibleBitmap(hdcSrc, width, height);
            IntPtr hOld = G32.SelectObject(hdcDest, hBitmap);
            G32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, G32.SRCCOPY);

            G32.SelectObject(hdcDest, hOld);

            var img = Image.FromHbitmap(hBitmap);

            G32.DeleteDC(hdcDest);
            U32.ReleaseDC(handle, hdcSrc);
            G32.DeleteObject(hBitmap);

            return img;
        }

        private static void Credit()
        {
            Console.WriteLine(@" 
  _________.__                         _________                                    ________            ___.    
 /   _____/|  |__ _____ _____________ /   _____/ ___________   ____   ____   ____  /  _____/___________ \_ |__  
 \_____  \ |  |  \\__  \\_  __ \____ \\_____  \_/ ___\_  __ \_/ __ \_/ __ \ /    \/   \  __\_  __ \__  \ | __ \ 
 /        \|   Y  \/ __ \|  | \/  |_> >        \  \___|  | \/\  ___/\  ___/|   |  \    \_\  \  | \// __ \| \_\ \
/_______  /|___|  (____  /__|  |   __/_______  /\___  >__|    \___  >\___  >___|  /\______  /__|  (____  /___  /
        \/      \/     \/      |__|          \/     \/            \/     \/     \/        \/           \/    \/ ");
            Console.WriteLine("");
            Console.WriteLine("By @ceramicskate0");
        }

        private static void Usage()
        {
            Console.WriteLine(@"
Commands:
/outfolder:{Full folder path}
/outformat:img
/outformat:base64 
/native
/endtimeseconds:{INT for seconds to run} (will screen cap every "+ secondsToScreenCap + " seconds)");
            Console.WriteLine("Capture a full window screenshot using the Windows API and save to disk:");
            Console.WriteLine(@"    SharpScreenGrab.exe /outfolder:c:\windows\temp /outformat:img /native");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot using .NET methods and save to disk:");
            Console.WriteLine(@"    SharpScreenGrab.exe /outfolder:c:\windows\temp /outformat:img");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot using the Windows API and write to the console as base64 (fileless):");
            Console.WriteLine(@"    SharpScreenGrab.exe /outformat:b64 /native");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot using .NET methods and write to the console as base64 (fileless):");
            Console.WriteLine(@"    SharpScreenGrab.exe /outformat:base64");
            Console.WriteLine("");
            Console.WriteLine("Capture a full window screenshot using .NET methods and write to the console as base64 for 60 seconds (fileless as b64 to console for 60 seconds every "+ secondsToScreenCap+" seconds):");
            Console.WriteLine(@"    SharpScreenGrab.exe /outformat:b64 /endtimeseconds:60");
        }
    }

    public static class G32
    {
        public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjectSource,
            int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
            int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
    
    public static class U32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
    }
}