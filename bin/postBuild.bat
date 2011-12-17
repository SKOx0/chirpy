REM Merge assemblies
ILMerge /out:"Chirpy.dll" "TempChirpy.dll" "EcmaScript.NET.modified.dll" "Yahoo.YUI.Compressor.dll" "dotless.Core.dll" "AjaxMin.dll" "Jurassic.dll" /targetplatform:v4,C:\Windows\Microsoft.NET\Framework\v4.0.30319
REM Create VSI file
del Chirpy.vsi
del Chirpy.zip
7za.exe u Chirpy.zip Chirpy.dll Chirpy.vscontent Zippy.AddIn SassAndCoffee.Core.dll "Microsoft.Dynamic.dll" "Microsoft.Scripting.dll" "Microsoft.Scripting.Metadata.dll" "IronRuby.dll" "IronRuby.Libraries.dll" "IronRuby.Libraries.Yaml.dll"
rename Chirpy.zip Chirpy.vsi
REM Done