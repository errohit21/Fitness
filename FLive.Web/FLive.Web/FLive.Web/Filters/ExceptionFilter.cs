using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace FLive.Web.Filters
{
	public class ApiExceptionFilter : ExceptionFilterAttribute
	{


		public ApiExceptionFilter()
		{

		}

		public override void OnException(ExceptionContext context)
		{
			var message = string.Empty;
			if (context.Exception is BusinessException)
			{

				Log.Logger.Information($"Business exception: {context.Exception.Message}", context.Exception);
				context.HttpContext.Response.StatusCode = 400;
				var jsonString = "{\"Message\":\"" + context.Exception.Message.Trim() + "\"}";
				byte[] data = Encoding.UTF8.GetBytes(jsonString);
				context.HttpContext.Response.ContentType = "application/json";
				context.HttpContext.Response.Body.Write(data, 0, data.Length);

			}
			else if (context.Exception is UnauthorizedAccessException)
			{
				Log.Logger.Warning("Look, someone is trying to be smart with access");

				context.HttpContext.Response.StatusCode = 401;

				// handle logging here
			}
			else
			{
				// Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";                
                string stack = null;
#else
				var msg = context.Exception.GetBaseException().Message;
				string stack = context.Exception.StackTrace;
				msg = $"Message:{msg} , Details:{stack}";

#endif

				message = msg;

				Log.Logger.Error(context.Exception, "Unhandled Error");



				context.HttpContext.Response.StatusCode = 500;

				// handle logging here
			}

			// always return a JSON result
			context.Result = new JsonResult(message);

			base.OnException(context);
		}

		private Stream GenerateStreamFromString(String p)
		{
			var enc = Encoding.UTF8;
			var bytes = enc.GetBytes(p);
			MemoryStream strm = new MemoryStream();
			strm.Write(bytes, 0, bytes.Length);
			return strm;
		}
	}
}
