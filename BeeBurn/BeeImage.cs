using System.Windows.Media;

namespace BeeBurn
{
    public class BeeImage
    {
        public ImageSource ImageSource { get; set; }
        public string Name { get; set; }

        public BeeImage(ImageSource src, string name)
        {
            Name = name;
            ImageSource = src;
        }
    }
}
