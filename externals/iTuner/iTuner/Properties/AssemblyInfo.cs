﻿//************************************************************************************************
// Copyright © 2010 Steven M. Cohn. All Rights Reserved.
//
//************************************************************************************************

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

[assembly: AssemblyTitle("iTuner")]
[assembly: AssemblyDescription("The iTunes Companion")]
[assembly: AssemblyConfiguration(".NET 3.5 AnyCPU")]
[assembly: AssemblyCompany("Steven M. Cohn")]
[assembly: AssemblyProduct("iTuner 1.2")]
[assembly: AssemblyCopyright("Copyright © 2010 Steven M. Cohn.  All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]

[assembly: ThemeInfo(
	ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
	//(used if a resource is not found in the page, 
	// or application resource dictionaries)
	ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
	//(used if a resource is not found in the page, 
	// app, or any theme specific resource dictionaries)
)]

[assembly: AssemblyVersion("1.2.*")]
[assembly: AssemblyFileVersion("1.2.3782")]

[assembly: InternalsVisibleTo("ControllerHarness")]
[assembly: InternalsVisibleTo("iTunerTests")]
