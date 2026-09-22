# WindowEnumerator

Small console app that enumerates top-level windows on the desktop and prints those that are visible and have a non-empty title.

What it does:
- Calls the native Windows API to enumerate top-level windows.
- Filters out non-visible windows and windows without titles.
- Prints each window handle (in hexadecimal) and its window title to the console.

How to run:
1. Open the solution in Visual Studio or run `dotnet run` from the project folder.
2. The console will list visible top-level window handles and titles.

Notes:
- This app uses P/Invoke (user32.dll) and must be run on Windows.
- No elevated privileges are required for basic enumeration, but some windows may hide or restrict information depending on system policies.
