﻿
namespace SV.UPnPLite.Protocols.DLNA
{
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;
    using SV.UPnPLite.Protocols.DLNA.Services.AvTransport;
    using SV.UPnPLite.Protocols.DLNA.Services.RenderingControl;
    using SV.UPnPLite.Protocols.UPnP;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Discovers the Media Renderer devices.
    /// </summary>
    public class MediaRenderersDiscovery : UPnPDevicesDiscovery<MediaRenderer>, IMediaRenderersDiscovery
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaRenderersDiscovery" /> class.
        /// </summary>
        public MediaRenderersDiscovery()
            : base("urn:schemas-upnp-org:device:MediaRenderer:1")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaRenderersDiscovery" /> class.
        /// </summary>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public MediaRenderersDiscovery(ILogManager logManager)
            : base("urn:schemas-upnp-org:device:MediaRenderer:1", logManager)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an instance of concrete <see cref="UPnPDevice"/> which manages a device.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="name">
        ///     A friendly name of the device.
        /// </param>
        /// <param name="services">
        ///     A set of UPnP service found on the device.
        /// </param>
        /// <returns>
        ///     A concrete instance of the <see cref="UPnPDevice"/> if all reuqired service available; otherwise, <c>null</c>.
        /// </returns>
        protected override MediaRenderer CreateDeviceInstance(string udn, string name, IEnumerable<UPnPService> services)
        {
            var missingServices = new List<string>();
            
            var avTransportService = services.FirstOrDefault(s => s is IAvTransportService) as IAvTransportService;
            var renderingControlService = services.FirstOrDefault(s => s is IRenderingControlService) as IRenderingControlService;
            if (avTransportService == null)
            {
                missingServices.Add(typeof(IAvTransportService).Name);
            }
            if (renderingControlService == null)
            {
                missingServices.Add(typeof(IRenderingControlService).Name);
            }

            if (missingServices.Any() == false)
            {
                return new MediaRenderer(udn, avTransportService, renderingControlService, this.logManager);
            }
            else
            {
                this.logger.Instance().Warning(
                    "Can't add device as one of required services is missing. [missingServices=[{0}], deviceName={1}, deviceUDN={2}]",
                    string.Join(",", missingServices),
                    name,
                    udn);

                return null;
            }
        }

        /// <summary>
        ///     Creates an instance of concrere <see cref="UPnPService"/> which manages concrete service on a device.
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
        /// <returns>
        ///     A concrete instance of the <see cref="UPnPService"/>.
        /// </returns>
        protected override UPnPService CreateServiceInstance(string serviceType, Uri controlUri, Uri eventsUri)
        {
            UPnPService service = null;

            if (serviceType.StartsWith("urn:schemas-upnp-org:service:AVTransport", StringComparison.OrdinalIgnoreCase))
            {
                service = new AvTransportService(serviceType, controlUri, eventsUri, this.logManager);
            }
            
            if (serviceType.StartsWith("urn:schemas-upnp-org:service:RenderingControl", StringComparison.OrdinalIgnoreCase))
            {
                service = new RenderingControlService(serviceType, controlUri, eventsUri, this.logManager);
            }

            return service;
        }

        #endregion
    }
}