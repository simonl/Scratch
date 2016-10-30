using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    public static class Program
    {
        public static void Main2(string[] args)
        {
            Console.WriteLine("> Get FizzBuzz");
            Console.ReadLine();

            foreach (var text in FizzBuzz.FizzBuzzes())
            {
                Console.WriteLine(text);
                Console.ReadLine();
            }
        }
    }
}
