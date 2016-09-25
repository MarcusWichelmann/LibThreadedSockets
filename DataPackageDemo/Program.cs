using LibThreadedSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPackageDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DataPackage package = new DataPackage(9999999999, (long)1, 0.1, new int[] { 2, 2 });
            package.Add(new short[] { 3, 3, 3 });
            package.Add(5);
            package.Add('c');
            package.Add("ABC");
            package.Add(true);

            Console.WriteLine($"Result: {package}");

            Console.ReadLine();
        }
    }
}
