using System;
using System.Net;

namespace Nfense.ModProxy
{
    public class HttpContextUtils
    {
        public static void CopyCtxRequestToWebRequest(HttpListenerRequest from, HttpWebRequest to)
        {
            if (from.Cookies != null)
            {
                if (to.CookieContainer == null)
                {
                    to.CookieContainer = new CookieContainer();
                }

                foreach (Cookie cookie in from.Cookies)
                {
                    if (cookie.Domain != null && cookie.Domain != "")
                        to.CookieContainer.Add(cookie);
                }
            }

            if (from.HttpMethod != null)
            {
                to.Method = from.HttpMethod;
            }

            if (from.ProtocolVersion != null)
            {
                to.ProtocolVersion = from.ProtocolVersion;
            }

            if (from.UserAgent != null)
            {
                to.UserAgent = to.UserAgent;
            }

            to.KeepAlive = from.KeepAlive;
            to.Headers = (WebHeaderCollection)from.Headers;
        }

        public static void CopyHttpResponseToCtxResponse(HttpWebResponse from, HttpListenerResponse to)
        {
            if (from.Headers != null)
            {
                to.Headers = from.Headers;
            }

            if (from.Cookies != null)
            {
                to.Cookies = from.Cookies;
            }

            if (from.ContentType != null)
            {
                to.ContentType = from.ContentType;
            }

            if (from.ProtocolVersion != null)
            {
                to.ProtocolVersion = from.ProtocolVersion;
            }

            if (from.ContentLength >= 0)
            {
                to.ContentLength64 = from.ContentLength;
            }

            to.StatusCode = (int)from.StatusCode;
        }

        public static HttpWebResponse GetResponse(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                return (HttpWebResponse)ex.Response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}