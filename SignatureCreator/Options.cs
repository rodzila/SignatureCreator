using System;

namespace SignatureCreator
{
    internal class Options
    {
        string filePath;
        int blockLen;

        public bool Parse(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments");
                return false;
            }
            foreach(string arg in args)
            {
                switch (arg.Substring(0, 3))
                {
                    case "-d=":
                        filePath = arg.Substring(3);
                        break;
                    case "-l=":
                        int.TryParse(arg.Substring(3), out blockLen);
                        break;
                }
            }
            if (filePath == "" || blockLen == 0)
            {
                Console.WriteLine("Invalid arguments");
                return false;
            }
            return true;
        }
        public void GetOptions(out string filePath, out int blockLen)
        {
            filePath = this.filePath;
            blockLen = this.blockLen;
        }
    }
}
