using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Lógica de interacción para MissatgeUC.xaml
    /// </summary>
    public partial class MissatgeUC : UserControl
    {
        public MissatgeUC()
        {
            InitializeComponent();
        }

        public Missatge myMessage
        {
            get { return (Missatge)GetValue(myMessageProperty); }
            set { SetValue(myMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for myMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty myMessageProperty =
            DependencyProperty.Register("myMessage", typeof(Missatge), typeof(MissatgeUC), new PropertyMetadata(null));


    }
}
