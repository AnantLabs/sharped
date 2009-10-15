using System.Windows;

namespace ShControls
{
    public class SaveFileDialog : FileDialog
    {
        public bool? ShowDialog(Window owner)
        {
            NativeMethods.OpenFileName ofn = ToOfn(owner);
            if (NativeMethods.GetSaveFileName(ofn))
            {
                FromOfn(ofn);
                return true;
            }
            FreeOfn(ofn);
            return false;
        }
    }
}