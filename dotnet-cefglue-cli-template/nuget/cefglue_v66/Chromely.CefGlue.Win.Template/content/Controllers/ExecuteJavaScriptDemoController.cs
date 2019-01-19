﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteJavaScriptDemoController.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once StyleCop.SA1300
namespace Chromely.CefSharp.Win.Controllers
{
    using System;
    using Chromely.CefGlue.Winapi;
    using Chromely.Core.RestfulService;
    using LitJson;

    using Xilium.CefGlue;

    /// <summary>
    /// The demo controller.
    /// </summary>
    [ControllerProperty(Name = "ExecuteJavaScriptDemoController", Route = "executejavascript")]
    public class ExecuteJavaScriptDemoController : ChromelyController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteJavaScriptDemoController"/> class.
        /// </summary>
        public ExecuteJavaScriptDemoController()
        {
            this.RegisterPostRequest("/executejavascript/execute", this.Execute);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ChromelyResponse"/>.
        /// </returns>
        private  ChromelyResponse Execute(ChromelyRequest request)
        {
            var response = new ChromelyResponse(request.Id);
            response.ReadyState = (int)ReadyState.ResponseIsReady;
            response.Status = (int)System.Net.HttpStatusCode.OK;
            response.StatusText = "OK";
            response.Data = DateTime.Now.ToLongDateString();

            try
            {
                ScriptInfo scriptInfo = new ScriptInfo(request.PostData);
                CefFrame frame = FrameHandler.GetFrame(scriptInfo.FrameName);
                if (frame == null)
                {
                    response.Data = $"Frame {scriptInfo.FrameName} does not exist.";
                    return response;
                }

                frame.ExecuteJavaScript(scriptInfo.Script, null, 0);
                response.Data = "Executed script :" + scriptInfo.Script;
                return response;
            }
            catch (Exception)
            {
                response.ReadyState = (int)ReadyState.RequestReceived;
                response.Status = (int)System.Net.HttpStatusCode.BadRequest;
                response.StatusText = "Error";
            }

            return response;
        }

        /// <summary>
        /// The script info.
        /// </summary>
        private class ScriptInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ScriptInfo"/> class.
            /// </summary>
            /// <param name="postData">
            /// The post data.
            /// </param>
            public ScriptInfo(object postData)
            {
                this.FrameName = string.Empty;
                this.Script = string.Empty;
                if (postData != null)
                {
                    JsonData jsonData = JsonMapper.ToObject(postData.ToString());
                    this.FrameName = jsonData.Keys.Contains("framename") ? jsonData["framename"].ToString() : string.Empty;
                    this.Script = jsonData.Keys.Contains("script") ? jsonData["script"].ToString() : string.Empty;
                }
            }

            /// <summary>
            /// Gets the frame name.
            /// </summary>
            public string FrameName { get; }

            /// <summary>
            /// Gets the script.
            /// </summary>
            public string Script { get; }
        }
    }
}
