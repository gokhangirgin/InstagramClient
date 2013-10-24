using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace DotNetOpenAuth.AspNet.Clients
{
    public class InstagramClient : OAuth2Client
    {
        /// <summary>
        /// Authorization End Point
        /// </summary>
        private const string AuthorizationEndpoint = "https://api.instagram.com/oauth/authorize/";
        /// <summary>
        /// Access Token End Point
        /// </summary>
        private const string TokenEndPoint = "https://api.instagram.com/oauth/access_token";
        /// <summary>
        /// Consumer Key
        /// </summary>
        private string ClientId;
        /// <summary>
        /// Consumer Secret
        /// </summary>
        private string ClientSecret;
        /// <summary>
        /// Scopes ..
        /// </summary>
        private string[] Scopes;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId">Instagram Client Id (AppId)</param>
        /// <param name="clientSecret">Instagram Client Secret (AppSecret)</param>
        /// <param name="scopes">Scopes to get more info (basic,comments,likes,relationships) http://instagram.com/developer/authentication/ according to documentation, it can be basic+relationships, However It gives invaild scopes error with multiple scope :/ </param>
        public InstagramClient(string clientId, string clientSecret,string[] scopes) : base("instagram") { 
            ClientId = clientId; 
            ClientSecret = clientSecret; 
            Scopes = scopes; }
        //extraData!
        private Dictionary<string, string> userData = new Dictionary<string, string>();
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            //client_id={0}&redirect_uri={1}&response_type=code&scope=[] **Authentication Params
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id",ClientId);
            query.Add("redirect_uri",returnUrl.ToString());
            query.Add("response_type","code");
            if (Scopes.Length > 0)
                query.Add("scope", String.Join("+", Scopes));
            return new UriBuilder(AuthorizationEndpoint) { Query = query.ToString() }.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            return userData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("client_secret", ClientSecret);
            query.Add("code", authorizationCode);
            query.Add("grant_type", "authorization_code");
            query.Add("redirect_uri", returnUrl.ToString());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TokenEndPoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (Stream rstream = request.GetRequestStream())
            using(StreamWriter sw = new StreamWriter(rstream))
            {
                sw.Write(query.ToString());
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using(StreamReader sr = new StreamReader(stream))
            {
                //basic user info through access_token request that maps to extraData
                //http://instagram.com/developer/authentication/
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                userData.Add("access_token", Convert.ToString(obj.access_token));
                userData.Add("id", Convert.ToString(obj.user[0].id));
                userData.Add("username", Convert.ToString(obj.user[0].username));
                userData.Add("full_name", Convert.ToString(obj.user[0].full_name));
                userData.Add("profile_picture", Convert.ToString(obj.user[0].profile_picture));
            }
            return userData["access_token"];
        }
    }
}