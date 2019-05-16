//==========================================================
// Type your name and student ID here.
// Guillermo Pérez Trueba A01377162
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Exam1 {
    public class Problem2 {
        public static void Main (String [] args) {

             if (args.Length != 1) {
                Console.Error.WriteLine("Please specify the name of the input file.");
                Environment.Exit(1);
            }

            try {            
                var inputPath = args[0];                
                var input = File.ReadAllText(inputPath);
                Regex regex = new Regex(@"([01234567]+)");

                int suma = 0;
                foreach (Match m in regex.Matches(input)){
                    if(m.Groups[0].Success) {
                       suma += Convert.ToInt32(m.Value.ToString(), 8);
                    } 
                }
                Console.WriteLine(Convert.ToString(suma, 8));
                
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }                

        }
    }
}
