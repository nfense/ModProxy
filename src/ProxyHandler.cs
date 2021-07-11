using System;
using System.Net;
using System.Text;
using System.IO;
using Nfense.NProxy;

namespace Nfense.ModProxy {
    public class ProxyHandler : Handler {
        public override bool OnHandle(HttpListenerContext ctx, Node node)
        {
            string hostname = ctx.Request.Url.Host;
            string path = ctx.Request.Url.PathAndQuery;

            string proxyHost = node.GetString("proxy_host", hostname);
            string target = node.GetString("proxy_target", "http://127.0.0.1:8080") + path;

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(target);
            request.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpContextUtils.CopyCtxRequestToWebRequest(ctx.Request, request);
            request.Headers.Set("Host", proxyHost);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            HttpWebResponse response = HttpContextUtils.GetResponse(request);

            Logger.Debug("Proxying connection to " + target + " | Host: " + proxyHost);

            if (response == null || !response.GetResponseStream().CanRead)
            {
                ctx.Response.Headers.Add("Content-Type", "text/html");
                byte[] buffer = Encoding.ASCII.GetBytes("<h1>Backend Response Error</h1><p>Server associated to " + ctx.Request.Url.Host + " isn't responding.</p><hr/><i>Powered by NProxy - Nfense</i>");
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                ctx.Response.Close();
                return false;
            }

            else
            {
                try
                {
                    HttpContextUtils.CopyHttpResponseToCtxResponse(response, ctx.Response);
                    ctx.Response.Headers.Set("X-Server", "NProxy");

                    MemoryStream ms = new MemoryStream();
                    response.GetResponseStream().CopyTo(ms);
                    byte[] buffer = ms.ToArray();
                    ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.Close();
                    ctx.Response.Close();
                    return true;
                }
                catch (Exception e)
                {
                    response.Close();
                    ctx.Response.Close();
                    Logger.Critical(e);
                    return false;
                }
            }
        }
    }
}