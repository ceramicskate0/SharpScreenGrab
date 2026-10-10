# SharpScreenGrab
SharpScreenGrab
Screencapture education poc for use in CobaltStrike .net exec methods
```
  _________.__                         _________                                    ________            ___.    
 /   _____/|  |__ _____ _____________ /   _____/ ___________   ____   ____   ____  /  _____/___________ \_ |__  
 \_____  \ |  |  \\__  \\_  __ \____ \\_____  \_/ ___\_  __ \_/ __ \_/ __ \ /    \/   \  __\_  __ \__  \ | __ \ 
 /        \|   Y  \/ __ \|  | \/  |_> >        \  \___|  | \/\  ___/\  ___/|   |  \    \_\  \  | \// __ \| \_\ \
/_______  /|___|  (____  /__|  |   __/_______  /\___  >__|    \___  >\___  >___|  /\______  /__|  (____  /___  /
        \/      \/     \/      |__|          \/     \/            \/     \/     \/        \/           \/    \/
Updated by @ceramicskate0       
```
## Usage:
            Commands:
            /outfolder:{Full folder path}
            /outformat:img
            /outformat:base64 
            /native
            /endtimeseconds:{INT for seconds to run} 

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
            
