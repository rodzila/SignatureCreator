using System;
using System.Collections.Generic;
using System.Threading;

using SignatureCreator.Hashing;

namespace SignatureCreator.Processes
{
    public class Block
    {
        public long Id { get; set; }
        public byte[] Data { get; set; }
        public byte[] Hash { get; set; }
    }

    class BlockManagment
    {
        private readonly Queue<Block> inputQueue;
        private readonly object inputLock;

        private readonly Thread[] workers;
        private readonly EventWaitHandle addBlockWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        private const int MAX_QUEUE_SIZE = 5;
        private Hash256 hasher = new Hash256();

        public BlockManagment(int threadsCount)
        {
            this.inputQueue = new Queue<Block>();
            this.inputLock = new object();

            this.workers = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                this.workers[i] = new Thread(Run);
                this.workers[i].Priority = ThreadPriority.Lowest;
            }
        }

        public void Run()
        {
            while(true)
            {
                Block block = null;
                try
                {
                    lock(this.inputLock)
                    {
                        if(this.inputQueue.Count > 0)
                        {
                            block = this.inputQueue.Dequeue();
                            if (block == null) return;
                        }
                    }
                    this.addBlockWaitHandle.Set();

                    if (block != null)
                    {
                        this.Action(block);
                    }
                }
                catch (ThreadAbortException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }

        }

        public void Start()
        {
            for (int i = 0; i < this.workers.Length; i++)
            {
                this.workers[i].IsBackground = true;
                this.workers[i].Start();
            }

            Console.WriteLine("Started {0} workers.", this.workers.Length);
        }

        public void Stop()
        {
            lock (this.inputLock)
            {
                for (int i = 0; i < this.workers.Length; i++)
                    this.inputQueue.Enqueue(null);
            }

            for (int i = 0; i < this.workers.Length; i++)
            {
                this.workers[i].Join();
                Console.WriteLine("Worker {0} stoped!", i);
            }
        }

        public void AddBlockInQueue(Block nextBlock)
        {
            bool isFullQueue;
            lock(inputLock)
            {
                isFullQueue = inputQueue.Count >= MAX_QUEUE_SIZE;
            }

            if(isFullQueue)
            {
                this.addBlockWaitHandle.WaitOne();
            }

            lock(inputLock)
            {
                this.inputQueue.Enqueue(nextBlock);
            }

        }

        private void Action(Block block)
        {
            byte[] hash = this.hasher.GetHash(block.Data);
            block.Hash = hash;
            string hashString = this.hasher.ToString(hash);
            Console.WriteLine("BlockId = {0}, Hash: {1}", block.Id, hashString);
        }
        
    }
}
