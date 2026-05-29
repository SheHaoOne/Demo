using System.IO;
using FlowDesk.Core;
using FlowDesk.SamplePlugin;

namespace FlowDesk.App.Services;

public static class AppBootstrapper
{
    public static PluginCatalog CreatePluginCatalog()
    {
        var catalog = new PluginCatalog();
        catalog.Register(new MessageStepPlugin());
        catalog.Register(new DelayStepPlugin());
        catalog.Register(new DataImportStepPlugin());
        catalog.Register(new DaqCardStepPlugin());

        var pluginDirectory = Path.Combine(AppContext.BaseDirectory, "Plugins");
        catalog.RegisterRange(new PluginAssemblyLoader().LoadFromDirectory(pluginDirectory));

        return catalog;
    }
}
