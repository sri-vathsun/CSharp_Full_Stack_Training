using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;

namespace EventListener_Examples.Example
{
    public class File_Example
    {
        public void main()
        {
            /*   
               string? contents = "This is a demo file created using C#. ";

               File.WriteAllText(path, contents);

               string? readContents = File.ReadAllText(path);
               Console.WriteLine(readContents);

               File.Create("demo2.txt");
            */
            string? path = "demo1.txt";
            string? contents= "This is a demo file created using some C# code. So give some big information to the file add something that is valuable to this example, and make it interesting and informative. This is a great example of how to use the File class in C#.";
            File.WriteAllText(path, contents);

            using StreamReader sr = new StreamReader(path);
            string? line;

            while((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }

            //FileInfo important properties and methods
            FileInfo f1 = new FileInfo(path);

            if (f1.Exists)
            {
                Console.WriteLine("File exist");
            }
            else
            {
                Console.WriteLine("File doesnt exist");
            }

            Console.WriteLine("File Name: " + f1.Name);
            Console.WriteLine("File Full Name: " + f1.FullName);
            Console.WriteLine("File Extension: " + f1.Extension);
            Console.WriteLine("File Directory Name: " + f1.DirectoryName);
            Console.WriteLine("File Length: " + f1.Length);

            
            File.Copy(path, @"D:\File\demo1.txt", true);

            f1.CopyTo(@"D:\File\d.txt", true);

 
        }
    }
}

