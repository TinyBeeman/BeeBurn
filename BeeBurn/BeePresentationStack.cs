namespace BeeBurn
{
    public class BeePresentationStack : BeeStack
    {
        private BeeImage m_currentImg = null;

        public BeePresentationStack() : base("Presentation Stack")
        {
            Images.Add(BeeImage.CreateStopImage());
        }

        public BeeImage PeekNextImage(bool loop)
        {
            return Images.Count > 0 ? Images[0] : null;
        }

        public bool StopAtNext
        {
            get => (Images.Count == 0) ? true : Images[0].IsStopImage;
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


        public BeeImage GetNextImage(bool loop = true)
        {
            if (Images.Count == 0)
            {
                m_currentImg = null;
                return null;
            }

            m_currentImg = Images[0];

            if (Images.Count > 1)
            {
                Images.RemoveAt(0);
                if (loop)
                    Images.Add(m_currentImg);
            }

            return m_currentImg;
        }
        public BeeImage CurrentImage { get => m_currentImg;}

    }
}
