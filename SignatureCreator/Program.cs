using System;
using System.IO;

using SignatureCreator.Processes;


namespace SignatureCreator
{
    class Program
    {
        private Stream sourceFile;
        private ThreadsProcess thProcess;

        private Stream GetInputStream(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        void StartCalculate(string filePath, int blockLen)
        {
            try
            {
                sourceFile = GetInputStream(filePath);
                this.thProcess = new ThreadsProcess(sourceFile, blockLen);
                bool result = this.thProcess.Run();
       
            }
            catch (FileNotFoundException fnfEx)
            {
                Console.WriteLine(fnfEx.Message);
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine(dirEx.Message);
            }
            catch (FileLoadException flEx)
            {
                Console.WriteLine(flEx.Message);
            }

            
            if (sourceFile != null)
            {
                sourceFile.Close();
                sourceFile.Dispose();
            }
        }

        static void Main(string[] args)
        {
            Options options = new Options();
            string filePath;
            int blockLen;

            if (!options.Parse(args)) return;

            options.GetOptions(out filePath, out blockLen);
            Console.WriteLine("Starting create hash:");

            Program program = new Program();
            program.StartCalculate(filePath, blockLen);
            Console.Write("Press any key..");
            Console.ReadKey();
        }
       
    }
}
