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
using System.Windows.Threading;

namespace KeyboardTrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _capsFlag;
        private bool _backSpaceFlag;
        private int _speedEnterChar;
        private int _countFails;
        private int _countCharInput;
        private int _accurate;
        private int _correctInput;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            InitTimer();
            Reset();
        }

        private void Reset()
        {
            TextUser.IsReadOnly = true;
            TextUser.Text = "";
            TrainerText.Text = "";
            CountFails.Text = "0";
            AccurateTextBlock.Text = "0";
            SpeedEnterValue.Text = "0";
            _capsFlag = false;
            _backSpaceFlag = false;
            _speedEnterChar = 0;
            _countFails = 0;
            _countCharInput = 0;
            _accurate = 0;
            _correctInput = 0;
        }
        private void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _speedEnterChar++;
            MeasureSpeed();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            ChangOpacityKey(e);
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                ChangingTheLayout();
            }
        }

        private void ChangingTheLayout()
        {
            (_capsFlag, LowerCase.Visibility, UpperCase.Visibility) =
                (!_capsFlag, UpperCase.Visibility, LowerCase.Visibility);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            GenratingText();
            TextUser.IsEnabled = true;
            TextUser.IsReadOnly = false;
            TextUser.Focus();
            _timer.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _speedEnterChar = 0;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            Reset();
        }

        private void LineUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_backSpaceFlag)
            {
                _countCharInput++;
            }

            string CheckedString = TrainerText.Text.Substring(0, TextUser.Text.Length);

            if (TextUser.Text.Equals(CheckedString))
            {
                TextUser.Background = Brushes.LightGreen;
                if (!_backSpaceFlag)
                {
                    _correctInput++;
                    MeasureSpeed();
                }
            }
            else
            {
                if (!_backSpaceFlag)
                {
                    _countFails++;
                    CountFails.Text = _countFails.ToString();
                }
                TextUser.Background = Brushes.Red;
            }

            if (TextUser.Text.Length == TrainerText.Text.Length)
            {
                _timer.Stop();
                TextUser.IsReadOnly = true;
                MessageBox.Show($"Всего символов введено: {_countCharInput}\n" +
                    $"Скорость ввода: {SpeedEnterValue.Text} символов в минуту\n" +
                    $"Допущено ошибок: {_countFails}\n" +
                    $"Точность ввода: {_accurate}",
                    "Сообщение",
                     MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void MeasureSpeed()
        {
            if (_correctInput > 0)
            {
                _accurate = 100 * _correctInput / _countCharInput;
            }
            AccurateTextBlock.Text = _accurate.ToString();

            SpeedEnterValue.Text = Math.Round((double)_countCharInput / _speedEnterChar * 60).ToString();
        }

        private void SliderDifficulty_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Difficulty.Text = ((int)SliderDifficulty.Value).ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SliderDifficulty.Maximum = 99;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SliderDifficulty.Maximum = 50;
        }

        private void GenratingText()
        {
            var rnd = new Random();
            var baseString = "QWERTYUIOPASDFGHJKLZXCVBNM~!@#$%^&*()_+{}|:\"<>?1234567890[],./\\`-=;'qwertyuiopasdfghjklzxcvbnm";
            int ptrStart = 47;
            if ((bool)CaseSensitiveCheckBox.IsChecked)
            {
                ptrStart = 0;
            }
            for (int i = 0; i < (int)SliderDifficulty.Value; i++)
            {
                TrainerText.Text += baseString[rnd.Next(ptrStart, baseString.Length)];
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                ChangOpacityKey(e);
            }
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift || e.Key == Key.CapsLock)
            {
                if (!e.IsRepeat)
                {
                    ChangingTheLayout();
                }
            }
            else if (e.Key == Key.Back)
            {
                _backSpaceFlag = true;
            }
            else
            {
                _backSpaceFlag = false;
            }
        }
        private void ChangOpacityKey(KeyEventArgs e)
        {
            foreach (StackPanel item in UpperCase.Children)
            {
                foreach (Grid item2 in item.Children)
                {
                    foreach (Border bor in item2.Children)
                    {
                        var tb = bor.Child as TextBlock;
                        if (tb.Text == e.Key.ToString())
                        {
                            if (bor.Opacity == 1)
                            {
                                bor.Opacity = 0.5;
                            }
                            else
                            {
                                bor.Opacity = 1;
                            }
                        }
                    }
                }
            }

            foreach (StackPanel item in LowerCase.Children)
            {
                foreach (Grid item2 in item.Children)
                {
                    foreach (Border bor in item2.Children)
                    {
                        var tb = bor.Child as TextBlock;
                        if (tb.Text.ToUpper() == e.Key.ToString())
                        {
                            if (bor.Opacity == 1)
                            {
                                bor.Opacity = 0.5;
                            }
                            else
                            {
                                bor.Opacity = 1;
                            }
                        }
                    }
                }
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            
        }
    }
}
