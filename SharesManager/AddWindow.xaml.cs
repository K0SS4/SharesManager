using System.Text.RegularExpressions;
using System.Windows;

namespace SharesManager
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public ShareProperties Prop { get; set; } = new ShareProperties();
        private AddWindow()
        {
            InitializeComponent();
            XtextName.Focus();
            Title = "Add share";
        }

        private AddWindow(ShareProperties prop)
        {
            InitializeComponent();

            Title = "Edit share";

            XtextName.Text = prop.Name;
            XtextIp.Text = prop.Ip;
            XtextShare.Text = prop.Share;
            XcheckVpn.IsChecked = prop.Vpn;

            XtextName.Focus();

            XbuttonAdd.Content = "Save";
        }

        private void XbuttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(XtextIp.Text)) return;

            string pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            Regex check = new Regex(pattern);
            if (check.IsMatch(XtextIp.Text, 0))
            {
                Prop.Name = XtextName.Text;
                Prop.Ip = XtextIp.Text;
                Prop.Share = XtextShare.Text;
                Prop.Vpn = XcheckVpn.IsChecked ?? false;

                Close();
            }
            else
            {
                MessageBox.Show("Invalid IP address", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static ShareProperties AddProp()
        {
            AddWindow add = new AddWindow();
            add.ShowDialog();

            return add.Prop;
        }

        public static ShareProperties AddProp(ShareProperties prop)
        {
            AddWindow add = new AddWindow(prop);
            add.ShowDialog();

            return add.Prop;
        }

        private void EnterPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            XbuttonAdd.Focus();
            XbuttonAdd_Click(sender, new RoutedEventArgs());
        }
    }
}
