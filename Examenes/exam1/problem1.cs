//==========================================================
// Type your name and student ID here.
// Guillermo Pérez Trueba A01377162
//==========================================================

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace Exam1 {

    public class Problem1 {
        public static void Main (String [] args) {
    
            if (args.Length != 1) {
                Console.Error.WriteLine("Please specify the name of the input file.");
                Environment.Exit(1);
            }

            try {            
                var inputPath = args[0];                
                var input = File.ReadAllText(inputPath);
                String[] months = {null, "January","February","March","April","May","June","July","August","September","October","November","December"};
                Regex regex = new Regex(@"([\d][\d][\d][\d]-[\d][\d]-[\d][\d])");
                List<String> dates_found = new List<String>();
                List<String> dates_format = new List<String>();
                
                foreach (Match m in regex.Matches(input)){
                    if(m.Groups[0].Success) {
                        dates_found.Add(m.Value);
                        string[] date = m.Value.Split('-');
                        int year = int.Parse(date[0]);
                        int month = int.Parse(date[1]);
                        int day = int.Parse(date[2]);
                        String date_format = months[month].ToString()  + " " + day + ", " + year;
                        dates_format.Add(date_format);
                    } 
                }

                int i = 0;
                String[] dates_format_arr = dates_format.ToArray();
                foreach (String aPart in dates_found)
                {
                    input = input.Replace(aPart, dates_format_arr[i]);
                    i++;
                }

                Console.WriteLine(input);
                
            } catch (FileNotFoundException e) {
                Console.Error.WriteLine(e.Message);
                Environment.Exit(1);
            }                

        }
    }
}

