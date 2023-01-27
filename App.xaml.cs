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

        public object[,] Init(int? Seed)
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
            object[,] IM = GT.InitMap(seed, SpwanPosX, SpwanPosY , GP.PaintDistance);
            return IM;

        }
    }

    class GenTerrian
    {
        public object[,] InitMap(int seed , int SPx , int SPy , int PDist)
        {
            object[,] Map = new object[2* PDist, 2 * PDist];

            for (int y = 0; y < 2 * PDist; y++)
            {
                for (int x = 0; x < 2 * PDist; x++)
                {
                    Map[x, y] = Block.Dirt();
                }
            }

            for (int x = 0; x <2 * PDist;x ++)
            {
                double FuncY = (x ^ 3) / (5 + 1);
                int Exp1 = (int)Math.Ceiling(FuncY);
                int Exp2 = (int)Math.Floor(FuncY);
                if (Exp1 <= 0 & Exp1 >= 2*PDist)
                {
                    
                }
                else
                {
                    Map[x, Exp1] = Block.Water();
                }
                if (Exp2 <= 0 && Exp2 >= 2 * PDist)
                {
                    
                }
                else
                {
                    Map[x, Exp2] = Block.Water();
                }

            }
            return Map;
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
        public int PaintDistance = 200;
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
            string ChaID = HashBuilder.Hash_MD5_16(DateTime.Now.ToString());
            string Name = "Lenny";
            double Speed = 0.03;
            return new object[] { };
        }
    }

    class HashBuilder
    {
        public static string Hash_MD5_16(string word, bool toUpper = true)
        {
            try
            {
                string sHash = Hash_MD5_32(word).Substring(8, 16);
                return toUpper ? sHash : sHash.ToUpper();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string Hash_MD5_32(string word, bool toUpper = true)
        {
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider MD5CSP
                    = new System.Security.Cryptography.MD5CryptoServiceProvider();

                byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                byte[] bytHash = MD5CSP.ComputeHash(bytValue);
                MD5CSP.Clear();

                //根据计算得到的Hash码翻译为MD5码
                string sHash = "", sTemp = "";
                for (int counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }
                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根据大小写规则决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
