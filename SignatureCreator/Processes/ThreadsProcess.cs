using System;
using System.IO;
using System.Threading;

namespace SignatureCreator.Processes
{
    class ThreadsProcess
    {
        private int blockLen;
        Stream sourceFile;
        private byte[] data;

        private Thread queueThread;
        private BlockManagment blockManagment;

        private bool isAborder;


        public ThreadsProcess(Stream sourceFile, int blockLen)
        {
            this.blockLen = blockLen;
            this.sourceFile = sourceFile;
            this.blockManagment = new BlockManagment(Environment.ProcessorCount);
            this.data = new byte[blockLen];
        }

        public bool Run()
        {
            this.queueThread = new Thread(Process);
            this.queueThread.Priority = ThreadPriority.AboveNormal;

            this.queueThread.Start();
            this.blockManagment.Start();
            this.queueThread.Join();
            this.blockManagment.Stop();

            return !this.isAborder;
        }

        private void Process()
        {
            long streamLen = this.sourceFile.Length;
            while (streamLen - 1 > this.sourceFile.Position)
            {
                try
                {
                    Block nextBlock = GetNextBlock();
                    blockManagment.AddBlockInQueue(nextBlock);
                }
                catch (ArgumentOutOfRangeException argEx)
                {
                    this.isAborder = true;
                    Console.WriteLine("Block reading fail. Reading of input file are stoped.{0}", argEx.Message);
                    return;
                }
                catch (ThreadAbortException thrEx)
                {
                    Console.WriteLine(thrEx.Message);
                }
            }
        }

        private Block GetNextBlock()
        {
            long i = sourceFile.Position % blockLen == 0 ? sourceFile.Position / blockLen + 1 : sourceFile.Position / blockLen + 2;
            sourceFile.Read(this.data, 0, blockLen);
            Block block = new Processes.Block
            {
                Id = i,
                Data = this.data,
            };
            var a = System.Diagnostics.Process.GetCurrentProcess();
            if (a.WorkingSet64 > 3221225472)
            {
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return block;
        }
    }
}

