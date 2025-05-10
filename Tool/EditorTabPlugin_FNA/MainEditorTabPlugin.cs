using EditorTabPlugin_FNA;
using EditorTabPlugin_FNA.Services;
using EditorTabPlugin_FNA.ViewModels;
using EditorTabPlugin_FNA.Views;
using EditorTabPlugin_XNA.Services;
using Gum;
using Gum.Commands;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Managers;
using Gum.Plugins;
using Gum.Plugins.BaseClasses;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Gum.Plugins.ScrollBarPlugin;
using Gum.ToolCommands;
using Gum.ToolStates;
using Gum.Wireframe;
using GumRuntime;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.Forms;
using MonoGameGum.Input;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using WPFNA;

namespace FnaTest1;

[Export(typeof(PluginBase))]
internal class MainEditorTabPlugin : PluginBase
{
    #region PropertiesSupportingIncrementalChange

    HashSet<string> PropertiesSupportingIncrementalChange = new HashSet<string>
        {
            "Animate",
            "Alpha",
            "AutoGridHorizontalCells",
            "AutoGridVerticalCells",
            "Blue",
            "CurrentChainName",
            "ChildrenLayout",
            "FlipHorizontal",
            "FontSize",
            "Green",
            "Height",
            "HeightUnits",
            "HorizontalAlignment",
            nameof(GraphicalUiElement.IgnoredByParentSize),
            "IsBold",
            "IsRenderTarget",
            "MaxLettersToShow",
            nameof(GraphicalUiElement.MaxHeight),
            nameof(Text.MaxNumberOfLines),
            nameof(GraphicalUiElement.MaxWidth),
            nameof(GraphicalUiElement.MinHeight),
            nameof(GraphicalUiElement.MinWidth),
            "Red",
            "Rotation",
            "SourceFile",
            "StackSpacing",
            "Text",
            "TextureAddress",
            "TextOverflowVerticalMode",
            "UseCustomFont",
            "UseFontSmoothing",
            "VerticalAlignment",
            "Visible",
            "Width",
            "WidthUnits",
            "X",
            "XOrigin",
            "XUnits",
            "Y",
            "YOrigin",
            "YUnits",
        };
    #endregion

    MainEditorView _mainEditorView;

    private readonly GuiCommands _guiCommands;
    private readonly LocalizationManager _localizationManager;
    private readonly ScreenshotService _screenshotService;
    private readonly SelectionManager _selectionManager;
    private readonly SinglePixelTextureService _singlePixelTextureService;
    private LayerService _layerService;
    private readonly WindowsCursorLogic _windowsCursorLogic;

    private EditingManager _editingManager;

    public override string FriendlyName => "Editor Window (FNA)";

    public MainEditorTabPlugin()
    {
        _windowsCursorLogic = new WindowsCursorLogic();
        _guiCommands = Gum.Services.Builder.Get<GuiCommands>();
        _localizationManager = Gum.Services.Builder.Get<LocalizationManager>();
        _editingManager = new EditingManager();
        _selectionManager = new SelectionManager(
            SelectedState.Self,
            _editingManager,
            GumCommands.Self.GuiCommands,
            _windowsCursorLogic);
        _screenshotService = new ScreenshotService(_selectionManager);
        _singlePixelTextureService = new SinglePixelTextureService();
        _layerService = new LayerService();

    }

    public override bool ShutDown(PluginShutDownReason shutDownReason)
    {
        return true;
    }

