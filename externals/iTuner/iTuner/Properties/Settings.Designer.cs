﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iTuner.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Library")]
        public string PlayerPlaylist {
            get {
                return ((string)(this["PlayerPlaylist"]));
            }
            set {
                this["PlayerPlaylist"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public ulong PlayerTrackID {
            get {
                return ((ulong)(this["PlayerTrackID"]));
            }
            set {
                this["PlayerTrackID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MP3")]
        public string ExportEncoder {
            get {
                return ((string)(this["ExportEncoder"]));
            }
            set {
                this["ExportEncoder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("M3U")]
        public string ExportPlaylistFormat {
            get {
                return ((string)(this["ExportPlaylistFormat"]));
            }
            set {
                this["ExportPlaylistFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\ExportedMusic")]
        public string ExportLocation {
            get {
                return ((string)(this["ExportLocation"]));
            }
            set {
                this["ExportLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ExportSubdirectories {
            get {
                return ((bool)(this["ExportSubdirectories"]));
            }
            set {
                this["ExportSubdirectories"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DuplicateScannerIsEnabled {
            get {
                return ((bool)(this["DuplicateScannerIsEnabled"]));
            }
            set {
                this["DuplicateScannerIsEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FileWatchScannerIsEnabled {
            get {
                return ((bool)(this["FileWatchScannerIsEnabled"]));
            }
            set {
                this["FileWatchScannerIsEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool MaintenanceScannerIsEnabled {
            get {
                return ((bool)(this["MaintenanceScannerIsEnabled"]));
            }
            set {
                this["MaintenanceScannerIsEnabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PhantomScannerIsEnabled {
            get {
                return ((bool)(this["PhantomScannerIsEnabled"]));
            }
            set {
                this["PhantomScannerIsEnabled"] = value;
            }
        }
    }
}
