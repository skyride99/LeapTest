namespace Leap.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    //using Bechtel.WebClient;
    //using BPIMLibrary;
    using System.Data;
    using System.Diagnostics;
    using System.EnterpriseServices;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    using System.Xml.Linq;
    using bechtel.ipix.common.services;
    using bechtel.ipix.common.utility;
    using BoxLibrary;
    using log4net;
    ///using System.Web;

    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class Leap : BaseService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Leap));
        private LeaseProcess _serviceProvider = new LeaseProcess();
        public Leap()
        {
            try
            {
            }
            catch (Exception ex)
            {
                this.HandleException(ex, MessageEnums.InitializationMessage, "xml");
                //_serviceProvider.LogErrorMessage(ex.ToString());
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }

        }

        [WebGet(UriTemplate = "/TestMessage")]
        [OperationContract]
        [Description("Used to bounce a test message to see if the service is running.")]
        public string GetTestMessage()
        {            
            return "hi";
        }

        [WebGet(UriTemplate = "/Ticket")]
        [OperationContract]
        public void GetTicket()
        {
            try
            {
                _serviceProvider.GetTicket(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "text/xml";
                    HttpContext.Current.Response.Write(response);
                });
                //string j = _serviceProvider.GetTicket();
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/Token/{ticket}")]
        [OperationContract]
        public void GetToken(string ticket)
        {
            try
            {
                _serviceProvider.GetToken(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, ticket);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/Folder?t={token}&i={folderID}")]
        [OperationContract]
        public void GetFolder(string token, string folderID)
        {
            try
            {
                _serviceProvider.GetFolderItems(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, folderID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/File?t={token}&i={fileID}")]
        [OperationContract]
        public void GetFile(string token, string fileID)
        {
            try
            {
                _serviceProvider.GetFileMetadata(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, fileID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/File/Comments?t={token}&i={fileID}")]
        [OperationContract]
        public void GetFileComments(string token, string fileID)
        {
            try
            {
                _serviceProvider.GetFileComments(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, fileID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebInvoke(UriTemplate = "/Folder?t={token}&n={folderName}&p={parentID}",Method="POST")]
        [OperationContract]
        public void CreateFolder(string token, string folderName, string parentID)
        {
            try
            {
                _serviceProvider.CreateFolder(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                },folderName, parentID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebInvoke(UriTemplate = "/File?t={token}&i={folderID}&n={fileName}", Method = "POST")]
        [OperationContract]
        public void UploadFile(string token, string folderID, string fileName, Stream fileStream)
        {
            try
            {                
                _serviceProvider.UploadFile(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, folderID, fileName, fileStream, HttpContext.Current.Request.Headers["Content-Type"]);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebInvoke(UriTemplate = "/FileVersion?t={token}&e={eTag}&n={fileName}&i={fileID}", Method = "POST")]
        [OperationContract]
        public void UploadVerionFile(string token, string eTag, string fileName, Stream fileStream, string fileID)
        {
            try
            {
                _serviceProvider.UploadVersionFile(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, eTag, fileName, fileStream, HttpContext.Current.Request.Headers["Content-Type"], fileID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebInvoke(UriTemplate = "/Discussion?t={token}&i={folderID}", Method = "POST")]
        [OperationContract]
        public void AddDiscussion(string token, string folderID)
        {
            try
            {  
                _serviceProvider.AddDiscussion(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, folderID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/Discussion/Comments?t={token}&i={discussionID}")]
        [OperationContract]
        public void GetDiscussionComments(string token, string discussionID)
        {
            try
            {
                _serviceProvider.GetDiscussionComments(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, discussionID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebInvoke(UriTemplate = "/CopyFile?t={token}&i={folderID}&f={fileID}", Method = "POST")]
        [OperationContract]
        public void CopyFile(string token, string folderID, string fileID)
        {
            try
            {
                _serviceProvider.CopyFile(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, fileID, folderID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }

        [WebGet(UriTemplate = "/FileDownload?t={token}&i={fileID}")]
        [OperationContract]
        public void DownloadFile(string token, string fileID)
        {
            try
            {
                _serviceProvider.DownloadFile(
                (string response) =>
                {
                    HttpContext.Current.Response.ContentType = "application/json";
                    HttpContext.Current.Response.Write(response);
                }, fileID);
            }
            catch (Exception exc)
            {
                //log error
                this.HandleException(exc, "json");
            }
        }



    }
}