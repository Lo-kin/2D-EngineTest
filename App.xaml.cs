using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Windows.Controls;

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

        public object[] Init(int? Seed)
        {
            int seed;
            if (Seed == null)
            {
                Random RandSeed = new Random();
                seed = RandSeed.Next(int.MinValue, int.MaxValue);
            }
            else
            {
                seed = (int)Seed;
            }

            //使玩家的生成点在正负200内
            Random RandSpwanPosX = new Random();
            Random RandSpwanPosY = new Random();
            int SpwanPosX = RandSpwanPosX.Next(-200, 200);
            int SpwanPosY = RandSpwanPosY.Next(-200, 200);
            GenTerrian GT = new GenTerrian();
            GameProperties GP = new GameProperties();
            object[] IM = GT.InitMap(seed, SpwanPosX, SpwanPosY , GP.PaintDistance);
            return IM;

        }
    }

    class GenTerrian
    {
        public object[] InitMap(int seed , int SPx , int SPy , int PDist)
        {
            object[] MixMap = new object[4]; 
            object[,] Map = new object[2* PDist, 2 * PDist];

            for (int y = 0; y < 2 * PDist; y++)
            {
                for (int x = 0; x < 2 * PDist; x++)
                {
                    Map[x, y] = Block.Dirt();
                }
            }

            for (int x = 0; x < 2 * PDist; x++)
            {
                double Cal = 3 * x;
                int up = (int)Math.Ceiling(Cal);
                int down = (int)Math.Floor(Cal);
                if (down >= 0 & down <= 2 * PDist)
                {
                    Map[x, down] = Block.Water();
                }
                if (up >=0 &up <= 2 * PDist)
                {
                    Map[x, up] = Block.Water();
                }
            }

            MixMap[0] = seed;
            MixMap[1] = SPx;
            MixMap[2] = SPy;
            MixMap[3] = Map;
            return MixMap;
        }
        public object Default(int Seed)
        {
            return new object();
        }
    }

    public class ExcuteCompute
    {

        public object Normal()
        {
            return new object();
        }

        public double[] CamFeedBack(double CavShiftX , double CavShiftY , double CamPosX , double CamPosY)
        {
            PaintProperties GP = new PaintProperties();
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

    class GameProperties
    {
        public int PaintDistance = 1000;
    }

    class Block
    {
        public static object[] Dirt()
        {
            return new object[] { "Dirt", new byte[] { 120, 150, 114 } };
        }
        public static object[] Water()
        {
            return new object[] { "Water", new byte[] { 178, 240, 250 } };
        }

    }

    class Entity
    {
        public object[] Default()
        {
            string ChaID = DateTime.Now.ToString();
            string Name = "Lenny";
            double Speed = 0.03;
            return new object[] { };
        }
    }
    
}