    public override void StartUp()
    {
        // These are handled by GumService:
        //GraphicalUiElement.SetPropertyOnRenderable = CustomSetPropertyOnRenderable.SetPropertyOnRenderable;
        //GraphicalUiElement.UpdateFontFromProperties = CustomSetPropertyOnRenderable.UpdateToFontValues;
        //GraphicalUiElement.ThrowExceptionsForMissingFiles = CustomSetPropertyOnRenderable.ThrowExceptionsForMissingFiles;
        //GraphicalUiElement.AddRenderableToManagers = CustomSetPropertyOnRenderable.AddRenderableToManagers;
        //GraphicalUiElement.RemoveRenderableFromManagers = CustomSetPropertyOnRenderable.RemoveRenderableFromManagers;

        _mainEditorView = new MainEditorView(_layerService, _windowsCursorLogic, _selectionManager);

        // assign events *after* creating the main editor view, because the main editor view handles some of the events
        AssignEvents();
        
        var tab = this.CreateTab(_mainEditorView, "Editor");
        tab.SuggestedLocation = TabLocation.RightTop;
        tab.Show();

        HandleXnaInitialized();

        //var tab = GumCommands.Self.GuiCommands.AddControl(fnaControl, "Fna Test");
    }



    private void HandleXnaInitialized()
    {
        //_wireframeEditControl.ZoomChanged += HandleControlZoomChange;

        //_wireframeControl.Initialize(_wireframeEditControl, gumEditorPanel, HotkeyManager.Self, _selectionManager);

        // _layerService must be created after _wireframeControl so that the SystemManagers.Default are assigned

        //_wireframeControl.ShareLayerReferences(_layerService);

        //_editingManager.Initialize(_wireframeContextMenuStrip);





        //this._wireframeControl.Parent.Resize += (not, used) =>
        //{
        //    UpdateWireframeControlSizes();
        //    PluginManager.Self.HandleWireframeResized();
        //};

        //this._wireframeControl.MouseClick += wireframeControl1_MouseClick;
        //this._wireframeControl.MouseDown += wireframeControl1_MouseDown;


        //this._wireframeControl.DragDrop += HandleFileDragDrop;
        //this._wireframeControl.DragEnter += DragDropManager.Self.HandleFileDragEnter;
        //this._wireframeControl.DragOver += (sender, e) =>
        //{
        //    //this.DoDragDrop(e.Data, DragDropEffects.Move | DragDropEffects.Copy);
        //    //DragDropManager.Self.HandleDragOver(sender, e);

        //};

        //this._wireframeControl.QueryContinueDrag += (sender, args) =>
        //{
        //    args.Action = DragAction.Continue;
        //};
        //this._wireframeControl.KeyDown += (o, args) =>
        //{
        //    if (args.KeyCode == Keys.Tab)
        //    {
        //        GumCommands.Self.GuiCommands.ToggleToolVisibility();
        //    }
        //};

        // Apply FrameRate, but keep it within sane limits
        float frameRate = Math.Max(Math.Min(ProjectManager.Self.GeneralSettingsFile.FrameRate, 60), 10);
        //_wireframeControl.DesiredFramesPerSecond = frameRate;

        //UpdateWireframeControlSizes();
    }

    private void AssignEvents()
    {
        this.CreateGraphicalUiElement += HandleCreateGraphicalUiElement;

        this.ReactToStateSaveSelected += HandleStateSelected;

        this.InstanceSelected += HandleInstanceSelected;
        this.InstanceReordered += HandleInstanceReordered;
        this.InstanceDelete += HandleInstanceDelete;

        this.ElementSelected += HandleElementSelected;
        this.ElementDelete += HandleElementDeleted;

        this.VariableSet += HandleVariableSet;
        this.VariableSetLate += HandleVariableSetLate;

        this.CategoryDelete += HandleCategoryDelete;
        this.TryHandleDelete += HandleDelete;

        this.StateDelete += HandleStateDelete;

        this.CameraChanged += _mainEditorView.HandleCameraChanged;

        // This may not be needed anymore:
        //this.XnaInitialized += HandleXnaInitialized;

        this.WireframeRefreshed += HandleWireframeRefreshed;
        this.WireframePropertyChanged += HandleWireframePropertyChanged;

        this.UiZoomValueChanged += HandleUiZoomValueChanged;


        this.IpsoSelected += HandleIpsoSelected;
        this.SetHighlightedIpso += HandleSetHighlightedElement;

        this.ProjectLoad += HandleProjectLoad;
        this.ProjectPropertySet += HandleProjectPropertySet;

        this.CreateRenderableForType += HandleCreateRenderableForType;
        this.GetSelectedIpsos += HandleGetSelectedIpsos;

        this.AfterUndo += HandleAfterUndo;

        this.ElementSelected += _mainEditorView.HandleElementSelected;
        this.WireframeResized += _mainEditorView.HandleWireframeResized;
        this.GuidesChanged += _mainEditorView.HandleGuidesChanged;
    }

