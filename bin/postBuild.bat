REM Merge assemblies
ILMerge /out:"Chirpy.dll" "TempChirpy.dll" "EcmaScript.NET.modified.dll" "Yahoo.YUI.Compressor.dll" "dotless.Core.dll" "AjaxMin.dll" "Jurassic.dll" /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319
REM Create VSI file

REM VS (v9)
del Chirpy-VisualStudio2008.vsi
del Chirpy.zip
del Zippy.AddIn
copy Zippy09.AddIn Zippy.AddIn
7za.exe u Chirpy.zip Chirpy.dll Chirpy.vscontent Zippy.AddIn "compiler.jar" "SassAndCoffee.Core.dll" "Microsoft.Dynamic.dll" "Microsoft.Scripting.dll" "Microsoft.Scripting.Metadata.dll" "IronRuby.dll" "IronRuby.Libraries.dll" "IronRuby.Libraries.Yaml.dll"
rename Chirpy.zip Chirpy-VisualStudio2008.vsi

REM VS 2010 (v10)
del Chirpy-VisualStudio2010.vsi
del Chirpy.zip
del Zippy.AddIn
copy Zippy10.AddIn Zippy.AddIn
7za.exe u Chirpy.zip Chirpy.dll Chirpy.vscontent Zippy.AddIn "compiler.jar" "SassAndCoffee.Core.dll" "Microsoft.Dynamic.dll" "Microsoft.Scripting.dll" "Microsoft.Scripting.Metadata.dll" "IronRuby.dll" "IronRuby.Libraries.dll" "IronRuby.Libraries.Yaml.dll"
rename Chirpy.zip Chirpy-VisualStudio2010.vsi

REM VS 11
del Chirpy-VisualStudio11.vsi
del Chirpy.zip
del Zippy.AddIn
copy Zippy11.AddIn Zippy.AddIn
7za.exe u Chirpy.zip Chirpy.dll Chirpy.vscontent Zippy.AddIn "compiler.jar" "SassAndCoffee.Core.dll" "Microsoft.Dynamic.dll" "Microsoft.Scripting.dll" "Microsoft.Scripting.Metadata.dll" "IronRuby.dll" "IronRuby.Libraries.dll" "IronRuby.Libraries.Yaml.dll"
rename Chirpy.zip Chirpy-VisualStudio11.vsi


REM VS v9,v10,v11
del Chirpy-VisualStudioAll.vsi
del Chirpy.zip
del Zippy.AddIn
copy Zippy10.AddIn Zippy.AddIn
7za.exe u Chirpy.zip Chirpy.dll ChirpyAll.vscontent Zippy.AddIn Zippy11.AddIn Zippy09.AddIn "compiler.jar" "SassAndCoffee.Core.dll" "Microsoft.Dynamic.dll" "Microsoft.Scripting.dll" "Microsoft.Scripting.Metadata.dll" "IronRuby.dll" "IronRuby.Libraries.dll" "IronRuby.Libraries.Yaml.dll"
rename Chirpy.zip Chirpy-VisualStudioAll.vsi


REM copy file to setup project
copy "Zippy.AddIn" "../Zippy.Setup.VS10/Zippy.AddIn"
copy "Chirpy.dll" "../Zippy.Setup.VS10/Chirpy.dll"
copy "compiler.jar" "../Zippy.Setup.VS10/compiler.jar"
REM copy SassAndCoffee.Core.dll ../Zippy.Setup.VS10/SassAndCoffee.Core.dll
REM copy Microsoft.Dynamic.dll ../Zippy.Setup.VS10/Microsoft.Dynamic.dll
REM copy Microsoft.Scripting.dll ../Zippy.Setup.VS10/Microsoft.Scripting.dll
REM copy Microsoft.Scripting.Metadata.dll ../Zippy.Setup.VS10/Microsoft.Scripting.Metadata.dll
REM copy IronRuby.dll ../Zippy.Setup.VS10/IronRuby.dll
REM copy IronRuby.Libraries.dll ../Zippy.Setup.VS10/IronRuby.Libraries.dll
REM copy IronRuby.Libraries.Yaml.dll ../Zippy.Setup.VS10/IronRuby.Libraries.Yaml.dll
REM Done