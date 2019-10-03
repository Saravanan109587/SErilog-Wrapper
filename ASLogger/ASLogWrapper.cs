using Serilog;
using Serilog.Core;
using Serilog.Sinks.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASLogger
{
    public class ASLogProvider : IAslogger
    {
        public static Logger logg =null;
        public ASLogProvider(string serverURL)
        {
            //logg = new LoggerConfiguration()              
            //   .WriteTo.Http(serverURL, httpClient: new CustomHttpClient())
            //   .CreateLogger();
            if(logg == null)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                logg = new LoggerConfiguration()
                .WriteTo.File(path + @"logs\Aslog.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, rollOnFileSizeLimit: true, fileSizeLimitBytes: 123456)
                .WriteTo.Http(serverURL, httpClient: new CustomHttpClient())
                .CreateLogger();
                logg.Debug("from Logger");
            }          
        }

        public void Debug(string msg, params object[] param)
        {
            logg.Debug(msg, param);
        }

        public void Error(Exception e, string msg, params object[] param)
        {
            logg.Error(e, msg, param);
        }

        public void Error(string msg)
        {
            logg.Debug(msg);
        }

        public void Info(string msg, params object[] param)
        {
            logg.Information(msg, param);
        }
    }

    public interface IAslogger
    {
        void Debug(string msg, params object[] param);
        void Info(string msg, params object[] param);
        void Error(Exception e, string msg, params object[] param);
        void Error(string msg);
    }

    public class CustomHttpClient : IHttpClient
    {
        private readonly HttpClient httpClient;

        public CustomHttpClient()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", "secret-api-key");
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) => httpClient.PostAsync(requestUri, content);

        public void Dispose() => httpClient?.Dispose();
    }
}
