using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CodeChallengeLineIntersection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please enter the coordinates for the first line (StartX StartY EndX EndY; separated by space)");
                string? userInput = Console.ReadLine();
                Line? lineA;
                while ((TryConvertToLine(userInput, out lineA)) is false)
                {
                    Console.WriteLine("Incorrect input, please try again");
                    userInput = Console.ReadLine();
                }


                Console.WriteLine("Please enter the coordinates for the second line (StartX StartY EndX EndY; separated by space)");
                userInput = Console.ReadLine();
                Line? lineB;
                while ((TryConvertToLine(userInput, out lineB)) is false)
                {
                    Console.WriteLine("Incorrect input, please try again");
                    userInput = Console.ReadLine();
                }

                
                if (lineA is not null && lineB is not null)
                {
                    if ((Line)lineA == (Line)lineB)
                    {
                        Console.WriteLine("These two lines overlap, please try again \n");
                    }
                    else
                    {
                        Console.WriteLine("Calculating Intersection ....");
                        (double x, double y)? result = CalculateIntersection((Line)lineA, (Line)lineB);

                        if (result is not null)
                        {
                            Console.WriteLine($"The intersection point is at x={result.Value.x}, y={result.Value.y}");
                        }
                        else
                        {
                            Console.WriteLine("There is no intersection\n");
                        }
                    } 
                }
            }
        }



        static bool TryConvertToLine(string? userInput, out Line? line)
        {
            if (!string.IsNullOrEmpty(userInput))
            {
                string[] inputArray = userInput.Split(' ');
                if (inputArray.Length is 4)
                {
                    if (double.TryParse(inputArray[0], out double startX) is false  || double.TryParse(inputArray[1], out double startY) is false  ||
                        double.TryParse(inputArray[2], out double endX) is false  || double.TryParse(inputArray[3], out double endY) is false)
                    {
                        line = null;
                        return false;
                    }
                    line = new Line(startX, startY, endX, endY);
                    return true;
                }
            }

            line = null;
            return false;
        }


        static (double, double)? CalculateIntersection (Line lineA, Line lineB)
        {
            double? x = null;
            double? y = null;
            try
            {
                /*Algebra
                 
                (x - lineA.StartX) / (x - lineA.EndX) = (y - lineA.StartY) / (y - lineA.EndY);
                - lineA.EndY* x - lineA.StartX * y + lineA.EndY * lineA.StartX =  - lineA.StartY * x - lineA.EndX * y + lineA.StartY * lineA.EndX;
                - lineA.EndY* x + lineA.StartY * x = lineA.StartX * y - lineA.EndX * y + lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX;
                ( - lineA.EndY + lineA.StartY) * x = (lineA.StartX - lineA.EndX) * y + lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX;
                x = ((lineA.StartX - lineA.EndX) * y + lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / ( - lineA.EndY + lineA.StartY);
                ((lineA.StartX - lineA.EndX) * y + lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / ( -lineA.EndY + lineA.StartY) = ((lineB.StartX - lineB.EndX) * y + lineB.StartY * lineB.EndX - lineB.EndY * lineB.StartX) / ( - lineB.EndY + lineB.StartY);
                ((lineA.StartX - lineA.EndX) / ( - lineA.EndY + lineA.StartY)) * y + (lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / ( -lineA.EndY + lineA.StartY) = ((lineB.StartX - lineB.EndX) / ( -lineB.EndY + lineB.StartY)) * y + (lineB.StartY * lineB.EndX - lineB.EndY * lineB.StartX) / ( - lineB.EndY + lineB.StartY);
                ((lineA.StartX - lineA.EndX) / (-lineA.EndY + lineA.StartY) - (lineB.StartX - lineB.EndX) / (-lineB.EndY + lineB.StartY)) * y = (lineB.StartY * lineB.EndX - lineB.EndY * lineB.StartX) / (-lineB.EndY + lineB.StartY) - (lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / (-lineA.EndY + lineA.StartY);
                 */

                y = ((lineB.StartY * lineB.EndX - lineB.EndY * lineB.StartX) / (-lineB.EndY + lineB.StartY) - (lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / (-lineA.EndY + lineA.StartY)) / ((lineA.StartX - lineA.EndX) / (-lineA.EndY + lineA.StartY) - (lineB.StartX - lineB.EndX) / (-lineB.EndY + lineB.StartY));

                x = ((lineA.StartX - lineA.EndX) * y + lineA.StartY * lineA.EndX - lineA.EndY * lineA.StartX) / (-lineA.EndY + lineA.StartY);

                if (x is double.PositiveInfinity or double.NegativeInfinity || y is double.PositiveInfinity or double.NegativeInfinity)
                {
                    // parallel lines
                    return null;
                }
                else if ((x - lineA.StartX) / (x - lineA.EndX) > 0 || (x - lineB.StartX) / (x - lineB.EndX) > 0 ||
                         (x - lineA.StartY) / (x - lineA.EndY) > 0 || (x - lineB.StartY) / (x - lineB.EndY) > 0)
                {
                    // intersection outside of segment
                    return null;
                }
            }
            catch
            {
                return null;
            }
            

            return ((double)x, (double)y);
        }
    }


    public readonly struct Line : IEquatable<Line>
    {
        public double StartX { get; }
        public double StartY { get; }
        public double EndX { get; }
        public double EndY { get; }

        public Line(double startX, double startY, double endX, double endY)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX; 
            EndY = endY;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Line line && Equals(line);
        }

        public bool Equals(Line other)
        {
            return StartX == other.StartX && StartY == other.StartY && EndX == other.EndX && EndY == other.EndY;
        }

        public static bool operator ==(Line left, Line right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line left, Line right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartX, StartY, EndX, EndY);
        }
    }
}