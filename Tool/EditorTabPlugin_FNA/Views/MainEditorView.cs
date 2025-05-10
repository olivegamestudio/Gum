using EditorTabPlugin_FNA.Services;
using EditorTabPlugin_FNA.ViewModels;
using EditorTabPlugin_XNA.Services;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Plugins.InternalPlugins.EditorTab.Services;
using Gum.Plugins.ScrollBarPlugin;
using Gum.Wireframe;
using MonoGameGum;
using MonoGameGum.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EditorTabPlugin_FNA.Views;
public class MainEditorView : Grid
{
    #region Fields/Properties

    private Cursor _cursor;
    private Keyboard _keyboard;
    private LayerService _layerService;
    private BackgroundSpriteService _backgroundSpriteService;
    private CanvasBoundsService _canvasBoundsService;
    private ToolLayerService _toolLayerService;
    private readonly RulerService _rulerService;
    private ToolFontService _toolFontService;
    private readonly WindowsCursorLogic _windowsCursorLogic;
    private readonly SelectionManager _selectionManager;
    private CameraViewModel _cameraViewModel;
    private readonly HotkeyManager _hotkeyManager;
    readonly ScrollbarService _scrollbarService;
    private ScrollBarControlLogic _scrollBarControlLogic;

    #endregion

    public MainEditorView(
        LayerService layerService, 
        WindowsCursorLogic windowsCursorLogic,
        SelectionManager selectionManager)
    {
        _layerService = layerService;
        _toolLayerService = new ToolLayerService();
        _scrollBarControlLogic = new ScrollBarControlLogic();
        _scrollbarService = new ScrollbarService(_scrollBarControlLogic);
        _backgroundSpriteService = new BackgroundSpriteService();
        _canvasBoundsService = new CanvasBoundsService();
        _rulerService = new RulerService();
        _toolFontService = new ToolFontService();
        _windowsCursorLogic = windowsCursorLogic;
        _selectionManager = selectionManager;
        _hotkeyManager = Gum.Services.Builder.Get<HotkeyManager>();

        RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

        ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });


        var game = new EditorGame();


        var fnaControl = new EditorWindow(
            _backgroundSpriteService,
            game,
            _canvasBoundsService,
            _rulerService,
            _selectionManager,
            _windowsCursorLogic);
        Children.Add(fnaControl);

        _cursor = GumService.Default.Cursor;
        _keyboard = new Keyboard();

        _layerService.Initialize();
        _backgroundSpriteService.Initialize(game.SystemManagers);
        _canvasBoundsService.Initialize(_layerService);
        _toolLayerService.Initialize(game.SystemManagers);
        _rulerService.Initialize(_layerService,
            game.SystemManagers,
            _cursor,
            _keyboard,
            _toolFontService,
            _toolLayerService,
            _windowsCursorLogic);

        _selectionManager.Initialize(_layerService,
            _toolFontService,
            game.SystemManagers,
            _cursor);


        _cameraViewModel = new CameraViewModel(
            game.SystemManagers.Renderer.Camera,
            _hotkeyManager,
            _cursor,
            game.SystemManagers);

        var bottomGrid = new Grid();
        bottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
        bottomGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        this.Children.Add(bottomGrid);
        Grid.SetRow(bottomGrid, 1);

        InitializeScrollBars(game, bottomGrid, fnaControl);

        InitializeZoomComboBox(bottomGrid);

        fnaControl.KeyDownWinforms += HandleKeyDownWinforms;

        fnaControl.DataContext = _cameraViewModel;

        _scrollbarService.HandleXnaInitialized();

    }

    private void InitializeZoomComboBox(Grid bottomGrid)
    {
        var comboBox = new ComboBox();

        comboBox.DataContext = _cameraViewModel;

        comboBox.SetBinding(ComboBox.SelectedIndexProperty, nameof(_cameraViewModel.SelectedZoomIndex));
        comboBox.SetBinding(ComboBox.ItemsSourceProperty, nameof(_cameraViewModel.AvailableZoomLevels));

        bottomGrid.Children.Add(comboBox);
    }

    private void InitializeScrollBars(EditorGame game, Grid bottomGrid, EditorWindow fnaControl)
    {
        var verticalScrollBar = new ScrollBar();
        verticalScrollBar.Orientation = System.Windows.Controls.Orientation.Vertical;

        var horizontalScrollBar = new ScrollBar();
        horizontalScrollBar.Orientation = Orientation.Horizontal;

        Grid.SetRow(verticalScrollBar, 0);
        Grid.SetColumn(verticalScrollBar, 1);
        this.Children.Add(verticalScrollBar);

        Grid.SetColumn(horizontalScrollBar, 1);
        bottomGrid.Children.Add(horizontalScrollBar);


        _scrollBarControlLogic.Initialize(verticalScrollBar, horizontalScrollBar, fnaControl, game.SystemManagers);
    }

    public void HandleElementSelected(ElementSave elementSave)
    {
        _scrollbarService.HandleElementSelected(elementSave);
    }

    public void HandleCameraChanged()
    {
        _scrollbarService.HandleCameraChanged();
    }

    public void HandleWireframeResized()
    {
        _scrollbarService.HandleWireframeResized();
    }

    public void HandleGuidesChanged()
    {
        _rulerService.RefreshGuides();
    }

    private void HandleKeyDownWinforms(object arg1, System.Windows.Forms.KeyEventArgs args)
    {
        var msg = new System.Windows.Forms.Message();
        bool handled = _hotkeyManager.ProcessCmdKeyWireframe(ref msg, args.KeyData);

        if (!handled)
        {
            handled = _cameraViewModel.HandleKeyPress(args);
        }

        if (!handled)
        {
            _hotkeyManager.HandleKeyDownWireframe(args);
        }

        args.Handled = handled;
    }
}
