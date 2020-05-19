using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Operation.RedisAggregate;
using System;
using System.Linq;
using System.Security.Claims;

namespace AgentManagement.Manipulate
{
    /// <summary>
    /// 用户验证身份
    /// </summary>
    public class AuthenticationAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //if (CheckMethod(context.ActionDescriptor)) return;
            //if (context.Result == null) return;
            //if (context.Result.GetType() != typeof(OkObjectResult))
            //{
            //    var type = context.Result.GetType();
            //    var status = type.GetProperty("StatusCode").GetValue(context.Result);
            //    var property = type.GetProperty("Value");
            //    var value = property.GetValue(context.Result);
            //    context.Result = new OkObjectResult(new { Status = status, Message = value });
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (CheckMethod(context.ActionDescriptor)) return;
            #region 验证
            var token = context.HttpContext.User.FindFirstValue("Token");
            var loginName = context.HttpContext.User.FindFirstValue("LoginName");
            var agentID = context.HttpContext.User.FindFirstValue("AgentID");
            var isHighest = context.HttpContext.User.FindFirstValue("IsHighest");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(agentID) || string.IsNullOrEmpty(isHighest))
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            var key = isHighest == "1" ? loginName + ":" + token : loginName + ":" + agentID;
            var redisToken = RedisOperation.GetValue(key, "Token");
            if (token != redisToken)
            {
                context.HttpContext.Session.Clear();
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            #endregion
        }

        /// <summary>
        /// 判断方法是否单独加特性
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private bool CheckMethod(ActionDescriptor descriptor)
        {
            //判断是否添加了不判断缓存信息
            if (descriptor is ControllerActionDescriptor)
            {
                var actionDescriptor = descriptor as ControllerActionDescriptor;
                var attributes = actionDescriptor.MethodInfo.CustomAttributes;
                if (attributes.FirstOrDefault(t => t.AttributeType == typeof(NotAuthenticationAttribute)) != null)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 不进行验证
    /// </summary>
    public class NotAuthenticationAttribute : Attribute
    {
    }
}
