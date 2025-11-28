using PSTParse;
using PSTParse.Message_Layer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            List<string> pstPaths = new List<string>()
            {
                @"C:\Users\nt084\Outlook\Test PST - no pwd - ANSI.pst",
                @"C:\Users\nt084\Outlook\Test PST - no pwd.pst",
                @"C:\Users\nt084\Outlook\Test PST - pwd - ANSI.pst",
                @"C:\Users\nt084\Outlook\Test PST - pwd.pst",
            };

            foreach (var pstPath in pstPaths)
            {
                ProcessPST(pstPath);
                Console.WriteLine();
            }

            sw.Stop();
            Console.WriteLine("Total Time: {0} milliseconds", sw.ElapsedMilliseconds);
        }

        static void ProcessPST(string pstPath)
        { 
            Console.WriteLine("Opening PST: " + pstPath);

            // var logPath = @"C:\Users\nt084\Downloads\vp346.txt"; // "C:\test\nidlog.txt";
            var pstSize = new FileInfo(pstPath).Length*1.0/1024/1024;
            Console.WriteLine("PST Size: {0:0.00} MB", pstSize);

            using (var file = new PSTFile(pstPath))
            {
                //Console.WriteLine("Magic value: " + file.Header.DWMagic);
                Console.WriteLine("Is Ansi: " + file.Header.IsANSI);

                //var lookup = new PSTParse.Message_Layer.NamedToPropertyLookup(file);

                // Write all Properties out from the dictionary with their name and value
                //foreach (var prop in lookup.PC.Properties)
                //{
                //    Console.WriteLine("Property: {0} - {1}", prop.Key, prop.Value);
                //}

                Console.WriteLine("Is Password Protected: " + file.IsPasswordProtected());

                //var stack = new Stack<MailFolder>();
                //stack.Push(file.TopOfPST);
                //var totalCount = 0;
                //if (File.Exists(logPath))
                //    File.Delete(logPath);
                //using (var writer = new StreamWriter(logPath))
                //{
                //    while (stack.Count > 0)
                //    {
                //        var curFolder = stack.Pop();

                //        foreach (var child in curFolder.SubFolders)
                //            stack.Push(child);
                //        var count = curFolder.ContentsTC.RowIndexBTH.Properties.Count;
                //        totalCount += count;
                //        Console.WriteLine(String.Join(" -> ", curFolder.Path) + " ({0} messages)", count);
                    
                //        foreach (var ipmItem in curFolder)
                //        {
                //            if (ipmItem is Message)
                //            {
                //                var message = ipmItem as Message;
                //                Console.WriteLine(message.Subject);
                //                Console.WriteLine(message.Imporance);
                //                Console.WriteLine("Sender Name: " + message.SenderName);
                //                if (message.From.Count > 0)
                //                    Console.WriteLine("From: {0}",
                //                                      String.Join("; ", message.From.Select(r => r.EmailAddress)));
                //                if (message.To.Count > 0)
                //                    Console.WriteLine("To: {0}",
                //                                      String.Join("; ", message.To.Select(r => r.EmailAddress)));
                //                if (message.CC.Count > 0)
                //                    Console.WriteLine("CC: {0}",
                //                                      String.Join("; ", message.CC.Select(r => r.EmailAddress)));
                //                if (message.BCC.Count > 0)
                //                    Console.WriteLine("BCC: {0}",
                //                                      String.Join("; ", message.BCC.Select(r => r.EmailAddress)));
                                

                //                writer.WriteLine(ByteArrayToString(BitConverter.GetBytes(message.NID)));
                //            }
                //        }
                //    }
                //}
                //sw.Stop();
                //Console.WriteLine("{0} messages total", totalCount);
                //Console.WriteLine("Parsed {0} ({2:0.00} MB) in {1} milliseconds", Path.GetFileName(pstPath),
                //                  sw.ElapsedMilliseconds, pstSize);
                ////file.Header.NodeBT.Root.GetOffset(1);
                //Console.Read();
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