    private void HandleProjectLoad(GumProjectSave save)
    {
        GraphicalUiElement.CanvasWidth = save.DefaultCanvasWidth;
        GraphicalUiElement.CanvasHeight = save.DefaultCanvasHeight;


        //_wireframeControl.UpdateCanvasBoundsToProject();


        _selectionManager.RestrictToUnitValues =
            save.RestrictToUnitValues;

        AdjustTextureFilter();
    }

    private void HandleAfterUndo()
    {
        _selectionManager.Refresh();
    }

    List<IPositionedSizedObject> ipsosToReturn = new List<IPositionedSizedObject>();
    private IEnumerable<IPositionedSizedObject>? HandleGetSelectedIpsos()
    {
        ipsosToReturn.Clear();

        if (SelectedState.Self.SelectedInstance != null)
        {
            foreach (var instance in SelectedState.Self.SelectedElement.Instances)
            {
                var representation = WireframeObjectManager.Self.GetRepresentation(instance);
                if (representation != null)
                {
                    ipsosToReturn.Add(representation);
                }
            }
        }
        else if (SelectedState.Self.SelectedElement != null)
        {
            var representation = WireframeObjectManager.Self.GetRepresentation(SelectedState.Self.SelectedElement);
            if (representation != null)
            {
                ipsosToReturn.Add(representation);
            }
        }

        return ipsosToReturn;

    }

    private void HandleProjectPropertySet(string propertyName)
    {
        if (propertyName == nameof(GumProjectSave.TextureFilter))
        {
            AdjustTextureFilter();
        }
        if (propertyName == nameof(GumProjectSave.RestrictToUnitValues))
        {
            _selectionManager.RestrictToUnitValues =
                ProjectManager.Self.GumProjectSave.RestrictToUnitValues;
        }
        else if (propertyName == nameof(GumProjectSave.SinglePixelTextureFile) ||
            propertyName == nameof(GumProjectSave.SinglePixelTextureTop) ||
            propertyName == nameof(GumProjectSave.SinglePixelTextureLeft) ||
            propertyName == nameof(GumProjectSave.SinglePixelTextureRight) ||
            propertyName == nameof(GumProjectSave.SinglePixelTextureBottom))
        {
            _singlePixelTextureService.RefreshSinglePixelTexture();

            WireframeObjectManager.Self.RefreshAll(forceLayout: true, forceReloadTextures: true);
        }
    }

    private void AdjustTextureFilter()
    {
        var project = ObjectFinder.Self.GumProjectSave;

        if (project != null)
        {
            switch (project.TextureFilter)
            {
                case nameof(TextureFilter.Linear):
                    _layerService.MainEditorLayer.IsLinearFilteringEnabled = true;
                    break;
                case nameof(TextureFilter.Point):
                default:
                    _layerService.MainEditorLayer.IsLinearFilteringEnabled = false;

                    break;
            }
        }
    }

    private void HandleSetHighlightedElement(IPositionedSizedObject whatToHighlight)
    {
        _selectionManager.HighlightedIpso = whatToHighlight;
    }

    private void HandleIpsoSelected(IPositionedSizedObject ipso)
    {
        _selectionManager.SelectedGue = ipso as GraphicalUiElement;
    }

    private void HandleWireframeRefreshed()
    {
        _selectionManager.Refresh();
    }

    private void HandleUiZoomValueChanged()
    {
        // Uncommenting this makes the area for teh combo box properly grow, but it
        // kills the wireframe view. Not sure why....
        //_wireframeEditControl.Height = _defaultWireframeEditControlHeight * _guiCommands.UiZoomValue / 100;
    }


