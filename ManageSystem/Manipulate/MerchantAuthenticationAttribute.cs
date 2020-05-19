using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Operation.Agent;
using System;
using System.Linq;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 权限验证
    /// </summary>
    public class MerchantAuthenticationAttribute : Attribute, IActionFilter
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
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (CheckMethod(context.ActionDescriptor)) return;
            #region 验证
            var dic = context.HttpContext.GetMerchantRedisCookie();
            if (dic == null || dic.Count == 0)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            var userToken = context.HttpContext.Request.Headers["Token"].ToString();
            foreach (var item in dic)
            {
                context.HttpContext.Items.Add(item.Key, item.Value);
            }
            var token = dic["Token"];
            if (userToken != token)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            var merchantID = dic["MerchantID"];
            var merchantName = dic["MerchantName"];
            var seurityNo = dic["SeurityNo"];
            var type = dic["Type"];
            var maturityTime = Convert.ToDateTime(dic["MaturityTime"]);
            var agentID = dic["AgentID"];
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = advancedSetupOperation.GetModel(t => t.AgentID == agentID);
            if (maturityTime < DateTime.Now && setup.Formal)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "商户已过期！", Status = RecoverEnum.身份过期 });
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
                if (attributes.FirstOrDefault(t => t.AttributeType == typeof(NotMerchantAuthenticationAttribute)) != null)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 不进行验证
    /// </summary>
    public class NotMerchantAuthenticationAttribute : Attribute
    {
    }

    /// <summary>
    /// 用户权限验证
    /// </summary>
    public class UserAuthenticationAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (CheckMethod(context.ActionDescriptor)) return;
            if (context.Result == null) return;
            if (context.Result.GetType() != typeof(OkObjectResult))
            {
                var type = context.Result.GetType();
                var status = type.GetProperty("StatusCode").GetValue(context.Result);
                var property = type.GetProperty("Value");
                var value = property.GetValue(context.Result);
                context.Result = new OkObjectResult(new { Status = status, Message = value });
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (CheckMethod(context.ActionDescriptor)) return;
            #region 验证
            var dic = context.HttpContext.GetUserRedisCookie();
            if (dic == null || dic.Count == 0)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            var userToken = context.HttpContext.Request.Headers["Token"].ToString();
            foreach (var item in dic)
            {
                context.HttpContext.Items.Add(item.Key, item.Value);
            }
            var token = dic["Token"];
            if (userToken != token)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "用户登录过期！", Status = RecoverEnum.身份过期 });
                return;
            }
            var merchantID = dic["MerchantID"];
            var userID = dic["UserID"];
            var seurityNo = dic["SeurityNo"];
            var maturityTime = Convert.ToDateTime(dic["MaturityTime"]);
            var agentID = dic["AgentID"];
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = advancedSetupOperation.GetModel(t => t.AgentID == agentID);
            if (maturityTime < DateTime.Now && setup.Formal)
            {
                context.Result = new JsonResult(new RecoverModel() { Message = "商户已过期！", Status = RecoverEnum.身份过期 });
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
                if (attributes.FirstOrDefault(t => t.AttributeType == typeof(NotUserAuthenticationAttribute)) != null)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 不进行用户验证
    /// </summary>
    public class NotUserAuthenticationAttribute : Attribute
    {
    }
}
