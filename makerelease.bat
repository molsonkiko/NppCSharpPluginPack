: this script requires 7-zip (https://www.7-zip.org/) to be installed on your computer
: sync testfiles with ones in repo
: ONLY EXECUTE THIS BATCH FILE FROM THE SAME DIRECTORY WHERE IT LIVES IN THE REPO!!!!

: copy translations and testfiles into 64-bit bin
xcopy .\testfiles .\NppCSharpPluginPack\bin\Release-x64\testfiles\ /s /y
xcopy .\translation .\NppCSharpPluginPack\bin\Release-x64\translation\ /s /y
: copy translations and testfiles into 32-bit bin
xcopy .\testfiles .\NppCSharpPluginPack\bin\Release\testfiles\ /s /y
xcopy .\translation .\NppCSharpPluginPack\bin\Release\translation\ /s /y
: zip testfiles and dlls to release zipfiles
: also copy directories to Downloads for easy access later
cd NppCSharpPluginPack\bin\Release-x64
xcopy . "%userprofile%\Downloads\NppCSharpPluginPack NEWEST x64\" /s /y
7z -r a ..\..\Release_x64.zip *.dll testfiles translation
cd ..\Release
xcopy . "%userprofile%\Downloads\NppCSharpPluginPack NEWEST x86\" /s /y
7z -r a ..\..\Release_x86.zip *.dll testfiles translation
cd ..\..\..