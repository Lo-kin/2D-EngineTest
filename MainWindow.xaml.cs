using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
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
            PaintProperties GP = new PaintProperties();
            this.Width = GP.ScreenWidth;
            this.Height = GP.ScreenHeight;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
            this.PreviewKeyUp += MainWindow_PreviewKeyUp;
            GUIStart();
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
            RegularStart.Click += new RoutedEventHandler(MajorLoader);
            MainMenu.Children.Add(RegularStart);
        }

        private void MajorLoader(object sender, EventArgs e)
        {
            Thread KExc = new Thread(new ThreadStart(KeyExcuter));
            KExc.IsBackground = true;
            KExc.Start();
            Server Sv = new Server();
            Map = Sv.Init(null);
            LoadPainter();
            Painter();

        }

        object[] Map = new object[4] ;

        private void LoadPainter()
        {
            //获得绘制属性
            PaintProperties GP = new PaintProperties();
            double[] PaintGP = GP.CalBC(0d ,0d);
            //绘制属性
            int PBSY = (int)PaintGP[2];
            int PBEY = (int)PaintGP[3];
            int PBSX = (int)PaintGP[0];
            int PBEX = (int)PaintGP[1];

            //单个方块的属性
            int BlockWidth = PaintProperties.BlockWidth;
            int BlockHeight = PaintProperties.BlockHeight;

            //定义画布并添加到背景画布
            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = new Canvas();
            BGCanvas.Children.Remove(BGCanvas.FindName("MenuCanvas") as UIElement);
            BGCanvas.Children.Add(MainCanvas);
            BGCanvas.RegisterName("PaintCanvas", MainCanvas);
            BGCanvas.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //初始化绘图位置
            MainCanvas.SetValue(Canvas.LeftProperty, 0d);
            MainCanvas.SetValue(Canvas.TopProperty, 0d);

            GameArgs[2] = PaintGP;

            int yIndex = -1;
            int xIndex = -1;
            for (int BlY = PBSY; BlY <= PBEY; BlY++)
            {
                yIndex++;
                for (int BlX = PBSX; BlX <= PBEX; BlX++)
                {
                    xIndex++;
                    byte[] BColor = (byte[])((object[])((object[,])Map[3])[xIndex , yIndex ])[1];

                    Rectangle NowPaintBlcok = new Rectangle();
                    NowPaintBlcok.Width = BlockWidth;
                    NowPaintBlcok.Height = BlockHeight;
                    NowPaintBlcok.Fill = new SolidColorBrush(Color.FromRgb(BColor[0], BColor[1], BColor[2]));
                    NowPaintBlcok.SetValue(Canvas.LeftProperty, (double)BlX * BlockWidth);
                    NowPaintBlcok.SetValue(Canvas.TopProperty, (double)BlY * BlockHeight);
                    MainCanvas.Children.Add(NowPaintBlcok);
                    //在注册名时不能带有特殊符号，例如-
                    string NX;
                    string NY;
                    if (BlX <= 0) { NX = "_" + -BlX; }
                    else { NX = Convert.ToString(BlX); }
                    if (BlY <= 0) { NY = "_" + -BlY; }
                    else { NY = Convert.ToString(BlY); }
                    string RectName = "x" + NX + "y" + NY;
                    MainCanvas.RegisterName(RectName, NowPaintBlcok);
                }
            }

        }

        object[] GameArgs = new object[] { 0d, 0d , 1};
        double[] MapShift = new double[] { 0d, 0d };

        private void Painter()
        {
            //获得绘制属性
            PaintProperties PP = new PaintProperties();

            //视角属性
            double[] CamPosPak = PP.CamProperties;
            int BlockFlush = PP.PreLoadBlcoks;

            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;
            InPaintUI();
            Label DebugMsg = BGCanvas.FindName("DebugMsg") as Label;

            Rectangle r = new Rectangle();
            r.Width = 10;
            r.Height = 10;
            r.Fill = new SolidColorBrush(Color.FromRgb((byte)0 , (byte)0 , (byte)0));
            r.SetValue(Canvas.LeftProperty, 405d);
            r.SetValue(Canvas.TopProperty, 305d);
            BGCanvas.Children.Add(r);

            for (; ; )
            {
                if (KeyValue[4] == 1) { break; }

                MainCanvas.SetValue(Canvas.LeftProperty, MapShift[0]);
                MainCanvas.SetValue(Canvas.TopProperty, MapShift[1]);
                DebugMsg.Content = "CamX:" + GameArgs[0] + "\n" + "CamY:" + GameArgs[1];


                DoEvents();
                Thread.Sleep(5);
            }
            Environment.Exit(0);
        }

        private double[] ChangePaintRegion()
        {
            PaintProperties PP = new PaintProperties();

            double CamMoveX = KeyValue[0] + KeyValue[2];
            double CamMoveY = KeyValue[1] + KeyValue[3];
            double ScrMinX = CamMoveX - (PP.ScreenWidth / 2);
            double ScrMinY = CamMoveY - (PP.ScreenHeight / 2);


            return new double[] { };
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

        public void InPaintUI()
        {
            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;

            //Health Bar
            Rectangle HBBG = new Rectangle();
            HBBG.Width = 200;
            HBBG.Height = 20;
            HBBG.Fill = new SolidColorBrush(Color.FromRgb(0 , 0 , 0));
            HBBG.SetValue(Canvas.LeftProperty, 500d);
            HBBG.SetValue(Canvas.TopProperty, 50d);
            Rectangle HBPerc = new Rectangle();
            HBPerc.Width = 200;
            HBPerc.Height = 20;
            HBPerc.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            HBPerc.SetValue(Canvas.LeftProperty, 500d);
            HBPerc.SetValue(Canvas.TopProperty, 50d);
            BGCanvas.RegisterName("HBBG" , HBBG);
            BGCanvas.RegisterName("HBPerc" , HBPerc);
            BGCanvas.Children.Add(HBBG);
            BGCanvas.Children.Add(HBPerc);

            Label DebugMsg = new Label();
            DebugMsg.Width = 800;
            DebugMsg.Height = 600;
            DebugMsg.SetValue(Canvas.LeftProperty, 10d);
            DebugMsg.SetValue(Canvas.TopProperty, 10d);
            BGCanvas.RegisterName("DebugMsg", DebugMsg);
            BGCanvas.Children.Add(DebugMsg);

            TextBox DBox = new TextBox();
            DBox.Background = new SolidColorBrush(Color.FromArgb(20, 20, 20, 20));
            DBox.Height = 16;
            DBox.Width = 500;
            DBox.SetValue(Canvas.LeftProperty, 5d);
            DBox.SetValue(Canvas.TopProperty, 500d);
            BGCanvas.Children.Add(DBox);
            BGCanvas.RegisterName("DBox", BGCanvas);
        }

        private object[,] ExcuteMenuPos()
        {
            return new object[,] { };
        }

        bool[] KeyPressStat = new bool[5];
        double[] KeyValue = new double[5] { 0, 0, 0, 0, -1 };

        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
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
            if (e.Key == Key.Escape)
            {
                KeyPressStat[4] = false;
            }

        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DateTime StartTime = DateTime.Now;
            if (e.Key == Key.A)
            {
                KeyPressStat[0] = true;
            }
            if (e.Key == Key.W)
            {
                KeyPressStat[1] = true;
            }
            if (e.Key == Key.D)
            {
                KeyPressStat[2] = true;
            }
            if (e.Key == Key.S)
            {
                KeyPressStat[3] = true;
            }
            if (e.Key == Key.Escape)
            {
                KeyPressStat[4] = true;
            }
        }

        private void MapShiftChecker()
        {
            while (true)
            {

                if (MapShift[0] >= 5d || MapShift[0] <= -5)

                Thread.Sleep(1);
            }
        }

        public void KeyExcuter()
        {
            double spd = 0.12;
            //每个按键对应特殊的算法
            object[] KeyGrade = new object[] {
                new object[] {spd , "+"},
                new object[] {spd , "+"},
                new object[] {spd , "-"},
                new object[] {spd , "-"},
                new object[] {-1.0 , "*"}
            };

            PaintProperties PP = new PaintProperties();


            while (true)
            {
                for (int Num = 0; Num < KeyPressStat.Length; Num++)
                {
                    object[] ExcCond = (object[])KeyGrade[Num];
                    double ExcVar = (double)ExcCond[0];
                    string ExcSyb = (string)ExcCond[1];

                    if (KeyPressStat[Num] == true)
                    {
                        if (ExcSyb == "+")
                        {
                            KeyValue[Num] += ExcVar;
                        }
                        if (ExcSyb == "-")
                        {
                            KeyValue[Num] -= ExcVar;
                        }
                        if (ExcSyb == "*")
                        {
                            KeyValue[Num] *= ExcVar;
                        }
                        if (ExcSyb == "/")
                        {
                            KeyValue[Num] /= ExcVar;
                        }
                    }
                }
                MapShift[0] += KeyValue[0] + KeyValue[2];
                MapShift[1] += KeyValue[1] + KeyValue[3];
                GameArgs[0] = KeyValue[0] + KeyValue[2] + (double)GameArgs[0];
                GameArgs[1] = KeyValue[1] + KeyValue[3] + (double)GameArgs[1];
                KeyValue[0] = 0d;
                KeyValue[1] = 0d;
                KeyValue[2] = 0d;
                KeyValue[3] = 0d;

                Thread.Sleep(1);
            }
        }

    }

    class PaintProperties
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
        int MenuWidth = 150;
        int MenuHeight = 15;

        //预加载方块数，保证低性能可容纳
        public int PreLoadBlcoks = 5;

        public object[] MenuProperties
        {
            get { return new object[] { MaxUseRegion, MenuDistance ,MenuWidth , MenuHeight}; }
        }
        public double[] CamProperties
        { 
            get { return new double[] { NowCameraPosX , NowCameraPosY }; }
            set { NowCameraPosX = value[0];NowCameraPosY = value[1]; }
        }
        public int[] RetGPs() 
        {
            return new int[] { };
        }
        public double[] CalBC(double CamPosX , double CamPosY)
        {

            //计算屏幕可容纳方块数量，确定绘制始末点
            double BCXStart = Math.Floor(CamPosX/ BlockWidth) - PreLoadBlcoks ;
            double BCXEnd = Math.Ceiling(((CamPosX + ScreenWidth) / BlockWidth)) + PreLoadBlcoks ;
            double BCXCount = BCXEnd - BCXStart;

            double BCYStart = Math.Floor(CamPosY/ BlockHeight)- PreLoadBlcoks;
            double BCYEnd = Math.Ceiling((CamPosY + ScreenHeight) / BlockHeight) + PreLoadBlcoks ;
            double BCYCount = BCYEnd - BCYStart;

            

            return new double[] { BCXStart , BCXEnd , BCYStart , BCYEnd  , BCXCount , BCYCount};
        }

    }
}
