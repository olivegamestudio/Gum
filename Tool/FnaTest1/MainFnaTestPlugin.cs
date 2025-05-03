using Gum;
using Gum.Plugins;
using Gum.Plugins.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFNA;

namespace FnaTest1;

[Export(typeof(PluginBase))]
internal class MainFnaTestPlugin : PluginBase
{
    public override string FriendlyName => "Editor Window (FNA)";

    public override bool ShutDown(PluginShutDownReason shutDownReason)
    {
        return true;
    }

    public override void StartUp()
    {
        GumCommands.Self.GuiCommands.PrintOutput("Starting it up!");


        var fnaControl = new FnaControl();
        var tab = this.CreateTab(fnaControl, "FNA test");
        tab.SuggestedLocation = TabLocation.RightBottom;
        tab.Show();

        //var tab = GumCommands.Self.GuiCommands.AddControl(fnaControl, "Fna Test");


    }
}
