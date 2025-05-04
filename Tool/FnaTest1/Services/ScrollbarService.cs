using Gum.DataTypes;
using Gum.Plugins.BaseClasses;
using Gum.ToolStates;
using Gum.Wireframe;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using WPFNA;

namespace Gum.Plugins.ScrollBarPlugin;

public class ScrollbarService
{
    ScrollBarControlLogic scrollBarControlLogic;

    public void HandleElementSelected(ElementSave obj)
    {

        GraphicalUiElement? ipso = null;

        if(obj != null)
        {
            ipso = WireframeObjectManager.Self.GetRepresentation(obj);
        }

        float minX = 0;
        float maxX = ProjectManager.Self.GumProjectSave.DefaultCanvasWidth;

        float minY = 0;
        float maxY = ProjectManager.Self.GumProjectSave.DefaultCanvasHeight;


        if(ipso != null)
        {
            var asGue = ipso;

            List<IRenderableIpso> toLoop = new List<IRenderableIpso>();

            if(GumState.Self.SelectedState.SelectedScreen != null)
            {
                toLoop.AddRange(asGue.ContainedElements);
            }
            else if(asGue.Children != null)
            {
                toLoop.AddRange(asGue.Children);
            }

            foreach(var item in toLoop)
            {
                UpdateMinMaxRecursively(item, ref minX, ref maxX, ref minY, ref maxY);
            }
        }

        scrollBarControlLogic?.SetDisplayedArea((int)maxX, (int)maxY);

    }

    private void UpdateMinMaxRecursively(IRenderableIpso item, ref float minX, ref float maxX, ref float minY, ref float maxY)
    {
        minX = Math.Min(minX, item.GetAbsoluteLeft());
        maxX = Math.Max(maxX, item.GetAbsoluteRight());

        minY = Math.Min(minY, item.GetAbsoluteTop());
        maxY = Math.Max(maxY, item.GetAbsoluteBottom());

        if(item.Children != null)
        {
            // this could be an invalid instance
            foreach(var child in item.Children)
            {
                UpdateMinMaxRecursively(child, ref minX, ref maxX, ref minY, ref maxY);
            }
        }
    }

    public void HandleWireframeInitialized(FnaControl wireframeControl1, System.Windows.Forms.Panel gumEditorPanel)
    {
        /// todo - need to get this working...
        //scrollBarControlLogic = new ScrollBarControlLogic(gumEditorPanel, wireframeControl1);
        //scrollBarControlLogic.SetDisplayedArea(800, 600);
    }

    public void HandleCameraChanged()
    {
        // I don't think we need to update
        // the canvas width or height anymore.
        // We do that whenever an object is selected...
        //if (ProjectManager.Self.GumProjectSave != null)
        //{

        //    scrollBarControlLogic.SetDisplayedArea(
        //        ProjectManager.Self.GumProjectSave.DefaultCanvasWidth,
        //        ProjectManager.Self.GumProjectSave.DefaultCanvasHeight);
        //}
        //else
        //{
        //    scrollBarControlLogic.SetDisplayedArea(800, 600);
        //}

        scrollBarControlLogic.UpdateScrollBars();
        scrollBarControlLogic.UpdateScrollBarsToCameraPosition();
    }

    public void HandleXnaInitialized()
    {
        //scrollBarControlLogic.Managers = global::RenderingLibrary.SystemManagers.Default;
        //scrollBarControlLogic.UpdateScrollBars();
    }

    public void HandleWireframeResized()
    {
        scrollBarControlLogic.UpdateScrollBars();
    }
}
