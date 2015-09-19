

namespace BoxLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net;
    using System.IO;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class BoxBase
    {
        public string Authenticate;
        const string KEY = "fyryt0rwvqldp4gi2h1bf2b3hjkd7s3b";
        const string TOKEN = "e2isqprt0i95dtlmagf7evjptyfw9amx";
        private string _baseAddress;
        public string authHeader { get; set; }
        public string Headers { get; set; }

        string addr = "/1.0/rest?action=";
        string action = "get_ticket";
        string api_key = "&api_key=";
        string api_ticket = "&ticket=";        
        
        /// <summary>
        /// use this constructor for login process.
        /// </summary>
        public BoxBase()
        {
            _baseAddress = "https://www.box.com/api";
            authHeader = "Authorization: BoxAuth api_key=" + KEY + "&auth_token=" + TOKEN;
        }
                
        /// <summary>
        /// Generic get request for data.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="restResource"></param>
        public void GetRequest(Action<string> process, string restResource)
        {
            try
            {
                string v =  restResource;
                HttpWebRequest request = WebRequest.Create(v) as HttpWebRequest ;
                request.Headers.Add(this.authHeader);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    string message;               
                    string header = response.Headers.ToString();
                    Stream stream = response.GetResponseStream();
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        message = streamReader.ReadToEnd();
                    }
                    if (process != null)
                    {
                        process.Invoke(message);
                    }                 
                }
            }
            catch (WebException e)
            {
                this.HandleWebException(e);
                throw;
            }
        }

        /// <summary>
        /// Generic post to box
        /// </summary>
        /// <param name="Process"></param>
        /// <param name="restAddress"></param>
        /// <param name="parameters">needs to be in json</param>
        public void PostRequest(Action<string> Process, string restAddress, string parameters)
        {
            try
            {          
                string v = _baseAddress + restAddress;
                HttpWebRequest request = WebRequest.Create(v) as HttpWebRequest;
                request.Method = "POST";
                request.Headers.Add(this.authHeader);
                
                // This is sent to the Post
                byte[] bytes = Encoding.UTF8.GetBytes(parameters);                                

                using (Stream requestStream = request.GetRequestStream())
                {                    
                    requestStream.Write(bytes, 0, bytes.Length);                   
                    requestStream.Flush();
                    requestStream.Close();
                    
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        string message;               
                        string header = response.Headers.ToString();
                        Stream stream = response.GetResponseStream();
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            message = streamReader.ReadToEnd();
                        }

                        if (Process != null)
                        {
                            Process.Invoke(message);
                        }                 
                    }
                }
            }
            catch (WebException e)
            {
                this.HandleWebException(e);
                throw;
            } 
        }

        /// <summary>
        /// Use for posting to box a multi part form data.
        /// </summary>
        /// <param name="Process"></param>
        /// <param name="restAddress"></param>
        /// <param name="parameters">needs to be in json</param>
        public void PostMultiPartFormData(Action<string> Process, string restAddress, List<string> parameters, Stream file, string contentType)
        {
            try
            {                                   
                HttpWebRequest request = WebRequest.Create(_baseAddress + restAddress) as HttpWebRequest;
                request.Method = "POST";
                request.Headers.Add(this.authHeader);
                string [] boundary = contentType.Split('=');
                if (file != null)
                {
                    request.ContentType = contentType;
                }

                using (Stream requestStream = request.GetRequestStream())
                {                                        
                    StringBuilder p = new StringBuilder();
                    foreach (var n in parameters)
                    {
                        p.Append("--" + boundary[1] + "\r\n" + n);
                    }
                    byte[] b = Encoding.UTF8.GetBytes(p.ToString());
                    requestStream.Write(b, 0, b.Length);

                    if (file != null)
                    {
                        file.CopyTo(requestStream);
                    }
                    requestStream.Flush();
                    requestStream.Close();                    
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        string message;               
                        string header = response.Headers.ToString();
                        Stream stream = response.GetResponseStream();
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            message = streamReader.ReadToEnd();
                        }

                        if (Process != null)
                        {
                            Process.Invoke(message);
                        }                 
                    }
                }
            }
            catch (WebException e)
            {
                this.HandleWebException(e);
                throw;
            }            
        }

        /// <summary>
        /// Used to handle web exceptions.
        /// </summary>
        /// <param name="e"></param>
        private void HandleWebException(WebException e)
        {
            string message = string.Empty;
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
                WebResponse resp = e.Response;
                this.Headers = resp.Headers.ToString();
                //string statusCode = resp.
                using (StreamReader streamReader = new StreamReader(resp.GetResponseStream()))
                {
                    message = streamReader.ReadToEnd();
                    Debug.Write(message);
                }
            }
            //this.LogErrorMessage(Headers + message);
            //or send alert
        }

        /// <summary>
        /// Logon Process. Box.Com Version 1.0
        /// GetTicket, redirect the user to login to Box wit the Ticket.
        /// </summary>
        /// <param name="process"></param>
        public void GetTicket(Action<string> process)
        {
            string k = "";         
            this.GetRequest(
                (string response) =>
                {
                    Debug.Write(response);
                    XElement x = XElement.Parse(response);

                    if (x.Descendants("status").FirstOrDefault().Value == "get_ticket_ok")
                    {
                        k = x.Descendants("ticket").FirstOrDefault().ToString();//.Value;
                        if (process != null)
                        {
                            process.Invoke(JsonConvert.SerializeObject(k));
                        }
                        else
                        {
                            if (process != null)
                            {
                                process.Invoke(JsonConvert.SerializeObject(x));
                            }
                        }
                    }                    
                }, addr + action + api_key + KEY);            
        }

        /// <summary>
        /// Logon Process. Box.Com Version 1.0
        /// GetTicket, redirect the user to login to Box wit the Ticket.
        /// GetToken for the session
        /// </summary>
        /// <param name="process"></param>
        /// <param name="ticket"></param>
        public void GetToken(Action<string> process, string ticket)
        {
            action = "get_auth_token";
            string k = "";
            this.GetRequest(
                (string response) =>
                {
                    Debug.Write(response);
                    XElement x = XElement.Parse(response);

                    if (x.Descendants("status").FirstOrDefault().Value == "get_ticket_ok")
                    {
                        k = x.Descendants("ticket").FirstOrDefault().ToString();//.Value;
                        if (process != null)
                        {
                            process.Invoke(JsonConvert.SerializeObject(k));
                        }
                    }
                    else
                    {
                        if (process != null)
                        {
                            process.Invoke(JsonConvert.SerializeObject(x));
                        }
                    }
                }, addr + action + api_key + KEY + api_ticket + ticket);  
        }  
        
        /// <summary>
        /// used in registration process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderName"></param>
        /// <param name="parentID"></param>
        public void CreateFolder(Action<string>process, string folderName, string parentID)
        {
            //build parameters
            JObject j = new JObject(
                new JProperty("name", folderName),
                    new JProperty("parent", 
                        new JObject(
                            new JProperty("id", parentID)))
            );
           
            this.PostRequest(
                (string response) =>
                {
                    Debug.Write(response);
                    JObject x = JObject.Parse(response);
                   
                    if (process != null)
                    {
                        process.Invoke(x.ToString());
                    }
                },"/2.0/folders", j.ToString());  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderID"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="contentType"></param>
        public void UploadFile(Action<string> process,  string folderID, string fileName, Stream file, string contentType)
        {
            _baseAddress = "https://api.box.com";            

            //build parameters
            List<string> j = new List<string>();
            j.Add("Content-Disposition: form-data; name=\"filename\"\r\n\r\n" + fileName + "\r\n");
            j.Add( "Content-Disposition: form-data; name=\"folder_id\"\r\n\r\n" + folderID + "\r\n");

            this.PostMultiPartFormData(
                (string response) =>
                {
                    Debug.Write(response);
                    JObject x = JObject.Parse(response);
                    
                    if (process != null)
                    {
                        process.Invoke(x.ToString());
                    }
                }, "/2.0/files/content", j, file, contentType); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderID"></param>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <param name="contentType"></param>
        public void UploadVersionFile(Action<string> process, string eTag, string fileName, Stream file, string contentType, string fileID)
        {
            _baseAddress = "https://api.box.com";

            //build parameters
            List<string> j = new List<string>();
            j.Add("Content-Disposition: form-data; name=\"filename\"\r\n\r\n" + fileName + "\r\n");
            //j.Add("Content-Disposition: form-data; name=\"folder_id\"\r\n\r\n" + folderID + "\r\n");

            this.PostMultiPartFormData(
                (string response) =>
                {
                    Debug.Write(response);
                    JObject x = JObject.Parse(response);

                    if (process != null)
                    {
                        process.Invoke(x.ToString());
                    }
                }, "/2.0/files/ " + fileID + "/content", j, file, contentType);
        }

        /// <summary>
        /// Returns a URL to download a file.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileID"></param>
        public void DownloadFile(Action<string> process, string fileID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/files/" + fileID + "/content");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="message"></param>
        /// <param name="fileID"></param>
        public void AddComment(Action<string> process, string message, string fileID )
        {
            //build parameters
            JObject j = new JObject(
                new JProperty("message", message)
            );

            this.PostRequest(
                (string response) =>
                {
                    //Debug.Write(response);
                    //JObject x = JObject.Parse(response);

                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "/2.0/files/"  + fileID + "/comments", j.ToString());
        }

        /// <summary>
        /// Add comments to a discussion.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="message"></param>
        /// <param name="discussionID"></param>
        public void AddDiscussionComment(Action<string> process, string message, string discussionID)
        {
            //build parameters
            JObject j = new JObject(
                new JProperty("message", message)
            );

            this.PostRequest(
                (string response) =>
                {
                    //Debug.Write(response);
                    //JObject x = JObject.Parse(response);

                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "/2.0/discussions/"  + discussionID + "/comments", j.ToString());
        }

        /// <summary>
        /// Get Comment from a file.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="commentID"></param>
        public void GetComment(Action<string> process, string commentID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/comments/" + commentID);
        }

        /// <summary>
        /// Used in register process. All comments are tied to a Discussion. A dicussion called Lease Process is created.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderID"></param>
        public void AddDiscussion(Action<string> process, string folderID)
        {
            //build parameters
            JObject j = new JObject(
                new JProperty("parent", 
                    new JObject(
                         new JProperty("id", folderID))),                     
                            new JProperty("name", "Lease Process")
            );           

            this.PostRequest(
                (string response) =>
                {
                    //Debug.Write(response);
                    //JObject x = JObject.Parse(response);

                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "/2.0/discussions" , j.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="discussionID"></param>
        public void GetDiscussionComments(Action<string> process, string discussionID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/discussions/" + discussionID + "/comments");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderID"></param>
        public void GetFolderItems(Action<string> process, string folderID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/folders/" + folderID + "/items?limit=100&offset=0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="folderID"></param>
        public void GetFolderMetadata(Action<string> process, string folderID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/folders/" + folderID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileID"></param>
        public void GetFileMetadata(Action<string> process, string fileID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/files/" + fileID);
        }

        //public void GetFolderMetadata(Action<string> process, string folderID)
        //{
        //    this.GetRequest(
        //        (string response) =>
        //        {
        //            if (process != null)
        //            {
        //                process.Invoke(response);
        //            }
        //        }, "https://api.box.com/2.0/folders/" + folderID);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileID"></param>
        public void GetFileComments(Action<string> process, string fileID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/files/" + fileID + "/Comments");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileID"></param>
        public void GetFileVersions(Action<string> process, string fileID)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0/files/" + fileID + "/versions");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="collabID"></param>
        public void GetCollaborations(Action<string> process)
        {
            this.GetRequest(
                (string response) =>
                {
                    if (process != null)
                    {
                        process.Invoke(response);
                    }
                }, "https://api.box.com/2.0" + "/collaborations?status=pending");
        }

        /// <summary>
        /// Copy a file into another directory.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="fileID"></param>
        /// <param name="folderID"></param>
        public void CopyFile(Action<string> process, string fileID, string folderID)
        {
            //build parameters
            JObject j = new JObject(
                new JProperty("parent",
                    new JObject(
                        new JProperty("id",folderID)))
            );

            this.PostRequest(
                (string response) =>
                {
                    Debug.Write(response);
                    JObject x = JObject.Parse(response);

                    if (process != null)
                    {
                        process.Invoke(x.ToString());
                    }
                }, "/2.0/files/" + fileID + "/copy" , j.ToString());  
        }
    }
}
