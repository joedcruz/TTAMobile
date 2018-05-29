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
        List<string> userRoles;
        List<string> roleClaims;
        public string newToken;
        public static string loginUrl = "";

        public RestService()
        {
            //client = new HttpClient();
            //client.MaxResponseContentBufferSize = 256000;
            userInfoModel = new UserInfoModel();
        }

        public async Task<Boolean> Register(string username, string password)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo();
            Boolean registrationSuccess = false;
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        
            registrationInfo.MobileNo = username;
            registrationInfo.Password = password;
            var jsonData = new StringContent(JsonConvert.SerializeObject(registrationInfo), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://10.0.2.2:5000/api/register", jsonData);
            response.EnsureSuccessStatusCode();
            registrationSuccess = true;
            var responseData = response.Content.ReadAsStringAsync().Result;
            //userRoles = JsonConvert.DeserializeObject<List<string>>(responseData.ToString());

            client.Dispose();

            return registrationSuccess;
        }

        public async Task<UserInfoModel> Login(string username, string password)
        {
            client = new HttpClient();
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

        // Api method for retrieving all roles assigned to the user
        public async Task<List<string>> GetUserRoles(string userId, string token)
        {
            if (userId != null)
            {
                // Call api GetUserRoles by passing the user id as body and user token as header
                UserInfoModel _userInfoModel = new UserInfoModel();

                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _userInfoModel.UserId = userId;
                var jsonData = new StringContent(JsonConvert.SerializeObject(_userInfoModel), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://10.0.2.2:5000/api/userroles", jsonData);
                response.EnsureSuccessStatusCode();
                var responseData = response.Content.ReadAsStringAsync().Result;
                userRoles = JsonConvert.DeserializeObject<List<string>>(responseData.ToString());

                client.Dispose();
            }

            return userRoles;
        }


        // Api method for retrieving all roles who has access to the form
        public async Task<List<string>> GetRoleClaims(List<string> roles, string pageName, string token)
        {
            if (roles != null)
            {
                // Call api GetRoleClaims by passing the claim type and claim value as body and user token as header
                RoleClaimsModel _rolesClaimsModel = new RoleClaimsModel();

                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _rolesClaimsModel.Type = "Mobile";
                _rolesClaimsModel.Value = pageName;
                var jsonData = new StringContent(JsonConvert.SerializeObject(_rolesClaimsModel), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://10.0.2.2:5000/api/roleclaims", jsonData);
                response.EnsureSuccessStatusCode();
                var responseData4 = response.Content.ReadAsStringAsync().Result;
                roleClaims = JsonConvert.DeserializeObject<List<string>>(responseData4.ToString());

                client.Dispose();
            }

            return roleClaims;
        }


        public Boolean CheckPageAuthorization(List<string> roles, List<string> claims)
        {
            bool roleMatch = false;

            // Check if any roles match
            foreach (var x in roles)
            {
                foreach (var y in claims)
                {
                    if (x == y)
                    {
                        roleMatch = true;
                        break;
                    }
                }
            }

            return roleMatch;
        }


        //public async Task<T> PostResponseLogin<T>(string weburl, FormUrlEncodedContent content) where T : class
        //{            
        //    var response = await client.PostAsync(weburl, content);
        //    var jsonResult = response.Content.ReadAsStringAsync().Result;
        //    var responseObject = JsonConvert.DeserializeObject<T>(jsonResult);
        //    return responseObject;
        //}

        //public async Task<T> PostResponse<T>(string weburl, string jsonString) where T : class
        //{
        //    var Token = ""; // App.TokenDatabase.GetToken();
        //    string ContentType = "application/json";
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        //    try
        //    {
        //        var result = await client.PostAsync(weburl, new StringContent(jsonString, Encoding.UTF8, ContentType));
        //        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var jsonResult = result.Content.ReadAsStringAsync().Result;
        //            try
        //            {
        //                var contentResp = JsonConvert.DeserializeObject<T>(jsonResult);
        //                return contentResp;
        //            }
        //            catch
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return null;
        //}

        //public async Task<T> GetResponse<T>(string weburl) where T : class
        //{
        //    var Token = "";  //App.TokenDatabase.GetToken();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        //    try
        //    {
        //        var response = await client.GetAsync(weburl);
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var jsonResult = response.Content.ReadAsStringAsync().Result;
        //            try
        //            {
        //                var contentResp = JsonConvert.DeserializeObject<T>(jsonResult);
        //                return contentResp;
        //            }
        //            catch
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    return null;
        //}
    }
}
