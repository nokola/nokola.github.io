using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
*	A Colourful Firework Demonstratoin in C#
*   from shinedraw.com
*/

namespace Shock
{
    public partial class Fireworks : UserControl
    {
        private static double FIREWORK_NUM = 2;            // Number of Dot generated each time
        private static double GRAVITY = 0.3;               // Gravity
        private static double X_VELOCITY = 5;              // Maximum X Velocity
        private static double Y_VELOCITY = 5;              // Maximum Y Velocity
        private static int SIZE_MIN = 2;                   // Minimum Size
        private static int SIZE_MAX = 4;                   // Maximum Size

        private List<MagicDot> _fireworks = new List<MagicDot>();

        private DispatcherTimer _timer;                // on enter frame simulator
        private static int FPS = 24;                  // fps of the on enter frame event

        private TimeSpan _totalDuration;
        private DateTime _startTime;

        private int _width;
        private int _height;
        private int _width2;
        private int _height2;
        private bool _shadeOfWhite = false;

        public Fireworks()
        {
            InitializeComponent();

            List<Object> array = new List<Object>();
            Object abc = new Object();
            _startTime = DateTime.Now;
        }

        public Fireworks(TimeSpan totalDuration, int width, int height, bool shadeOfWhite)
            : this()
        {
            _totalDuration = totalDuration;
            _width = width;
            _height = height;
            _width2 = width / 2;
            _height2 = height / 2;
            _shadeOfWhite = shadeOfWhite;
        }

        bool _isStopping = false;
        void _timer_Tick(object sender, EventArgs e)
        {
            if (_isStopping)
            {
                if (_fireworks.Count == 0)
                {
                    _timer.Stop();
                    ((Panel)Parent).Children.Remove(this);
                    LayoutRoot.Children.Clear();
                }
                else
                {
                    MoveFirework();
                }
                return;
            }

            if ((DateTime.Now - _startTime) > _totalDuration)
            {
                _isStopping = true;
                return;
            }

            Emit();
            MoveFirework();
        }

        void MoveFirework()
        {
            for (int i = _fireworks.Count - 1; i >= 0; i--)
            {
                MagicDot dot = _fireworks[i];
                dot.RunFirework();
                if (dot.Opacity <= 0.1)
                {
                    LayoutRoot.Children.Remove(dot);
                    _fireworks.Remove(dot);
                }
            }
        }

        private void Emit()
        {
            AddFirework(Defaults.RandomGenerator.Next(_width) - _width2, Defaults.RandomGenerator.Next(_height) - _height2);
        }

        void AddFirework(double x, double y)
        {
            for (int i = 0; i < FIREWORK_NUM; i++)
            {
                double size = SIZE_MIN + (SIZE_MAX - SIZE_MIN) * Defaults.RandomGenerator.NextDouble();
                byte red = (byte)(128 + Defaults.RandomGenerator.Next(128));
                byte green;
                byte blue;

                if (_shadeOfWhite)
                {
                    green = red;
                    blue = red;
                }
                else {
                    green = (byte)(128 + Defaults.RandomGenerator.Next(128));
                    blue = (byte)(128 + Defaults.RandomGenerator.Next(128));
                }

                double xVelocity = X_VELOCITY - 2 * X_VELOCITY * Defaults.RandomGenerator.NextDouble();
                double yVelocity = -Y_VELOCITY * Defaults.RandomGenerator.NextDouble();

                MagicDot dot = new MagicDot(red, green, blue, size);
                dot.X = x;
                dot.Y = y;
                dot.XVelocity = xVelocity;
                dot.YVelocity = yVelocity;
                dot.Gravity = GRAVITY;
                dot.RunFirework();
                _fireworks.Add(dot);

                LayoutRoot.Children.Add(dot);
            }
        }

        bool _hasStarted = false;
        public void Start()
        {
            if (_hasStarted)
            {
                return;
            }
            _hasStarted = true;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / FPS);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }
    }
}
