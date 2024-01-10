﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CurvaLauncher.Models;
using CurvaLauncher.Plugin;

namespace CurvaLauncher.Services;

public partial class PluginService
{
    private readonly PathService _pathService;
    private readonly ConfigService _configService;

    public string Path { get; set; } = "Plugins";
    public string ConfigFileName { get; set; } = "Config.json";

    public ObservableCollection<CurvaLauncherPluginInstance> PluginInstances { get; } = new();

    public PluginService(
        PathService pathService,
        ConfigService configService)
    {
        _pathService = pathService;
        _configService = configService;
    }


    private DirectoryInfo EnsurePluginDirectories()
    {
        DirectoryInfo dir = new(_pathService.GetPath(Path));

        if (!dir.Exists)
            dir.Create();

        return dir;
    }

    private void CoreLoadPlugins(out List<CurvaLauncherPluginInstance> plugins)
    {
        plugins = new List<CurvaLauncherPluginInstance>();

        var dir = EnsurePluginDirectories();
        var dllFiles = dir.GetFiles("*.dll");

        AppConfig config = _configService.Config;

        foreach (FileInfo dllFile in dllFiles)
            if (CoreLoadPlugin(config, dllFile.FullName, out CurvaLauncherPluginInstance? plugin))
            {
                plugins.Add(plugin);
            }
    }

    private bool CoreLoadPlugin(AppConfig config, string dllFilePath, [NotNullWhen(true)] out CurvaLauncherPluginInstance? pluginInstance)
    {
        pluginInstance = null;

        try
        {
            var assembly = Assembly.LoadFile(dllFilePath);

            Type? pluginType = assembly.ExportedTypes
                .Where(type => type.IsAssignableTo(typeof(ISyncPlugin)) || type.IsAssignableTo(typeof(IAsyncPlugin)))
                .FirstOrDefault();

            if (pluginType == null)
                return false;

            if (!CurvaLauncherPluginInstance.TryCreate(pluginType, out pluginInstance))
                return false;

            var typeName = pluginType.FullName!;

            if (config.Plugins.TryGetValue(typeName, out var pluginConfig))
            {
                pluginInstance.IsEnabled = pluginConfig.IsEnabled;
                pluginInstance.Weight = pluginConfig.Weight;

                var props = pluginInstance.Plugin.GetType().GetProperties()
                        .Where(p => p.GetCustomAttribute<PluginOptionAttribute>() is not null);

                if (pluginConfig.Options != null)
                {
                    foreach (var property in props)
                    {
                        if (pluginConfig.Options.TryGetPropertyValue(property.Name, out var value))
                        {
                            var type = property.PropertyType;
                            var val = JsonSerializer.Deserialize(value, type);
                            property.SetValue(pluginInstance.Plugin, val);
                        }
                    }
                }
            }
            else
            {
                pluginInstance.IsEnabled = true;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    [RelayCommand]
    public void LoadAllPlugins()
    {
        CoreLoadPlugins(out var plugins);

        PluginInstances.Clear();
        foreach (var plugin in plugins)
            PluginInstances.Add(plugin);
    }

    [RelayCommand]
    public async Task ReloadAllPlugins()
    {
        foreach (var plugin in PluginInstances.Where(ins => ins.IsEnabled))
        {
            if (plugin.Plugin is ISyncPlugin syncPlugin)
                syncPlugin.Initialize();
            else if (plugin.Plugin is IAsyncPlugin asyncPlugin)
                await asyncPlugin.InitializeAsync();
        }
    }
}