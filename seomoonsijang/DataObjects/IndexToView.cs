using System;
using System.Net;
using System.Drawing;

namespace seomoonsijang.DataObjects
{
    public class IndexToView
    {
        public IndexToView(string shopName, string blobURL, string text, int orientation, string location)
        {
            ShopName = shopName;
            BlobURL = blobURL;
            Text = text;
            Orientation = ImageRotation(orientation);
            Location = location;
        }

        public IndexToView(string blobURL, string text)
        {
            ShopName = "";
            BlobURL = blobURL;
            Text = text;
            Orientation = ImageRotation(ImageOrientation(blobURL));
        }

        public IndexToView()
        {

        }

        public string ShopName { get; set; }
        public string BlobURL { get; set; }
        public string Text { get; set; }
        public string Orientation { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Location { get; set; }

        protected string ImageRotation(int orientation)
        {
            string result;
            switch (orientation)
            {
                case 2:
                    result = "-moz-transform: scaleX(-1); -webkit-transform: scaleX(-1); -o-transform: scaleX(-1); transform: scaleX(-1); -ms-filter: fliph; /*IE*/ filter: fliph; /*IE*/";
                    //myImage_Android.RotationY = 180;
                    break;
                case 3:
                    result = "-moz-transform: scale(-1,-1); -webkit-transform: scale(-1, -1); -o-transform: scale(-1, -1); transform: scale(-1, -1);";
                    //myImage_Android.RotationX = 180;
                    //myImage_Android.RotationY = 180;
                    break;
                case 4:
                    result = "-moz-transform: scaleY(-1); -webkit-transform: scaleY(-1); -o-transform: scaleY(-1); transform: scaleY(-1); -ms-filter: flipv; /*IE*/ filter: flipv; /*IE*/";
                    //myImage_Android.RotationX = 180;
                    break;
                case 5:
                    result = "-moz-transform: scaleY(-1); -webkit-transform: scaleY(-1); -o-transform: scaleY(-1); transform: scaleY(-1); -ms-filter: flipv; /*IE*/ filter: flipv; /*IE*/";
                    result += "-moz-transform: rotate(90deg); -webkit-transform: rotate(90deg); -o-transform: rotate(90deg); transform: rotate(90deg);";
                    //myImage_Android.Rotation = 90;
                    //myImage_Android.RotationY = 180;
                    break;
                case 6:
                    result = "-moz-transform: rotate(90deg); -webkit-transform: rotate(90deg); -o-transform: rotate(90deg); transform: rotate(90deg);";
                    //myImage_Android.Rotation = 90;
                    break;
                case 7:
                    result = "-moz-transform: scaleX(-1); -webkit-transform: scaleX(-1); -o-transform: scaleX(-1); transform: scaleX(-1); -ms-filter: fliph; /*IE*/ filter: fliph; /*IE*/";
                    result = "-moz-transform: rotate(90deg); -webkit-transform: rotate(90deg); -o-transform: rotate(90deg); transform: rotate(90deg);";

                    //myImage_Android.Rotation = 90;
                    //myImage_Android.RotationX = 180;
                    break;
                case 8:
                    result = "-moz-transform: rotate(270deg); -webkit-transform: rotate(270deg); -o-transform: rotate(270deg); transform: rotate(270deg);";
                    //myImage_Android.Rotation = 270;
                    break;
                default:
                    result = "";
                    break;


            }
            

            return result;
        }

        protected int ImageOrientation(string imgURL)
        {
            WebRequest req = WebRequest.Create(imgURL);
            WebResponse response = req.GetResponse();
            Image img = Image.FromStream(response.GetResponseStream());

            // Get the index of the orientation property.
            int orientation_index =
                Array.IndexOf(img.PropertyIdList, 0x0112);

            return img.GetPropertyItem(0x0112).Value[0];
        }
    }
}