    private void HandleWireframePropertyChanged(string name)
    {
        if (name == nameof(WireframeCommands.AreHighlightsVisible))
        {
            _selectionManager.AreHighlightsVisible =
                GumCommands.Self.WireframeCommands.AreHighlightsVisible;
        }
        else if (name == nameof(WireframeCommands.IsBackgroundGridVisible))
        {
            //_wireframeControl.BackgroundSprite.Visible =
            //    GumCommands.Self.WireframeCommands.IsBackgroundGridVisible;
        }
        else if (name == nameof(WireframeCommands.AreRulersVisible))
        {
            //_wireframeControl.RulersVisible =
            //    GumCommands.Self.WireframeCommands.AreRulersVisible;
        }
        else if (name == nameof(WireframeCommands.AreCanvasBoundsVisible))
        {
            //_wireframeControl.CanvasBoundsVisible =
            //    GumCommands.Self.WireframeCommands.AreCanvasBoundsVisible;
        }
    }



    private void HandleCategoryDelete(StateSaveCategory category)
    {
        _selectionManager.Refresh();
    }

    private void HandleStateDelete(StateSave save)
    {
        _selectionManager.Refresh();

    }

    private bool HandleDelete()
    {
        return _selectionManager.TryHandleDelete();
    }

    private void HandleVariableSet(ElementSave save1, InstanceSave save2, string arg3, object arg4)
    {
        _selectionManager.Refresh();
    }


    private void HandleVariableSetLate(ElementSave element, InstanceSave instance, string qualifiedName, object oldValue)
    {
        /////////////////////////////Early Out//////////////////////////
        if (element == null)
        {
            // This could be a variable on a behavior or instance in a behavior. If so, we don't show anything in the editor
            return;
        }
        ////////////////////////////End Early Out///////////////////////

        if (instance != null)
        {
            qualifiedName = instance.Name + "." + qualifiedName;
        }

        var state = SelectedState.Self.SelectedStateSave ?? element?.DefaultState;
        var value = state.GetValue(qualifiedName);

        var areSame = value == null && oldValue == null;
        if (!areSame && value != null)
        {
            areSame = value.Equals(oldValue);
        }

        var unqualifiedMember = qualifiedName;
        if (qualifiedName.Contains("."))
        {
            unqualifiedMember = qualifiedName.Substring(qualifiedName.LastIndexOf('.') + 1);
        }

        // Inefficient but let's do this for now - we can make it more efficient later
        // November 19, 2019
        // While this is inefficient
        // at runtime, it is *really*
        // inefficient for debugging. If
        // a set value fails, we have to trace
        // the entire variable assignment and that
        // can take forever. Therefore, we're going to
        // migrate towards setting the individual values
        // here. This can expand over time to just exclude
        // the RefreshAll call completely....but I don't know
        // if that will cause problems now, so instead I'm going
        // to do it one by one:
        var handledByDirectSet = false;

        var supportsIncrementalChange = PropertiesSupportingIncrementalChange.Contains(unqualifiedMember);

        // If the values are the same they may have been set to be the same by a plugin that
        // didn't allow the assignment, so don't go through the work of saving and refreshing.
        // Update January 19, 2025 - actually for incrmeental changes just use it, it will be fast
        if (!areSame || supportsIncrementalChange)
        {

            // if a deep reference is set, then this is more complicated than a single variable assignment, so we should
            // force everything. This makes debugging a little more difficult, but it keeps the wireframe accurate without having to track individual assignments.
            if (PropertiesSupportingIncrementalChange.Contains(unqualifiedMember) &&
            // June 19, 2024 - if the value is null (from default assignment), we
            // can't set this single value - it requires a recursive variable finder.
            // for simplicity (for now?) we will just refresh all:
                value != null &&

                (instance != null || SelectedState.Self.SelectedComponent != null || SelectedState.Self.SelectedStandardElement != null))
            {
                // this assumes that the object having its variable set is the selected instance. If we're setting
                // an exposed variable, this is not the case - the object having its variable set is actually the instance.
                //GraphicalUiElement gue = WireframeObjectManager.Self.GetSelectedRepresentation();
                GraphicalUiElement gue = null;
                if (instance != null)
                {
                    gue = WireframeObjectManager.Self.GetRepresentation(instance);
                }
                else
                {
                    gue = WireframeObjectManager.Self.GetSelectedRepresentation();
                }

                // If we dispose a file, we should re-create the screen for sure!
                var disposedFile = false;

                if (gue != null)
                {
                    VariableSave variable = null;
                    if (element != null)
                    {
                        variable = ObjectFinder.Self.GetRootVariable(qualifiedName, element);
                    }

                    if (variable?.IsFile == true && value is string asString)
                    {
                        try
                        {
                            var standardized = ToolsUtilities.FileManager.Standardize(asString, preserveCase: true, makeAbsolute: true);
                            standardized = ToolsUtilities.FileManager.RemoveDotDotSlash(standardized);
                            // invalidate files...
                            var loaderManager = global::RenderingLibrary.Content.LoaderManager.Self;

                            var existing = loaderManager.GetDisposable(standardized);

                            disposedFile = existing != null;

                            loaderManager.Dispose(standardized);

                        }

                        catch
                        {
                            // this could be an invalid file name, so tolerate crashes
                        }
                    }

                    gue.SetProperty(unqualifiedMember, value);

                    WireframeObjectManager.Self.RootGue?.ApplyVariableReferences(SelectedState.Self.SelectedStateSave);
                    //gue.ApplyVariableReferences(SelectedState.Self.SelectedStateSave);

                    handledByDirectSet = !disposedFile;
                }
                if (unqualifiedMember == "Text" && _localizationManager.HasDatabase)
                {
                    WireframeObjectManager.Self.ApplyLocalization(gue, value as string);
                }
            }

            if (!handledByDirectSet)
            {
                WireframeObjectManager.Self.RefreshAll(true, forceReloadTextures: false);
            }


            _selectionManager.Refresh();
        }
    }

