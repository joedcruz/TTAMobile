using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TTAMobile
{
    public class RestService
    {        
        HttpClient client;
        UserInfoModel userInfoModel;
        public string newToken;
        public static string loginUrl = "";

        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            userInfoModel = new UserInfoModel();
        }

        public async Task<UserInfoModel> Login(string username, string password)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonData = new StringContent(JsonConvert.SerializeObject(new { MobileNo = username, Password = password }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://10.0.2.2:5000/api/login", jsonData);
            response.EnsureSuccessStatusCode();

            var responseData1 = response.Content.ReadAsStringAsync();
            newToken = JsonConvert.DeserializeObject(responseData1.Result).ToString();

            userInfoModel.Token = newToken;

            if (newToken != null)
            {
                // Call api GetUserInfo by passing the user token as header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                response = await client.GetAsync("http://10.0.2.2:5000/api/userinfo");
                response.EnsureSuccessStatusCode();
                var responseData = response.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData.Result.ToString());

                foreach (var userInfo in userData)
                {
                    if (userInfo.Key == "userId")
                        userInfoModel.UserId = userInfo.Value;

                    if (userInfo.Key == "username")
                        userInfoModel.Username = userInfo.Value;
                }
            }

            client.Dispose();

            return userInfoModel;
            //return newToken;
        }

        public async Task<T> PostResponseLogin<T>(string weburl, FormUrlEncodedContent content) where T : class
        {            
            var response = await client.PostAsync(weburl, content);
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            var responseObject = JsonConvert.DeserializeObject<T>(jsonResult);
            return responseObject;
        }

        public async Task<T> PostResponse<T>(string weburl, string jsonString) where T : class
        {
            var Token = ""; // App.TokenDatabase.GetToken();
            string ContentType = "application/json";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            try
            {
                var result = await client.PostAsync(weburl, new StringContent(jsonString, Encoding.UTF8, ContentType));
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonResult = result.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var contentResp = JsonConvert.DeserializeObject<T>(jsonResult);
                        return contentResp;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public async Task<T> GetResponse<T>(string weburl) where T : class
        {
            var Token = "";  //App.TokenDatabase.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            try
            {
                var response = await client.GetAsync(weburl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonResult = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var contentResp = JsonConvert.DeserializeObject<T>(jsonResult);
                        return contentResp;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
