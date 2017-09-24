using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace seomoonsijang.Controllers
{
    public class PushNotificationController : Controller
    {

        // GET: PushNotification
        public ActionResult Index(string deviceId, string message)
        {
            if (deviceId != null && message != null)
            {
                string serverKey = "AAAAqDQOgEI:APA91bGp_kLy57U2DjJG9_w50mD0zX9K5dm40slA_gwfA65VMnsvt0plQQ3ijrOwKHotmlIAfNv4LIuBKAi0szQeqwv0cjUc91B16E-VquwQTeyl3DsPLbXuOYPH4vmTegRHHDj006XR";

                try
                {
                    var result = "-1";
                    var webAddr = "https://fcm.googleapis.com/fcm/send";

                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"to\": \"" + deviceId + "\",\"notification\": {\"body\": \"" + message + "\", \"title\" : \"TestMessage From Backend\", \"icon\" : \"HeartFilled.png\"}}";
                        //{"to": "Your device token","data": {"message": "This is a Firebase Cloud Messaging Topic Message!",}}
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    // return result;
                }
                catch (Exception ex)
                {
                    //  Response.Write(ex.Message);
                }

                //string SERVER_API_KEY = "AAAAqDQOgEI:APA91bGp_kLy57U2DjJG9_w50mD0zX9K5dm40slA_gwfA65VMnsvt0plQQ3ijrOwKHotmlIAfNv4LIuBKAi0szQeqwv0cjUc91B16E-VquwQTeyl3DsPLbXuOYPH4vmTegRHHDj006XR";

                //var value = message;
                //string resultStr = "";

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                //request.Method = "POST";
                //request.ContentType = "application/json;charset=utf-8;";
                //request.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                //var postData =
                //new
                //{
                //    data = new
                //    {
                //        title = "서문시장.net",
                //        body = message,
                //    },

                //// FCM allows 1000 connections in parallel.
                //to = deviceId
                //};

                ////Linq to json
                //string contentMsg = JsonConvert.SerializeObject(postData);

                //Byte[] byteArray = Encoding.UTF8.GetBytes(contentMsg);
                //request.ContentLength = byteArray.Length;

                //Stream dataStream = request.GetRequestStream();
                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();

                //try
                //{
                //    WebResponse response = request.GetResponse();
                //    Stream responseStream = response.GetResponseStream();
                //    StreamReader reader = new StreamReader(responseStream);
                //    resultStr = reader.ReadToEnd();
                //    reader.Close();
                //    responseStream.Close();
                //    response.Close();
                //}
                //catch (Exception e)
                //{
                //    resultStr = "";
                //}
            }
            return View();
        }
        
    }
}