    private IRenderableIpso? HandleCreateRenderableForType(string type)
    {
        return RuntimeObjectCreator.TryHandleAsBaseType(type, SystemManagers.Default) as IRenderableIpso;
    }

    private void HandleInstanceSelected(ElementSave element, InstanceSave instance)
    {
        WireframeObjectManager.Self.RefreshAll(forceLayout: false);
        _editingManager.RefreshContextMenuStrip();
        _selectionManager.WireframeEditor?.UpdateAspectRatioForGrabbedIpso();
        _selectionManager.Refresh();
    }

    private void HandleElementDeleted(ElementSave save)
    {
        WireframeObjectManager.Self.RefreshAll(true);
    }

    private void HandleInstanceDelete(ElementSave save1, InstanceSave save2)
    {
        _selectionManager.Refresh();
    }

    private void HandleInstanceReordered(InstanceSave save)
    {
        _selectionManager.Refresh();
    }

    private void HandleElementSelected(ElementSave save)
    {
        WireframeObjectManager.Self.RefreshAll(forceLayout: true);
        _selectionManager.Refresh();

    }

    private GraphicalUiElement? HandleCreateGraphicalUiElement(ElementSave elementSave)
    {
        var toReturn = elementSave.ToGraphicalUiElement(SystemManagers.Default, addToManagers: false);
        toReturn.AddToManagers(SystemManagers.Default, _layerService.MainEditorLayer);
        UpdateTextOutlines(toReturn);
        return toReturn;
    }

    private void UpdateTextOutlines(GraphicalUiElement rootGue)
    {
        if (rootGue.Component is Text text)
        {
            text.RenderBoundary = ProjectManager.Self.GeneralSettingsFile.ShowTextOutlines;
        }
        if (rootGue.Children != null)
        {
            foreach (var child in rootGue.Children)
            {
                if (child is GraphicalUiElement gue)
                {
                    UpdateTextOutlines(gue);
                }
            }
        }
        else
        {
            foreach (var child in rootGue.ContainedElements)
            {
                UpdateTextOutlines(child);
            }
        }
    }

    private void HandleStateSelected(StateSave save)
    {
        WireframeObjectManager.Self.RefreshAll(forceLayout: true);
    }
}
