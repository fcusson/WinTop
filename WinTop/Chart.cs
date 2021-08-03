﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTop
{
    class Chart
    {

        public const int LINE_CHART = 0;
        public const int AREA_CHART = 1;

        //chars for the line chart type
        private const char LINE_UP = '┗';
        private const char LINE_DOWN = '┏';
        private const char LINE_HORIZONTAL = '━';
        private const char LINE_VERTICAL = '┃';
        private const char LINE_FROM_UP = '┛';
        private const char LINE_FROM_DOWN = '┓';

        public int Type { get; set; }
        public List<float> DataSet { get; set; }
        public float MaxValue { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ConsoleColor ChartColor { get; set; }
        public int FrameIndex { get; set; }
        public int[] ProtectedData { get; set; }

        /// <summary>
        /// Base constructor with all properties modifiable
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="datatSet">set of data to print</param>
        /// <param name="maxValue">the maximum value that can exist in the dataset. If zero, will take the maxValue of the current dataset</param>
        /// <param name="startX">Horizontal start position (top left)</param>
        /// <param name="startY">Vertical start posotion (top left)</param>
        /// <param name="width">width of the chart</param>
        /// <param name="height">height of the chart</param>
        /// <param name="chartColor">Color of the chart</param>
        public Chart(int type, List<float> datatSet, float maxValue, int startX, int startY, int width, int height, ConsoleColor chartColor, int frameIndex)
        {
            Type = type;
            DataSet = datatSet;
            StartX = startX;
            MaxValue = maxValue;
            StartY = startY;
            Width = width;
            Height = height;
            ChartColor = chartColor;
            FrameIndex = frameIndex;
            ProtectedData = new int[] { 0, 0 };
        }

        /// <summary>
        /// Chart constructor that takes a data set and a Frame object and derives start position and size from it
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="datatSet">set of data to print</param>
        /// <param name="maxValue">the maximum value that can exist in the dataset (can be dynamically modified for moving max</param>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        public Chart(int type, List<float> datatSet, float maxValue, Frame frame, ConsoleColor chartColor, int frameIndex) : this(type, datatSet, maxValue,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// Chart constructor that takes a Frame object and derives start position and size from it. Assumes the max value is 100.
        /// </summary>
        /// <param name="type">Type of chart to print (Line or Area)</param>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        public Chart(int type, Frame frame, ConsoleColor chartColor, int frameIndex) : this(type, new List<float>(), 100,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// Line chart constructor that takes a Frame object and derives start position and size from it. Assumes the max value is 100.
        /// </summary>
        /// <param name="frame">Frame object in which the chart resides</param>
        /// <param name="chartColor">Color of the chart</param>
        public Chart(Frame frame, ConsoleColor chartColor, int frameIndex) : this(LINE_CHART, new List<float>(), 100,frame.PosX + 1, frame.PosY + 1, frame.Width - 2, frame.Height - 2, chartColor, frameIndex) { }

        /// <summary>
        /// Method that updates the starts and size property of chart based on a frame object
        /// </summary>
        /// <param name="frame">the frame in which the chart resides</param>
        public void UpdatePosition(Frame frame)
        {
            StartX = frame.PosX + 1;
            StartY = frame.PosY + 1;
            Width = frame.Width - 2;
            Height = frame.Height - 2;
            ProtectedData = frame.ProtectedData;
        }

        public void PrintChart()
        {
            switch (Type)
            {
                case LINE_CHART:
                    PrintLineChart();
                    break;
                case AREA_CHART:
                    PrintAreaChart();
                    break;
            }
        }

        private void PrintAreaChart()
        {

        }

        private void PrintLineChart()
        {

            Console.ForegroundColor = ChartColor;

            int hStart = StartX + Width - DataSet.Count;

            for (int i = DataSet.Count - 1; i >= 0; i--)
            {

                float max;
                int presentValue;
                int olderValue;

                do
                {
                    try
                    {
                        max = MaxValue != 0 ? MaxValue : DataSet.Max();

                        presentValue = VerticalValue(DataSet[i], max, Height);
                        olderValue = (i != 0) ? VerticalValue(DataSet[i - 1], max, Height) : presentValue;

                        //set cursor posotion
                        Console.SetCursorPosition(hStart + i, StartY + Height - presentValue - 1);
                        break;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Frame.UpdateFrame(Program.appFrames);
                        UpdatePosition(Program.appFrames[FrameIndex]);
                        UpdateDataSet(DataSet);
                        i = DataSet.Count - 1;
                        hStart = StartX + Width - DataSet.Count;
                    }
                } while (true);

                //print the char
                if (!(Console.CursorLeft <= ProtectedData[0] && Console.CursorTop <= ProtectedData[1])) { Console.Write(GetLineChar(presentValue, olderValue)); }
                else { string test = "here"; }
                
                //print the ascending or descending line
                PrintVeticalLines(presentValue, olderValue, hStart + i);
            }

            Console.ResetColor();
        }

        private void PrintVeticalLines(int presentValue, int olderValue, int hPos)
        {

            int end = StartY + Height - olderValue - 1;
            int start = StartY + Height - presentValue - 1;
            char endChar = LINE_VERTICAL;
            
            //set the horizontal position
            Console.CursorLeft = hPos;

            //determine direction, if both value equal, exit
            if (presentValue == olderValue)
            {
                return;
            }
            else if (presentValue > olderValue)
            {
                for (int i = start + 1; i < end; i++)
                {
                    
                    Console.CursorTop = i;
                    if (!(Console.CursorLeft <= ProtectedData[0] && Console.CursorTop <= ProtectedData[1]))
                    {
                        Console.Write(LINE_VERTICAL);
                        Console.CursorLeft--;
                    }
                    else
                    {
                        string test = "here";
                    }
                    
                }

                endChar = LINE_FROM_UP;
            }
            else if (presentValue < olderValue)
            {
                for (int i = start - 1; i > end; i--)
                {
                    Console.CursorTop = i;
                    if (!(Console.CursorLeft <= ProtectedData[0] && Console.CursorTop <= ProtectedData[1]))
                    {
                        Console.Write(LINE_VERTICAL);
                        Console.CursorLeft--;
                    }
                    else
                    {
                        string test = "here";
                    }
                    
                }

                endChar = LINE_FROM_DOWN;
            }

            //write the last char
            Console.CursorTop = end;
            if (!(Console.CursorLeft <= ProtectedData[0] && Console.CursorTop <= ProtectedData[1]))
            {
                Console.Write(endChar);
                Console.CursorLeft--;
            }
            else
            {
                string test = "here";
            }
            
        }

        private char GetLineChar(float presentValue, float olderValue)
        {
            if (olderValue > presentValue)
            {
                return LINE_UP;
            }
            else if (olderValue == presentValue)
            {
                return LINE_HORIZONTAL;
            }
            else if (olderValue < presentValue)
            {
                return LINE_DOWN;
            }

            return LINE_DOWN;
        }

        public void UpdateDataSet(List<float> newData)
        {
            //remove data out of the range of the width
            if (newData.Count > Width)
            {
                newData.RemoveRange(0, newData.Count - Width);
            }
        
            
            //reverse the dataset
            //newData.Reverse();

            DataSet = newData;
        }

        private static int VerticalValue(float data, float max, int height)
        {
            float percent = data / max;
            int result = (int)(percent * height);

            return result < height ? result : height - 1;
        }

    }
}