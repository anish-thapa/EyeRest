# EyeRestApp

A lightweight and elegant Windows desktop app that helps reduce eye strain by reminding you to take regular breaks with a beautiful, fullscreen overlay.

![EyeRestApp Screenshot](![image](https://github.com/user-attachments/assets/49263ce6-72d9-4c25-a028-0b7fef516925)
)

## ✨ Features

-   **Immersive Fullscreen Overlay:** A non-intrusive but unmissable overlay appears when it's time for a break, with a simple and clear message.
-   **Perfectly Centered Design:** The overlay uses a robust layout that looks clean and professional on any screen size.
-   **Live Countdown Timers:** Instantly see the time remaining until your next break, both in the settings window and by hovering over the system tray icon.
-   **Full System Tray Control:** The app runs quietly in your system tray. A right-click gives you instant access to show settings, pause/resume the timer, or quit.
-   **Intelligent Audio Control:** A custom sound plays to alert you, and it stops immediately if you choose to skip the break.
-   **"Start with Windows" Option:** A simple checkbox allows the app to launch automatically every time you log in.
-   **Lightweight & Efficient:** Built with modern C# and .NET Windows Forms for minimal memory and CPU usage. The final executable is tiny!

## 🚀 How to Use (For Users)

1.  Go to the [**Releases Page**](https://github.com/anish-thapa/EyeRest/releases). <!-- IMPORTANT: Replace "YourUsername" with your actual GitHub username! -->
2.  Download the `EyeRestApp.exe` file from the latest release.
3.  Run the `.exe` file. The app will start hidden in your system tray. Enjoy healthier work habits!

## 🛠️ How to Build (For Developers)

This project is built using .NET 8 and Windows Forms.

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build Command

Open a command prompt in the project's root directory and run the following command to create a small, framework-dependent executable:

```bash
dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true -p:SelfContained=false
