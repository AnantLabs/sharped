using System.Windows;

namespace ShControls
{
    public class OpenFileDialog : FileDialog
    {
        public bool? ShowDialog(Window owner)
        {
            NativeMethods.OpenFileName ofn = ToOfn(owner);
            if (NativeMethods.GetOpenFileName(ofn))
            {
                FromOfn(ofn);
                return true;
            }
            FreeOfn(ofn);
            return false;
        }
    }
}