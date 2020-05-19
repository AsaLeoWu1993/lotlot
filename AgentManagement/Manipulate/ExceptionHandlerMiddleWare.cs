using Entity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Operation.Agent;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AgentManagement.Manipulate
{
    /// <summary>
    /// 处理异常中间层
    /// </summary>
    public class ExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleWare(RequestDelegate next)
        {
            this.next = next;
            //Task.Run(async () =>
            //{
            //    AgentUserOperation agentUserOperation = new AgentUserOperation();
            //    var list = await agentUserOperation.GetModelListAsync(t => true);
            //    foreach (var data in list)
            //    {
            //        data.FreeDuration = 100;
            //        await agentUserOperation.UpdateModelAsync(data);
            //    }
            //});
        }

        #region
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null) return;
            await WriteExceptionAsync(context, exception).ConfigureAwait(false);
        }

        private static async Task WriteExceptionAsync(HttpContext context, Exception exception)
        {
            Utils.Logger.Error(exception);
            //返回提示
            var response = context.Response;

            //状态码
            response.StatusCode = (int)HttpStatusCode.OK;

            await response.WriteAsync(JsonConvert.SerializeObject(new RecoverModel() { Message = exception.Message, Status = RecoverEnum.系统错误 })).ConfigureAwait(false);
        }
        #endregion

    }
}
