using System;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class CoinImage : Disposable, IImage
    {
        public CoinImage(float width, float height, ThemeColor color, string centerImageID)
        {
            this.width = width;
            this.height = height;
            this.color = color;
            this.centerImageID = centerImageID;
        }


        private bool disposable = false;
        public bool Disposable { get => disposable; set => disposable = value; }

        private float width;
        public float Width => width;

        private float height;
        public float Height => height;

        private ThemeColor color;
        private string centerImageID;
        private IImage image;

        private readonly Rect imageRect = new Rect();

        private readonly float[] polig1 = new float[12];
        private readonly float[] polig2 = new float[12];
        private readonly float[] polig3 = new float[12];

        private float w1 = 70 / 96f;
        private float h1 = 80 / 96f;
        private float dh1 = 40 / 96f;

        private bool created = false;
        private void CreatePoligons()
        {
            if (created) return;
            created = true;
            float wd = width * w1;
            float hd = height * h1;
            float lh = width * dh1;
            float y1 = (hd - lh) / 2;

            this.image = Images.Get(this.centerImageID);
            if (this.image != null)
            {
                float imsize = wd * 0.6f;
                this.imageRect.Set((width - imsize) / 2, (hd - imsize) / 2, imsize, imsize);
            }


            int num = 0;

            polig1[num++] = width / 2;
            polig1[num++] = 0;

            polig1[num++] = width / 2 + wd / 2;
            polig1[num++] = y1;

            polig1[num++] = width / 2 + wd / 2;
            polig1[num++] = y1 + lh;

            polig1[num++] = width / 2;
            polig1[num++] = hd;

            polig1[num++] = width / 2 - wd / 2;
            polig1[num++] = y1 + lh;

            polig1[num++] = width / 2 - wd / 2;
            polig1[num++] = y1;


        }

        public void Draw(Graphics g, float x, float y, float width, float height, int state)
        {
        }

        public void DrawScale(Graphics g, float x, float y, float scaleX, float scaleY, int state)
        {
            CreatePoligons();
            g.Smoosh(() =>
            {
                int color = this.color;
                g.Offset(x, y, () =>
                {
                    int s = g.Save();
                    float wd = width * w1;
                    g.Translate(width / 2 - wd / 2, height * 0.34f);
                    g.Scale(0.7f, 0.7f);
                    g.FillPolygon(polig1, Color.A(color, 150));

                    g.Translate(width / 2 - wd / 2, height * 0.34f);
                    g.Scale(0.7f, 0.7f);
                    g.FillPolygon(polig1, Color.A(color, 50));

                    g.Restore(s);
                    g.FillPolygon(polig1, color);
                    if (this.image != null)
                        this.image.Draw(g, imageRect.x, imageRect.y, imageRect.width, imageRect.height, Color.White);
                });
            });
        }
    }
}
