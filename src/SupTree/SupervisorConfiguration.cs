namespace SupTree
{
    public struct SupervisorConfiguration
    {
        /// <summary>
        /// Max initial workers allowed (default 10)
        /// </summary>
        public int MaxWorkers { get; set; }

        /// <summary>
        /// Milliseconds to wait for threads to finish processing (default 1000)
        /// </summary>
        public int WaitFreeThreadTime { get; set; }

        public SupervisorConfiguration(int maxWorkers, int waitFreeThreadTime = 1000)
        {
            MaxWorkers = maxWorkers;
            WaitFreeThreadTime = waitFreeThreadTime;
        }
    }
}
