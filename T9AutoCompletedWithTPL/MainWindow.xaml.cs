using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace T9AutoCompletedWithTPL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> list { get; set; } = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            //To read from a database(sqlite)
            /*var connString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            using (var conn = new SQLiteConnection(connString))
            {
                DataTable dt = new DataTable();

                string sql = "Select * from Expressions";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
                adapter.Fill(dt);

                foreach (DataRow item in dt.Rows)
                {
                    string row = item["Expression"].ToString();
                    list.Add(row);
                }
            }*/


            //To read from a text file
            list = File.ReadAllLines(Directory.GetParent(Environment.CurrentDirectory).GetDirectories()[0].FullName + @"\Words.txt", Encoding.UTF8).ToList();
        }

        private void TxbWriting_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txbWriting.SelectionLength == 0)
                _deleteText = null;

            if (tglBtnAutoCompleted.IsChecked == true)
            {
                foreach (string word in list)
                {
                    if (!string.IsNullOrWhiteSpace(txbWriting.Text))
                    {
                        if (word.StartsWith(txbWriting.Text))
                        {
                            string writedText = txbWriting.Text;
                            string completedText = word.Substring(writedText.Length);
                            txbWriting.Text = word;
                            txbWriting.Select(writedText.Length, completedText.Length);
                            _deleteText = txbWriting.SelectedText;
                            break;
                        }
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txbWriting.Text = "";
        }
        string _deleteText = "";
        private void TxbWriting_KeyDown(object sender, KeyEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (e.Key == Key.Left)
                    {
                        tglBtnAutoCompleted.IsChecked = false;
                        txbWriting.Text = txbWriting.Text.Replace(_deleteText, null);
                        txbWriting.CaretIndex = txbWriting.Text.Length;
                    }
                });
            });
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (txbWriting.SelectionLength != 0)
                    {
                        txbWriting.CaretIndex = txbWriting.Text.Length;
                        txbWriting.Focus();
                    }
                    else
                    {
                        txbWriting.CaretIndex += 1;
                        txbWriting.Focus();
                    }
                });
            });
        }
        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (txbWriting.SelectionLength != 0)
                    {
                        tglBtnAutoCompleted.IsChecked = false;
                        txbWriting.Text = txbWriting.Text.Replace(_deleteText, null);
                        txbWriting.CaretIndex = txbWriting.Text.Length;
                        txbWriting.Focus();
                    }
                    else
                    {
                        if (txbWriting.CaretIndex != 0)
                            txbWriting.CaretIndex -= 1;
                        txbWriting.Focus();
                    }
                });
            });
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            txbWriting.CaretIndex = 0;
            txbWriting.Focus();
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            txbWriting.CaretIndex = txbWriting.Text.Length;
            txbWriting.Focus();
        }

        private void TglBtnAutoCompleted_Click(object sender, RoutedEventArgs e) => txbWriting.Focus();

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (string.IsNullOrEmpty(_deleteText) && !(string.IsNullOrWhiteSpace(txbWriting.Text)))
                    {

                        //-----------------------------------------------------------------------------------------------------------------------------------------
                        // File add
                        string path = Directory.GetParent(Environment.CurrentDirectory).GetDirectories()[0].FullName + @"\Words.txt";
                        if (File.ReadAllLines(path).ToList().Exists(w => w != txbWriting.Text))
                        {
                            File.AppendAllText(path, "\n" + txbWriting.Text, Encoding.UTF8);
                            list.Add(txbWriting.Text);
                            MessageBox.Show("A new expression has been successfully added to the dictionary!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txbWriting.Text = string.Empty;
                        }
                        else
                            MessageBox.Show("This expression is already in the database!", "", MessageBoxButton.OK, MessageBoxImage.Warning);


                        //-----------------------------------------------------------------------------------------------------------------------------------------


                        // Database add
                        /*if (!list.Contains(txbWriting.Text))
                        {
                            var connString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
                            using (var conn = new SQLiteConnection(connString))
                            {
                                conn.Open();
                                string sql = "INSERT INTO Expressions(Expression) VALUES(?)";
                                SQLiteCommand sQLiteCommand = new SQLiteCommand(sql, conn);
                                sQLiteCommand.Parameters.AddWithValue("param1", txbWriting.Text);
                                sQLiteCommand.ExecuteNonQuery();
                                list.Add(txbWriting.Text);
                                MessageBox.Show("A new expression has been successfully added to the dictionary!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                txbWriting.Text = string.Empty;
                            }
                        }
                        else
                            MessageBox.Show("This expression is already in the database!", "", MessageBoxButton.OK, MessageBoxImage.Warning);*/
                        //-----------------------------------------------------------------------------------------------------------------------------------------
                    }
                    else
                        MessageBox.Show("Enter the word correctly!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            });
        }

        private void BtnNumpad_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            txbWriting.Text += $"{btn.Content}";
        }
    }

}
