using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SharesManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadData();
        }

        public ObservableCollection<ShareProperties> Shares { get; set; } = new ObservableCollection<ShareProperties>();
        private int _counter = 1;

        private void XbuttonAdd_Click(object sender, RoutedEventArgs e)
        {
            ShareProperties prop = AddWindow.AddProp();
            if (string.IsNullOrEmpty(prop.Ip)) return;

            prop.Id = _counter;
            Shares.Add(prop);
            _counter++;
            SaveData();
        }

        private void XbuttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (XdataShares.SelectedIndex == -1) return;
            Shares.Remove(Shares[XdataShares.SelectedIndex]);
            SaveData();
        }

        private void XbuttonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (XdataShares.SelectedIndex == -1) return;

            int index = XdataShares.SelectedIndex;
            string path = "\\\\" + Shares[index].Ip + "\\" + Shares[index].Share;
            Process.Start("cmd.exe","/C start " + path);
        }

        private void XbuttonEdit_Click(object sender, RoutedEventArgs e)
        {
            int index = XdataShares.SelectedIndex;
            if (index == -1) return;

            ShareProperties prop = AddWindow.AddProp(Shares[index]);
            if (string.IsNullOrEmpty(prop.Ip)) return;

            Shares[index].Name = prop.Name;
            Shares[index].Ip = prop.Ip;
            Shares[index].Share = prop.Share;
            Shares[index].Vpn = prop.Vpn;

            XdataShares.Items.Refresh();
            SaveData();
        }

        public void SaveData()
        {
            string fileName = "shares.dat";
            using (var stream = File.Open(fileName, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    string key = RandomString();
                    writer.Write(key);
                    foreach (ShareProperties prop in Shares)
                    {
                        writer.Write(prop.Id);
                        writer.Write(EncryptOrDecrypt(prop.Name, key));
                        writer.Write(EncryptOrDecrypt(prop.Ip, key));
                        writer.Write(EncryptOrDecrypt(prop.Share, key));
                        writer.Write(prop.Vpn);
                    }
                }
            }
        }

        public void LoadData()
        {
            string fileName = "shares.dat";

            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        string key = reader.ReadString();
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            int id = reader.ReadInt32();
                            string name = EncryptOrDecrypt(reader.ReadString(), key);
                            string ip = EncryptOrDecrypt(reader.ReadString(), key);
                            string share = EncryptOrDecrypt(reader.ReadString(), key);
                            bool vpn = reader.ReadBoolean();
                            Shares.Add(new ShareProperties
                            {
                                Id = id,
                                Name = name,
                                Ip = ip,
                                Share = share,
                                Vpn = vpn
                            });
                        }
                    }
                }

                if (Shares.Any())
                    _counter = Shares.Last().Id + 1;
            }
        }

        private void XdataShares_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (XdataShares.SelectedIndex == -1) return;

            XbuttonOpen_Click(sender, new RoutedEventArgs());
        }

        private string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char) ((uint) text[c] ^ (uint) key[c % key.Length]));

            return result.ToString();
        }

        private string RandomString()
        {
            Random random = new Random();
            int length = random.Next(25, 30);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
