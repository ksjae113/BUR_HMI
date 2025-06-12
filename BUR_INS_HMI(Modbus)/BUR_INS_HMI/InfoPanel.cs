using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUR_INS_HMI
{
    public partial class InfoPanel : UserControl
    {
        public Label[] roll;

        public InfoPanel()
        {
            InitializeComponent();

            roll = new Label[] {
            roll_num1, roll_num2, roll_num3, roll_num4, roll_num5, roll_num6,
            roll_num7, roll_num8, roll_num9, roll_num10, roll_num11, roll_num12,
            roll_num13, roll_num14, roll_num15, roll_num16, roll_num17, roll_num18,
            roll_num19, roll_num20, roll_num21, roll_num22, roll_num23, roll_num24,
            roll_num25, roll_num26, roll_num27
        };
        }
    }
}
