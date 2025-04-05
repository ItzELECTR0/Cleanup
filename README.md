# ELECTRO's Cleanup Program

This little program lets you add files and/or directories which will be deleted on system startup OR when the program is run.
<br>On first-run the program will give you a configuration menu. To access this menu again, run the app with '--config' tag.

There is a persistent mode which is activated per session and will continously check for the files/directories you add
<br>It will delete them immediately.

## To build from source
- Clone the project
- Open the solution with Visual Studio
- Go to Build > Batch build
- Tick the checkbox next to Release and untick the one next to Debug
- Start the build and wait. Your build will be in bin/Release
