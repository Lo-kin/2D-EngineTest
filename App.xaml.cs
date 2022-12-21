using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace _WPF_RPG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

    }

    class Server
    {
        object[] Players = new object[20];
        object ServerMap = new object();

    }

    public class ExcuteCompute
    {
        public object Normal()
        {
             
            return new object();
        }

        public double[] CamFeedBack(double CavShiftX , double CavShiftY , double CamPosX , double CamPosY)
        {
            GameProperties GP = new GameProperties();
            double ScreenW = GP.ScreenWidth;
            double ScreenH = GP.ScreenHeight;
            double CCDistanceX = ScreenW / 2;
            double CCDistanceY = ScreenH / 2;
            //                               之前的区域
            double[] OldRegionX = new double[] { 
                Math.Abs(CCDistanceX - 2 * Math.Abs(CavShiftX - CCDistanceX)) ,
                Math.Abs(CCDistanceY - 2 * Math.Abs(CavShiftY - CCDistanceY)) ,
            };
            return new double[] { };
        }

    }
}
