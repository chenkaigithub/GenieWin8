
namespace SV.UPnPLite.Protocols.DLNA.Services.RenderingControl
{
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public interface IRenderingControlService
    {
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
        Task SetVolume(uint instanceId, int volume);
    }
}