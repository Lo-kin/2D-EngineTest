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

        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DateTime StartTime = DateTime.Now;
            if (e.Key == Key.A)
            {
                KeyPressStat[0] = true;
                KeyPressTime[0] = StartTime;
            }
            if (e.Key == Key.W)
            {
                KeyPressStat[1] = true;
                KeyPressTime[1] = StartTime;
            }
            if (e.Key == Key.D)
            {
                KeyPressStat[2] = true;
                KeyPressTime[2] = StartTime;
            }
            if (e.Key == Key.S)
            {
                KeyPressStat[3] = true;
                KeyPressTime[3] = StartTime;
            }

        }
        
        object GPObejct = new object();

        private void GUIStart()
        {
            Canvas MainMenu = new Canvas();
            this.Content = MainMenu;

            Button RegularStart = new Button();
            RegularStart.Width = 80;
            RegularStart.Height = 60;
            RegularStart.SetValue(Canvas.LeftProperty, (double)400);
            RegularStart.SetValue(Canvas.TopProperty, (double)300);
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

            //定义画布并添加到窗口
            Canvas MainCanvas = new Canvas();
            this.Content = MainCanvas;
            MainCanvas.Background = new SolidColorBrush(Color.FromRgb(100, 150, 100));
            MainCanvas.Margin = new Thickness(0, 0, 0, 0);

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
            //*用于直观的测试边界
            LineGeometry tx = new LineGeometry();
            LineGeometry ty = new LineGeometry();
            tx.StartPoint = new Point(0 , 600);
            tx.EndPoint = new Point(800 , 600);
            ty.StartPoint = new Point(800, 0);
            ty.EndPoint = new Point(800 , 600);

            Path myPath1 = new Path();
            myPath1.Stroke = Brushes.Red;
            myPath1.StrokeThickness = 10;
            myPath1.Data = tx;

            Path myPath2 = new Path();
            myPath2.Stroke = Brushes.Red;
            myPath2.StrokeThickness = 10;
            myPath2.Data = ty;

            MainCanvas.Children.Add(myPath1);
            MainCanvas.Children.Add(myPath2);
            //*/

            //将地图信息传递到object
            GPObejct = MainCanvas as object;
            Console.WriteLine("Done");
            Button a = new Button();
            a.Width = 80;
            a.Height = 60;
            a.SetValue(Canvas.LeftProperty, (double)200);
            a.SetValue(Canvas.TopProperty, (double)150);
            a.Click += new RoutedEventHandler(test);
            MainCanvas.Children.Add(a);

        }

        private void test(object sender, EventArgs e)
        {
            //直接在主线程执行循环会导致ui不更新，使用dispatcher更新
            Thread PaintThread = new Thread(Painter);
            PaintThread.IsBackground = true;
            PaintThread.Start();
        }

        private void Painter()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, (ThreadStart)delegate ()
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

                Canvas MainCanvas = GPObejct as Canvas;

                int spd = 2;
                double CamMoveX = 20;
                double CamMoveY = 20;
                
                for (; ; )
                {

                    DateTime NowDateTime = DateTime.Now;
                    if (KeyPressStat[0] == true)
                    {
                        TimeSpan MoveTime = KeyPressTime[0] - NowDateTime;
                        double MTtoInt = MoveTime.Seconds;
                        CamMoveX -= MTtoInt * spd;
                    }
                    if (KeyPressStat[1] == true)
                    {
                        TimeSpan MoveTime = KeyPressTime[0] - NowDateTime;
                        double MTtoInt = MoveTime.Seconds;
                        CamMoveY -= MTtoInt * spd;
                    }
                    if (KeyPressStat[2] == true)
                    {
                        TimeSpan MoveTime = KeyPressTime[0] - NowDateTime;
                        double MTtoInt = MoveTime.Seconds;
                        CamMoveX += MTtoInt * spd;
                    }
                    if (KeyPressStat[3] == true)
                    {
                        TimeSpan MoveTime = KeyPressTime[0] - NowDateTime;
                        double MTtoInt = MoveTime.Seconds;
                        CamMoveY += MTtoInt * spd;
                    }

                    DoEvents();
                    
                }
            });
        }

        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                 new DispatcherOperationCallback(delegate (object f)
                 {
                     ((DispatcherFrame)f).Continue = false;

                     return null;
                 }
                    ), frame);
            Dispatcher.PushFrame(frame);
        }
    }

    class GameProperties
    {
        //窗口属性
        public int ScreenWidth = 800;
        public int ScreenHeight = 600;

        //绘制属性
        public const int BlockWidth = 30;
        public const int BlockHeight = 30;

        //视角属性
        double NowCameraPosX = 0;
        double NowCameraPosY = 0;

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
            int PreLoadBlcoks = 16;
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
