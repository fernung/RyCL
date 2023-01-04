namespace RyCL.Runner.DX.Client
{
    public class ClientConfiguration
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool WaitVBlank { get; set; }

        public ClientConfiguration() : 
            this("Rysys by Ryan Fernung") 
        { }
        public ClientConfiguration(string title) : 
            this(title, 640, 480) 
        { }
        public ClientConfiguration(string title, int width, int height, bool waitVBlank = false)
        {
            Title = title;
            Width = width;
            Height = height;
            WaitVBlank = waitVBlank;
        }
    }
}
