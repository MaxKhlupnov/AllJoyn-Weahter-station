using System;
using System.Diagnostics;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Web.Http;
using Windows.System.Threading;
using System.Threading;

namespace SensorClient.DataModel
{
    public sealed class ConnectTheDotsHelper
    {
        // App Settings variables
        AppSettings localSettings = new AppSettings();

        // Http connection string, SAS tokem and client
        Uri uri;
        private string sas;
        HttpClient httpClient = new HttpClient();
        bool EventHubConnectionInitialized = false;
        private Mutex mutex;
        private ThreadPoolTimer SASTokenRenewTimer;

        private ConnectTheDotsHelper() {
            // Create a timer-initiated ThreadPool task to renew SAS token regularly
            SASTokenRenewTimer = ThreadPoolTimer.CreatePeriodicTimer(RenewSASToken, TimeSpan.FromMinutes(15));
        }

        public static ConnectTheDotsHelper makeConnectTheDotsHelper()
        {
            ConnectTheDotsHelper helper = new ConnectTheDotsHelper();
            helper.localSettings.ServicebusNamespace = "mtcmanager";
            helper.localSettings.EventHubName = "ehdevices";
            helper.localSettings.KeyName = "IoTHub";
            helper.localSettings.Key = "dVNY7kKjvsW7ouu2p5T7jeKZQath8lTqICEbsrUzccc=";
            helper.localSettings.DisplayName = "makhlupi";
            helper.localSettings.Organization = "Microsoft Technology Center";
            helper.localSettings.Location = "Moscow";
            helper.InitEventHubConnection();
            return helper;
        }


        string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value).Replace("%20", "+");
        }
        /// <summary>
        /// Validate the settings 
        /// </summary>
        /// <returns></returns>
        bool ValidateSettings()
        {
            if ((localSettings.ServicebusNamespace == "") ||
                (localSettings.EventHubName == "") ||
                (localSettings.KeyName == "") ||
                (localSettings.Key == "") ||
                (localSettings.DisplayName == "") ||
                (localSettings.Organization == "") ||
                (localSettings.Location == ""))
            {
                this.localSettings.SettingsSet = false;
                return false;
            }

            this.localSettings.SettingsSet = true;
            return true;

        }

        /// <summary>
        /// When appSettings popup closes, apply new settings to sensors collection
        /// </summary>
        void SaveSettings()
        {
            if (ValidateSettings())
            {
               // ApplySettingsToSensors();
                this.InitEventHubConnection();
            }
        }

        /// <summary>
        /// reset SAS token (as it expires after a given time we need to allow the app to renew the token regularly)
        /// </summary>
        void UpdateSASToken()
        {
            this.sas = SASTokenHelper();
            this.httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);
        }

        /// <summary>
        ///  Apply settings to sensors collection
        /// </summary>
        /*      private void ApplySettingsToSensors()
              {
                  foreach (ConnectTheDotsSensor sensor in sensors)
                  {
                      sensor.displayname = this.localSettings.DisplayName;
                      sensor.location = this.localSettings.Location;
                      sensor.organization = this.localSettings.Organization;
                  }
              }*/

        private ConnectTheDotsMeasure ApplySettingsToMeasure(ConnectTheDotsMeasure measure)
        {

                measure.displayname = this.localSettings.DisplayName;
                measure.location = this.localSettings.Location;
                measure.organization = this.localSettings.Organization;

            return measure;
        }

        /// <summary>
        /// Send message to Azure Event Hub using HTTP/REST API
        /// </summary>
        /// <param name="message"></param>
        public async void sendMeasure(ConnectTheDotsMeasure measure)
        {
            measure = ApplySettingsToMeasure(measure);
            string message = measure.ToJson();
            if (this.EventHubConnectionInitialized)
            {
                try
                {
                    HttpStringContent content = new HttpStringContent(message, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                    HttpResponseMessage postResult = await httpClient.PostAsync(uri, content);

                    if (postResult.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Message Sent: {0}", content);
                    }
                    else
                    {
                        Debug.WriteLine("Failed sending message: {0}", postResult.ReasonPhrase);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception when sending message:" + e.Message);
                }
            }
        }

        private void RenewSASToken(ThreadPoolTimer timer)
        {
            bool hasMutex = false;

            try
            {
                hasMutex = mutex.WaitOne(1000);
                if (hasMutex)
                {
                    UpdateSASToken();
                }
            }
            finally
            {
                if (hasMutex)
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Helper function to get SAS token for connecting to Azure Event Hub
        /// </summary>
        /// <returns></returns>
        private string SASTokenHelper()
        {
            int expiry = (int)DateTime.UtcNow.AddMinutes(20).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string stringToSign = UrlEncode(this.uri.ToString()) + "\n" + expiry.ToString();
            string signature = HmacSha256(this.localSettings.Key.ToString(), stringToSign);
            string token = String.Format("sr={0}&sig={1}&se={2}&skn={3}", UrlEncode(this.uri.ToString()), UrlEncode(signature), expiry, this.localSettings.KeyName.ToString());

            return token;
        }

        /// <summary>
        /// Because Windows.Security.Cryptography.Core.MacAlgorithmNames.HmacSha256 doesn't
        /// exist in WP8.1 context we need to do another implementation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string HmacSha256(string key, string kvalue)
        {
            var keyStrm = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var valueStrm = CryptographicBuffer.ConvertStringToBinary(kvalue, BinaryStringEncoding.Utf8);

            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(keyStrm);
            hash.Append(valueStrm);

            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }

        /// <summary>
        /// Initialize Event Hub connection
        /// </summary>
        private bool InitEventHubConnection()
        {
            try
            {
                this.uri = new Uri("https://" + this.localSettings.ServicebusNamespace +
                              ".servicebus.windows.net/" + this.localSettings.EventHubName +
                              "/publishers/" + this.localSettings.DisplayName + "/messages");
                UpdateSASToken();
                //this.sas = SASTokenHelper();
                //this.httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("SharedAccessSignature", sas);
                this.EventHubConnectionInitialized = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
