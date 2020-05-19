using Entity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Operation.Common;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 处理异常中间层
    /// </summary>
    public class ExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// 中间层处理
        /// </summary>
        /// <param name="next"></param>
        public ExceptionHandlerMiddleWare(RequestDelegate next)
        {
            this.next = next;
        }

        #region
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    ////返回提示
                    //var response = context.Response;
                    //response.Redirect("/404.html");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null) return;
            await WriteExceptionAsync(context, exception).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static async Task WriteExceptionAsync(HttpContext context, Exception exception)
        {
            Utils.Logger.Error(exception);
            //返回提示
            var response = context.Response;

            //状态码
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.WriteAsync(JsonConvert.SerializeObject(new RecoverModel() { Message = JsonConvert.SerializeObject(exception), Status = RecoverEnum.系统错误 })).ConfigureAwait(false);

            //throw exception;
        }
        #endregion

    }
}
