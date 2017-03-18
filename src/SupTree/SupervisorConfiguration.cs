namespace SupTree
{
    public class SupervisorConfiguration
    {
        /// <summary>
        /// Max initial workers allowed (default 10)
        /// </summary>
        public int MaxWorkers { get; set; }

        /// <summary>
        /// Milliseconds to wait for threads to finish processing (default 1000)
        /// </summary>
        public int WaitFreeThreadTime { get; set; }

        public SupervisorConfiguration()
        {
            MaxWorkers = 10;
            WaitFreeThreadTime = 1000;
        }
    }
}
