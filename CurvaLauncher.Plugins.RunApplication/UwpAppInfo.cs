﻿using System.IO;

namespace CurvaLauncher.Plugins.RunApplication;

public class UwpAppInfo : AppInfo
{
    public string PackageId { get; set; } = string.Empty;
    public string FamilyID { get; set; } = string.Empty;
    public string PackageRootFolder { get; set; } = string.Empty;
    public string AppxManifestPath => Path.Combine(PackageRootFolder, "AppxManifest.xml");

    public string ApplicationId { get; set; } = string.Empty;
    public UwpAppLogo[] AppLogos { get; set; } = Array.Empty<UwpAppLogo>();
    
    public record struct UwpAppLogo(int Size, string Path);
}