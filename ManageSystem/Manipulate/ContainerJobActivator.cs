using Hangfire;
using System;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 
    /// </summary>
    public class ContainerJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ContainerJobActivator(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        public override object ActivateJob(Type jobType)
        {
            return _serviceProvider.GetService(jobType);
        }
    }
}
