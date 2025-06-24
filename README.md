# EyeRestApp
A simple, lightweight Windows desktop app that helps reduce eye strain by reminding you to take regular breaks with a fullscreen overlay.
![image](https://github.com/user-attachments/assets/b0d69b49-ac0d-4d41-ae3d-2f3153ecce95)
![image](https://github.com/user-attachments/assets/c94adc9d-14c4-4719-9b7d-5ae8257425e8)

## Features

-   **Fullscreen Break Overlay:** A non-intrusive but unmissable overlay appears when it's time for a break.
-   **20-Second Countdown:** The overlay displays a countdown timer and automatically disappears after 20 seconds.
-   **Customizable Timer:** Set your preferred break interval (in minutes) via the settings window.
-   **System Tray Integration:** The app runs quietly in the system tray, staying out of your way.
-   **Quick-Access Menu:** Right-click the tray icon to show settings, pause/resume the timer, or quit.
-   **Start with Windows:** A simple checkbox lets you automatically start the app on login.
-   **Lightweight:** Built with C# and .NET Windows Forms for minimal memory and CPU usage.

## How to Use (For Users)

1.  Go to the [**Releases Page**](https://github.com/anish-thapa/EyeRestApp-CSharp/releases). 
2.  Download the `EyeRestApp.exe` file from the latest release.
3.  Run the `.exe` file. The app will start in your system tray.

## How to Build (For Developers)

This project is built using .NET 8 and Windows Forms.

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build Command

Open a command prompt in the project's root directory and run the following command to create a small, framework-dependent executable:

```bash
dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true
