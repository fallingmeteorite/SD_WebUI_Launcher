namespace Awake.Models
{
    public struct CommitItem
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string Message { get; set; }
        public string Date { get; set; }
        public string Tag { get; set; }

        public bool use_start { get; set; }

        public bool Checked { get; set; }

    }
}
