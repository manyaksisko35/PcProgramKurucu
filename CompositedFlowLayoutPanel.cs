using System.Windows.Forms;

namespace PcProgramKurucu
{
    // flowLayoutPanel1'in scroll sırasında child'ları (kartları) tek tek
    // yeniden çizmesini engelleyen, tam double-buffered flow panel.
    public class CompositedFlowLayoutPanel : FlowLayoutPanel
    {
        public CompositedFlowLayoutPanel()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
        }
    }
}
