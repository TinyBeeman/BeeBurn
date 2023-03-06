using System.Windows.Media.Imaging;

namespace BeeBurn
{
    public class BeePresentationStack : BeeStack
    {
        public BeePresentationStack() : base("Presentation Stack")
        {
            Images.Add(BeeImage.CreateStopImage());
        }

        public BeeImage PeekNextImage()
        {
            return NextImage;
        }

        public BeeImage NextImage
        {
            get => Images.Count > 1 ? Images[1] : CurrentImage;
        }

        public bool StopAtNext
        {
            get => NextImage.IsStopImage;
        }

        public void InsertNextImage(BeeImage img)
        {
            if (Images.Count <= 1)
            {
                Images.Add(img);
            }
            else
            {
                Images.Insert(1, img);
            }
        }

        public void PasteNextImage()
        {
            // TODO: Log failure.
            try
            {
                BitmapImage srcClip = BeeClipboard.BitmapImageFromClipboard();
                if (srcClip != null)
                {
                    InsertNextImage(new BeeImage(srcClip, "Paste-" + BeeBurnVM.Get().PasteCounter.ToString("D" + 4)));
                }
            }
            catch { };
        }

        public void EnsureStopImageAfterCurrent()
        {
            // If we have 0 or 1 images, we can get away
            // with making sure the last images is a stop image,
            // because that will always be the "next" image.
            if (Images.Count <= 1)
            {
                EnsureStopImage(false);
            }
            else
            {
                if (!Images[0].IsStopImage)
                    Images.Insert(1, BeeImage.CreateStopImage());
            }
        }

        public void EnsureStopImage(bool atStart)
        {
            if (atStart)
            {
                if (Images.Count == 0 || !Images[0].IsStopImage)
                    Images.Insert(0, BeeImage.CreateStopImage());
            }
            else
            {
                if (Images.Count == 0 || !Images[Images.Count - 1].IsStopImage)
                    Images.Add(BeeImage.CreateStopImage());
            }
        }


        public BeeImage CycleNextImage()
        {
            if (Images.Count > 1)
            {
                BeeImage imgNext = Images[0];
                Images.RemoveAt(0);
                Images.Add(imgNext);
            }

            return CurrentImage;
        }
        public BeeImage CurrentImage
        {
            get
            {
                if (Images.Count == 0)
                {
                    EnsureStopImage(true);
                }
                return Images[0];
            }
        }

    }
}
