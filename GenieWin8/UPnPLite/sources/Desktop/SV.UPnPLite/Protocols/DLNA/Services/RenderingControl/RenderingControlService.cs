
namespace SV.UPnPLite.Protocols.DLNA.Services.RenderingControl
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public class RenderingControlService : UPnPService, IRenderingControlService
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instanceId of the <see cref="RenderingControlService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c>.
        /// </exception>
        public RenderingControlService(string serviceType, Uri controlUri, Uri eventsUri)
            : base(serviceType, controlUri, eventsUri)
        {
        }

        /// <summary>
        ///     Initializes a new instanceId of the <see cref="RenderingControlService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c> -OR-
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public RenderingControlService(string serviceType, Uri controlUri, Uri eventsUri, ILogManager logManager)
            : base(serviceType, controlUri, eventsUri, logManager)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///      Set the Volume of the current resource that is associated with the specified instance.
        /// </summary>
        /// <param name="instanceId">
        ///      Identifies the virtual instanceId of the AVTransport service to which the action applies.
        /// </param>
        /// <param name="volume">
        ///     The Volume of the resource to set.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> instance which could be use for waiting an operation to complete.
        /// </returns>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public async Task SetVolume(uint instanceId, int volume)
        {
            try
            {
                var args = new Dictionary<string, object>();
                args["InstanceID"] = 0;
                args["Channel"] = "Master";
                args["DesiredVolume"] = volume;
                await this.InvokeActionAsync("SetVolume", args);
            }
            catch (FormatException)
            {
                // We don't expect result, so, we don't care if parsing error occurred
            }
        }
        #endregion
    }
}
