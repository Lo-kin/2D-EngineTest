using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace _WPF_RPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GameProperties GP = new GameProperties();
            this.Width = GP.ScreenWidth;
            this.Height = GP.ScreenHeight;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;

            GUIStart();
        }

        bool[] KeyPressStat = new bool[4];
        DateTime[] KeyPressTime = new DateTime[4];
        bool[] KeyExcuteStat = new bool[] { true , true, true, true};
        double[] KeyPreesSpan = new double[] {0 , 0 , 0 , 0 };
        
        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            DateTime EndTime = DateTime.Now;
            if (e.Key == Key.A)
            {
                KeyPressStat[0] = false;
            }
            if (e.Key == Key.W)
            {
                KeyPressStat[1] = false;
            }
            if (e.Key == Key.D)
            {
                KeyPressStat[2] = false;
            }
            if (e.Key == Key.S)
            {
                KeyPressStat[3] = false;
            }

        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DateTime StartTime = DateTime.Now;
            if (e.Key == Key.A && KeyPressStat[0] == false)
            {
                //如果是刚按下按键就初始化
                KeyPressStat[0] = true;
                KeyPreesSpan[0] = 0;
                KeyPressTime[0] = StartTime;
            }
            else
            {
                //判断按键是否被处理
                if (KeyExcuteStat[0] == true)
                {
                    KeyPreesSpan[0] = 0;
                }
                //如果不是就计算时间并使处理为假
                
                TimeSpan KeyASpan = StartTime - KeyPressTime[0];
                KeyPressTime[0] = StartTime;
                KeyPreesSpan[0] += KeyASpan.TotalMilliseconds;
                KeyExcuteStat[0] = false;
            }

            if (e.Key == Key.W && KeyPressStat[1] == false)
            {
                KeyPressStat[1] = true;
                KeyPreesSpan[1] = 0;
                KeyPressTime[1] = StartTime;
            }
            else
            {
                if (KeyExcuteStat[1] == true)
                {
                    KeyPreesSpan[1] = 0;
                }
                
                TimeSpan KeyWSpan = StartTime - KeyPressTime[1];
                KeyPressTime[1] = StartTime;
                KeyPreesSpan[1] += KeyWSpan.TotalMilliseconds;
                KeyExcuteStat[1] = false;
            }

            if (e.Key == Key.D && KeyPressStat[2] == false)
            {
                KeyPressStat[2] = true;
                KeyPreesSpan[2] = 0;
                KeyPressTime[2] = StartTime;
            }
            else
            {
                if (KeyExcuteStat[2] == true)
                {
                    KeyPreesSpan[2] = 0;
                }
                TimeSpan KeyDSpan = StartTime - KeyPressTime[2];
                KeyPressTime[2] = StartTime;
                KeyPreesSpan[2] += KeyDSpan.TotalMilliseconds;
                KeyExcuteStat[2] = false;
            }

            if (e.Key == Key.S && KeyPressStat[3] == false)
            {
                KeyPressStat[3] = true;
                KeyPreesSpan[3] = 0;
                KeyPressTime[3] = StartTime;
            }
            else
            {
                if (KeyExcuteStat[3] == true)
                {
                    KeyPreesSpan[3] = 0;
                }
                TimeSpan KeySSpan = StartTime - KeyPressTime[3];
                KeyPressTime[3] = StartTime;
                KeyPreesSpan[3] += KeySSpan.TotalMilliseconds;
                KeyExcuteStat[3] = false;
            }


        }

        private object[,] ExcuteMenuPos()
        {
            return new object[,] { };
        }

        private void GUIStart()
        {
            /*
                绘图大纲
                    背景画布 （BGCanvas）
                    {
                        菜单画布 （MainMenu）
                        绘图画布 （MainCanvas）
                    }
                    在载入绘图时就使 菜单 隐藏或删除
            */

            //定义一个背景画布
            Canvas BGCanvas = new Canvas();
            this.Content = BGCanvas;
            this.RegisterName("BGCanvas", BGCanvas);

            Canvas MainMenu = new Canvas();
            BGCanvas.Children.Add(MainMenu);
            BGCanvas.RegisterName("MenuCanvas", MainMenu);
            string[,] MenuControlsList = new string[,] {
                {"Start" , "Setting" , "Exit" }
            };
            Button RegularStart = new Button();
            RegularStart.Width = 80;
            RegularStart.Height = 60;
            RegularStart.SetValue(Canvas.LeftProperty, (double)400 - 40);
            RegularStart.SetValue(Canvas.TopProperty, (double)300 - 30);
            RegularStart.Click += new RoutedEventHandler(LoadPainter);
            MainMenu.Children.Add(RegularStart);

        }


        private void LoadPainter(object sender , EventArgs e)
        {
            //获得绘制属性
            GameProperties GP = new GameProperties();
            double[] PaintGP = GP.CalBC();
            //绘制属性
            int PBSY = (int)PaintGP[2];
            int PBEY = (int)PaintGP[3];
            int PBSX = (int)PaintGP[0];
            int PBEX = (int)PaintGP[1];
            //int PainterW = (int)PaintGP[4];
            //int PainterH = (int)PaintGP[5];
            
            //单个方块的属性
            int BlockWidth = GameProperties.BlockWidth;
            int BlockHeight = GameProperties.BlockHeight;

            //定义画布并添加到背景画布
            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = new Canvas();
            BGCanvas.Children.Remove(BGCanvas.FindName("MenuCanvas") as UIElement);
            BGCanvas.Children.Add(MainCanvas);
            BGCanvas.RegisterName("PaintCanvas" , MainCanvas);
            BGCanvas.Background = new SolidColorBrush(Color.FromRgb(100, 150, 100));
            //初始化绘图位置
            MainCanvas.SetValue(Canvas.LeftProperty, 0d);
            MainCanvas.SetValue(Canvas.TopProperty, 0d);

            //定义一个动态大小数组储存绘制信息
            //object[,] PreLoadPainter = new object[PainterW , PainterH];

            
            SolidColorBrush DefaultColor = new SolidColorBrush(Color.FromRgb(150 , 150 , 150));
            Random r = new Random();
            Random g = new Random();
            Random b = new Random();
            
            for (int BlY = PBSY; BlY <= PBEY; BlY++)
            {
                for (int BlX = PBSX; BlX <= PBEX; BlX++)
                {

                    Rectangle NowPaintBlcok = new Rectangle();
                    NowPaintBlcok.Width = BlockWidth;
                    NowPaintBlcok.Height = BlockHeight;
                    NowPaintBlcok.Fill = new SolidColorBrush(Color.FromRgb((byte)r.Next(0,255) , (byte)g.Next(0, 255), (byte)b.Next(0, 255)));
                    NowPaintBlcok.SetValue(Canvas.LeftProperty, (double)BlX * BlockWidth);
                    NowPaintBlcok.SetValue(Canvas.TopProperty, (double)BlY * BlockHeight);
                    MainCanvas.Children.Add(NowPaintBlcok);
                    //在注册名时不能带有特殊符号，例如-
                    string NX;
                    string NY;
                    if (BlX <= 0) { NX = "_" + -BlX ; }
                    else { NX = Convert.ToString(BlX); }
                    if (BlY <= 0) { NY = "_" + -BlY ; }
                    else { NY = Convert.ToString(BlY); }
                    string RectName = "x" + NX + "y" + NY;
                    MainCanvas.RegisterName(RectName, NowPaintBlcok);
                }
            }
            test();

        }

        private void test()
        {
            Painter();
        }

        private void Painter()
        {
            //获得绘制属性
             GameProperties GP = new GameProperties();
            
            double[] PaintGP = GP.CalBC();
            //绘制属性
            int PBSY = (int)PaintGP[2];
            int PBEY = (int)PaintGP[3];
            int PBSX = (int)PaintGP[0];
            int PBEX = (int)PaintGP[1];
            //单个方块的属性
            int BlockWidth = GameProperties.BlockWidth;
            int BlockHeight = GameProperties.BlockHeight;

            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;
            double spd = 0.01;
            double CamMoveX = 0;
            double CamMoveY = 0;

            double NewRegionX = 0;
            double NewRegionY = 0;

            for (;;)
            {
                //判断按键是否按下并判断处理状况
                if (KeyPressStat[0] == true && KeyExcuteStat[0] == false)
                {
                    //获取按键时间，并标记事件已处理
                    double MTtoIntL = KeyPreesSpan[0];
                    CamMoveX += MTtoIntL * spd;
                    KeyExcuteStat[0] = true;
                }
                if (KeyPressStat[1] == true && KeyExcuteStat[1] == false)
                {
                    double MTtoIntF = KeyPreesSpan[1];
                    CamMoveY += MTtoIntF * spd;
                    KeyExcuteStat[1] = true;
                }
                if (KeyPressStat[2] == true && KeyExcuteStat[2] == false)
                {
                    double MTtoIntR = KeyPreesSpan[2];
                    CamMoveX -= MTtoIntR * spd;
                    KeyExcuteStat[2] = true;
                }
                if (KeyPressStat[3] == true && KeyExcuteStat[3] == false)
                {
                    double MTtoIntB = KeyPreesSpan[3];
                    CamMoveY -= MTtoIntB * spd;
                    KeyExcuteStat[3] = true;
                }

                if (CamMoveX >= BlockWidth)
                {

                }

                MainCanvas.SetValue(Canvas.LeftProperty, CamMoveX);
                MainCanvas.SetValue(Canvas.TopProperty, CamMoveY);

                DoEvents();
                
            }
        }

        public void DoEvents()
        {

            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate (object f)
            {
                ((DispatcherFrame)f).Continue = false;
                return null;
            }), frame);
            Dispatcher.PushFrame(frame);

        }

        private void LoadCharacters()
        {

        }
    }

    class GameProperties
    {
        //窗口属性
        public int ScreenWidth = 800;
        public int ScreenHeight = 600;

        //绘制属性
        public const int BlockWidth = 32;
        public const int BlockHeight = 32;

        //视角属性
        double NowCameraPosX = 0;
        double NowCameraPosY = 0;

        //菜单属性
        float MaxUseRegion = 0.8F;
        double MenuDistance = 10;
        //
        int MenuWidth = 150;
        int MenuHeight = 15;

        public object[] MenuProperties
        {
            get { return new object[] { MaxUseRegion, MenuDistance ,MenuWidth , MenuHeight}; }
        }

        public double NCPX 
        { 
            get { return NowCameraPosX; }
            set { NowCameraPosX = value; }
        }
        public double NCPY
        {
            get { return NowCameraPosY; }
            set { NowCameraPosY = value; }
        }

        public int[] RetGPs() 
        {
            return new int[] { };
        }

        public double[] CalBC()
        {
            //预加载方块数，保证低性能可容纳
            int PreLoadBlcoks = 4;
            //计算屏幕可容纳方块数量，确定绘制始末点
            double BCXStart = - PreLoadBlcoks;
            double BCXEnd = Math.Ceiling((double)(ScreenWidth / BlockWidth)) + PreLoadBlcoks;
            double BCXCount = BCXEnd - BCXStart;

            double BCYStart = - PreLoadBlcoks;
            double BCYEnd = Math.Ceiling((double)(ScreenHeight / BlockHeight)) + PreLoadBlcoks;
            double BCYCount = BCYEnd - BCYStart;

            return new double[] { BCXStart , BCXEnd , BCYStart , BCYEnd  , BCXCount , BCYCount};
        }

    }
}
