using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Progressive.Commons.Views
{
    public abstract class GlassWindow : Window
    {
        // DWM の dll をロード
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern bool DwmIsCompositionEnabled();

        private const int WindowsVista = 6;
        private bool hasDwm;
        private IntPtr hwnd;
        private MARGINS margins;

        public GlassWindow()
        {
            hasDwm = HasDwm();
            margins = new MARGINS(new Thickness(-1));
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            InitializeGlass();
            base.OnSourceInitialized(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            UpdateGlass();
            base.OnActivated(e);
        }
        private bool HasDwm()
        {
            if (Environment.OSVersion.Version.Major < WindowsVista)
            {
                return false;
            }
            try
            {
                // DLLの存在チェック
                DwmIsCompositionEnabled();
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            return true;
        }
        private void InitializeGlass()
        {
            if (!hasDwm)
            {
                this.Background = SystemColors.ControlBrush;
                return;
            }
            hwnd = new WindowInteropHelper(this).Handle;
            if (hwnd == IntPtr.Zero)
                throw new InvalidOperationException();
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;
            UpdateGlass();
        }
        private void UpdateGlass()
        {
            if (!hasDwm)
            {
                return;
            }
            if (!DwmIsCompositionEnabled())
            {
                this.Background = SystemColors.ControlBrush;
                return;
            }
            this.Background = Brushes.Transparent;
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }
    }

    struct MARGINS
    {
        public MARGINS(Thickness t)
        {
            Left = (int)t.Left;
            Right = (int)t.Right;
            Top = (int)t.Top;
            Bottom = (int)t.Bottom;
        }
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
