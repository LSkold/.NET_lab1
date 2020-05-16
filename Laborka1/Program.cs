using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfoExtension
{
    public static class DirectoryInfoExtension
    {
        public static DateTime FindOldestFile(this System.IO.DirectoryInfo directory, DateTime oldest_date)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                //
                // Replace if checked file is older
                //
                if (file.CreationTime < oldest_date)
                    oldest_date = file.CreationTime;
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                //
                // Go through each directory
                //
                oldest_date = dir.FindOldestFile(oldest_date);
            }

            return oldest_date;
        }

        public static void GetKeysAndValues(this System.IO.DirectoryInfo directory)
        {
            // FileName -> FileLength
            // Longest FileName is first
            SortedDictionary<string, long> pairs = new SortedDictionary<string, long>(new LongestFirst());

            //
            // Add to collection
            //
            foreach (FileInfo file in directory.GetFiles())
            {
                pairs.Add(file.Name, file.Length);
            }
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                pairs.Add(dir.Name, dir.GetFiles().Length+dir.GetDirectories().Length);
            }

            //
            // Print coleection
            //
            foreach (KeyValuePair<string, long> pair in pairs)
            {
                Console.WriteLine(pair.Key + " -> " + pair.Value);
            }
        }

    }
    public static class FileSystemInfoExtension
    {
        public static string GetDOS(this System.IO.FileInfo file)
        {
            string rahs = "";

            if (File.GetAttributes(file.FullName) == FileAttributes.ReadOnly)
                rahs += "r";
            else
                rahs += "-";

            if (File.GetAttributes(file.FullName) == FileAttributes.Archive)
                rahs += "a";
            else
                rahs += "-";

            if (File.GetAttributes(file.FullName) == FileAttributes.Hidden)
                rahs += "h";
            else
                rahs += "-";

            if (File.GetAttributes(file.FullName) == FileAttributes.System)
                rahs += "s";
            else
                rahs += "-";

            return rahs;
        }
    }

    class LongestFirst : IComparer<string>
    {
        public int Compare(string string1, string string2)
        {
            if (string1.Length < string2.Length)
                return -1;
            if (string1.Length > string2.Length)
                return 1;
            return string1.CompareTo(string2);
        }
    }
}

namespace Laborka1
{
    using InfoExtension;

    class Program
    {
        static void ShowDir(string path, int deepLevel)
        {     
            string deep = "";

            //
            // Set deepnes 
            //
            for (int i = 0; i < deepLevel; i++)
            {
                deep += "\t";
            }

            //
            // Print files
            //
            foreach (string file in Directory.GetFiles(path))
            {
                string file_output = deep + Path.GetFileName(file);
                FileInfo current_file = new FileInfo(file);

                file_output += " " + current_file.Length + " bajtow";
                file_output += " "+ current_file.GetDOS(); 

                Console.WriteLine(file_output);
            }

            //
            // Print catalogs
            //
            foreach (string dir in Directory.GetDirectories(path))
            {
                string dir_output = deep + Path.GetFileName(dir);
                DirectoryInfo current_dir = new DirectoryInfo(dir);
                FileInfo current_dir_file_info = new FileInfo(dir);

                int size = current_dir.GetFiles().Length + current_dir.GetDirectories().Length;
                dir_output += " (" + size + ") ";
                dir_output += " " + current_dir_file_info.GetDOS();
                Console.WriteLine(dir_output);

                //
                // Go through each directory 
                //
                ShowDir(dir, deepLevel + 1);
            }

        }


        static void Main(string[] args)
        {
            if (args != null)
            {
                string path = args[0];
                FileInfo catalog = new FileInfo(path);
                DirectoryInfo directory = new DirectoryInfo(path);

                string output = Path.GetFileName(path);
                int size = directory.GetFiles().Length + directory.GetDirectories().Length;

                output += " (" + size + ") ";
                Console.WriteLine(output);
               
                ShowDir(path, 1);

                Console.WriteLine("\nOldest file: " + directory.FindOldestFile(DateTime.Now) + "\n");
                directory.GetKeysAndValues();
            }
            else
            {
                Console.WriteLine("Tab of args is empty");
            }
            Console.Read();
        }
    }
}
