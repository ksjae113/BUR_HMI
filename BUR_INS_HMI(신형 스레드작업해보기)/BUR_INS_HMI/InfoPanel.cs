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
        public Label[] roll_num;
        public Label[] roll_rpm;
        public Panel[] roll;
        public InfoPanel()
        {
            InitializeComponent();

            roll_num = new Label[] {
            roll_num1, roll_num2, roll_num3, roll_num4, roll_num5, roll_num6,
            roll_num7, roll_num8, roll_num9, roll_num10, roll_num11, roll_num12,
            roll_num13, roll_num14, roll_num15, roll_num16, roll_num17, roll_num18,
            roll_num19, roll_num20, roll_num21, roll_num22, roll_num23, roll_num24,
            roll_num25, roll_num26, roll_num27
        };

            roll_rpm = new Label[]
            {
                roll_rpm1,roll_rpm2,roll_rpm3,roll_rpm4,roll_rpm5,roll_rpm6,roll_rpm7,roll_rpm8,
                roll_rpm9,roll_rpm10,roll_rpm11,roll_rpm12,roll_rpm13,roll_rpm14,roll_rpm15,roll_rpm16,
                roll_rpm17,roll_rpm18,roll_rpm19,roll_rpm20,roll_rpm21,roll_rpm22,roll_rpm23,roll_rpm24,
                roll_rpm25,roll_rpm26,roll_rpm27
            };

            roll = new Panel[] { roll1,roll2,roll3,roll4,roll5,roll6,roll7,roll8,roll9,
            roll10,roll11,roll12,roll13,roll14,roll15, roll16,roll17,roll18,roll19,
            roll20,roll21,roll22,roll23,roll24,roll25,roll26,roll27};
        }
    }
}
