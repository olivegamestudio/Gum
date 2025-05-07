using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinCursor = System.Windows.Input.Cursor;

namespace EditorTabPlugin_FNA.Services;
public class WindowsCursorLogic
{

    bool mHasBeenSet = false;

    WinCursor mSetCursor = Cursors.Arrow;
    WinCursor mLastSet = Cursors.Arrow;

    public void StartCursorSettingFrameStart()
    {
        mHasBeenSet = false;
    }

    public void SetWinformsCursor(WinCursor cursor)
    {
        if (!mHasBeenSet)
        {
            mSetCursor = cursor;
            mHasBeenSet = true;
        }
    }

    public void EndCursorSettingFrameStart(System.Windows.FrameworkElement control)
    {
        Cursor desiredCursor = Cursors.Arrow;
        if (mHasBeenSet)
        {
            desiredCursor = mSetCursor;

        }
        else if (control.Cursor != Cursors.Arrow)
        {
            desiredCursor = Cursors.Arrow;

        }

        // May 7, 2025
        // No idea why, but
        // this flickers like
        // crazy:
        control.Cursor = desiredCursor;
        Mouse.OverrideCursor = desiredCursor;
        mLastSet = desiredCursor;

    }
}
