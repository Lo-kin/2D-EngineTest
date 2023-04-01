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
            Thread MSChecker = new Thread(new ThreadStart(MapShiftChecker));
            MSChecker.IsBackground = true;
            MSChecker.Start();
            Server Sv = new Server();
            Map = Sv.TestInit();
            LoadPainter();
            Painter();

        }

        object[] Map = new object[0] ;

        private void LoadPainter()
        {
            //获得绘制属性
            PaintProperties PP = new PaintProperties();

            double[] PaintGP = PP.ScrChunk();
            //绘制属性
            int PaintChunkWidth = (int)PaintGP[0];
            int PaintChunkHeight = (int)PaintGP[1];

            //单个方块的属性
            int BlockWidth = PaintProperties.BlockWidth;
            int BlockHeight = PaintProperties.BlockHeight;

            //定义画布并添加到背景画布
            Canvas BGCanvas = FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = new Canvas();
            BGCanvas.Children.Remove(BGCanvas.FindName("MenuCanvas") as UIElement);
            BGCanvas.Children.Add(MainCanvas);
            BGCanvas.RegisterName("PaintCanvas", MainCanvas);
            BGCanvas.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            //初始化绘图位置
            MainCanvas.SetValue(Canvas.LeftProperty, 0d);
            MainCanvas.SetValue(Canvas.TopProperty, 0d);

            GameArgs[2] = PaintGP;

            for (int BlY =  -PaintChunkHeight; BlY <= PaintChunkHeight * 2; BlY++)
            {
                for (int BlX = -PaintChunkWidth; BlX <= PaintChunkWidth * 2; BlX++)
                {
                    object[] BT = (Map[0] as object[,])[500 + BlX , 500 + BlY] as object[];
                    byte[] cl = BT[1] as byte[];

                    Rectangle NowPaintBlcok = new Rectangle();
                    NowPaintBlcok.Width = BlockWidth;
                    NowPaintBlcok.Height = BlockHeight;
                    NowPaintBlcok.Fill = new SolidColorBrush(Color.FromRgb(cl[0], cl[1], cl[2]));
                    NowPaintBlcok.SetValue(Canvas.LeftProperty, (double)BlX * BlockWidth) ;
                    NowPaintBlcok.SetValue(Canvas.TopProperty, (double)BlY * BlockHeight);

                    //在注册名时不能带有特殊符号，例如-
                    string NX;
                    string NY;
                    if (BlX <= 0) { NX = "_" + -BlX; }
                    else { NX = Convert.ToString(BlX); }
                    if (BlY <= 0) { NY = "_" + -BlY; }
                    else { NY = Convert.ToString(BlY); }

                    string RectName = /*"Px" + PCX + "Py" + PCY +*/ "x" + NX + "y" + NY;
                    MainCanvas.RegisterName(RectName, NowPaintBlcok);
                    MainCanvas.Children.Add(NowPaintBlcok);
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

            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;
            InPaintUI();
            Label DebugMsg = BGCanvas.FindName("DebugMsg") as Label;

            for (; ; )
            {
                if (KeyValue[4] == 1) { break; }

                MainCanvas.SetValue(Canvas.LeftProperty, MapShift[0]);
                MainCanvas.SetValue(Canvas.TopProperty, MapShift[1]);
                DebugMsg.Content = "CamX:" + GameArgs[0] + "\n" + "CamY:" + GameArgs[1] + "\n" + "MapShiftX:" + MapShift[0] + "\n" + "MapShiftY:" + MapShift[1] ;

                DoEvents();
                Thread.Sleep(5);
            }
            Environment.Exit(0);
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

        bool FlushXStat = false;
        bool FlushYStat = false;
        private void MapShiftChecker()
        {
            PaintProperties PP = new PaintProperties();
            double[] PaintGP = PP.ScrChunk();
            int PaintChunkWidth = (int)PaintGP[0];
            int PaintChunkHeight = (int)PaintGP[1];

            while (true)
            {
                if (MapShift[0] >= PaintChunkWidth * PaintProperties.BlockWidth / 2 || MapShift[0] <= -PaintChunkWidth * PaintProperties.BlockWidth / 2)
                {

                    FlushXStat = true;
                }
                if (MapShift[1] >= PaintChunkHeight * PaintProperties.BlockHeight / 2 || MapShift[1] <= -PaintChunkHeight * PaintProperties.BlockHeight / 2)
                {
                    
                    FlushYStat = true;
                }

                if (FlushXStat == true || FlushYStat == true)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Canvas BGCanvas = FindName("BGCanvas") as Canvas;
                        Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;

                        int shiftX = (int)(double)GameArgs[0] / PaintProperties.BlockWidth;
                        int shiftY = (int)(double)GameArgs[1] / PaintProperties.BlockHeight;

                        for (int BlY = -PaintChunkHeight; BlY <= PaintChunkHeight * 2; BlY++)
                        {
                            for (int BlX = -PaintChunkWidth; BlX <= PaintChunkWidth * 2; BlX++)
                            {
                                object[] BT = (Map[0] as object[,])[500 + BlX - shiftX, 500 + BlY - shiftY] as object[];
                                byte[] cl = BT[1] as byte[];

                                //在注册名时不能带有特殊符号，例如-
                                string NX;
                                string NY;
                                if (BlX <= 0) { NX = "_" + -BlX; }
                                else { NX = Convert.ToString(BlX); }
                                if (BlY <= 0) { NY = "_" + -BlY; }
                                else { NY = Convert.ToString(BlY); }
                                string RectName = /*"Px" + PCX + "Py" + PCY +*/ "x" + NX + "y" + NY;
                                Rectangle NowPaintBlcok = MainCanvas.FindName(RectName) as Rectangle;

                                NowPaintBlcok.Fill = new SolidColorBrush(Color.FromRgb(cl[0], cl[1], cl[2]));
                            }
                        }
                    }));
                    MapShift[0] %= PaintProperties.BlockWidth;
                    MapShift[1] %= PaintProperties.BlockHeight;
                    FlushXStat = false;
                    FlushYStat = false;
                }

                Thread.Sleep(5);
            }
        }
        
        private void MapFlush()
        {

        }

        public void KeyExcuter()
        {
            double spd = 1.0d;
            //每个按键对应特殊的算法
            object[] KeyGrade = new object[] {
                new object[] {spd , "+"},
                new object[] {spd , "+"},
                new object[] {spd , "-"},
                new object[] {spd , "-"},
                new object[] {-1.0 , "*"},
                new object[] {-1},
            };

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

                Thread.Sleep(5);
            }
        }

        public void InPaintUI()
        {
            Canvas BGCanvas = this.FindName("BGCanvas") as Canvas;
            Canvas MainCanvas = BGCanvas.FindName("PaintCanvas") as Canvas;

            //Health Bar
            Rectangle HBBG = new Rectangle();
            HBBG.Width = 200;
            HBBG.Height = 20;
            HBBG.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            HBBG.SetValue(Canvas.LeftProperty, 500d);
            HBBG.SetValue(Canvas.TopProperty, 50d);
            Rectangle HBPerc = new Rectangle();
            HBPerc.Width = 200;
            HBPerc.Height = 20;
            HBPerc.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            HBPerc.SetValue(Canvas.LeftProperty, 500d);
            HBPerc.SetValue(Canvas.TopProperty, 50d);
            BGCanvas.RegisterName("HBBG", HBBG);
            BGCanvas.RegisterName("HBPerc", HBPerc);
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
        //public int PreLoadBlcoks = 5;

        public object[] MenuProperties
        {
            get { return new object[] { MaxUseRegion, MenuDistance ,MenuWidth , MenuHeight}; }
        }
        public double[] CamProperties
        { 
            get { return new double[] { NowCameraPosX , NowCameraPosY }; }
            set { NowCameraPosX = value[0];NowCameraPosY = value[1]; }
        }

        public double[] CalBC(double CamPosX , double CamPosY)
        {

            //计算屏幕可容纳方块数量，确定绘制始末点
            double BCXStart = Math.Floor(CamPosX/ BlockWidth) ;
            double BCXEnd = Math.Ceiling(((CamPosX + ScreenWidth) / BlockWidth)) ;
            double BCXCount = BCXEnd - BCXStart;

            double BCYStart = Math.Floor(CamPosY/ BlockHeight);
            double BCYEnd = Math.Ceiling((CamPosY + ScreenHeight) / BlockHeight) ;
            double BCYCount = BCYEnd - BCYStart;

            return new double[] { BCXStart , BCXEnd , BCYStart , BCYEnd  , BCXCount , BCYCount};
        }

        public double[] ScrChunk()
        {
            double BCXStart = 0;
            double BCXEnd = Math.Ceiling((double)(ScreenWidth / BlockWidth));
            double BCXCount = BCXEnd - BCXStart;

            double BCYStart = 0;
            double BCYEnd = Math.Ceiling((double)(ScreenHeight / BlockHeight));
            double BCYCount = BCYEnd - BCYStart;

            return new double[] { BCXCount, BCYCount };
        }
    }
}

