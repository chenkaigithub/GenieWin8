#include "pch.h"
#include "DLNADelegation.h"

#define TRACK_TIMEOUT_COUNT 3

using namespace SiteView;

DLNADelegation * DLNADelegation::m_pThis = NULL;

DLNADelegation::DLNADelegation()
{
	m_core = new SiteView::DLNACore(this);
    m_bPhotosImpor = false;

    const char *dmrProtocolInfo =
    "http-get:*:image/png:*,"
    "http-get:*:image/jpeg:*,"
    "http-get:*:image/bmp:*,"
    "http-get:*:image/gif:*,"
    "http-get:*:audio/mpeg:*,"
    "http-get:*:audio/3gpp:*,"
    "http-get:*:audio/mp4:*,"
    "http-get:*:audio/x-ms-wma:*,"
    "http-get:*:audio/wav:*,"
    "http-get:*:video/mp4:*,"
    "http-get:*:video/mpeg:*,"
    "http-get:*:video/x-ms-wmv:*,"
    "http-get:*:video/x-ms-asf:*,"
    "http-get:*:video/3gpp:*,"
    "http-get:*:video/avi:*,"
    "http-get:*:video/quicktime:*";

	setProperty("PlatformName", "Win8");
	setProperty("OSVersion", "1.1");
	setProperty("DMRProtocolInfo", dmrProtocolInfo);
	setProperty("FriendlyName", "Genie Test");

	m_core->start();
}

DLNADelegation::~DLNADelegation()
{
    if (m_currentServer)
    {
        m_currentServer->release();
        m_currentServer = NULL;
    }
    if (m_currentRender)
    {
        m_currentRender->release();
        m_currentRender = NULL;
    }
    if (m_trackOp) 
    {
		m_trackOp->release();
		m_trackOp = NULL;
	}

    delete m_core;
}

DLNADelegation* DLNADelegation::GetInstance()
{
	if (NULL == m_pThis)
	{
		m_pThis = new DLNADelegation();
	}
	return m_pThis;
}


//////////////////////////////////////////////////////////////////////////
// Callback
void DLNADelegation::onMediaRendererListChanged()
{
}


void DLNADelegation::onMediaServerListChanged()
{
	DeviceDescList ls = m_core->snapshotMediaServerList();
}


void DLNADelegation::onMediaServerStateVariablesChanged(SiteView::DeviceDesc *deviceDesc, SiteView::ServiceDesc *serviceDesc, const NPT_List<NPT_String>& nameList, const NPT_List<NPT_String>& valueList)
{
}

void DLNADelegation::onMediaRendererStateVariablesChanged(SiteView::DeviceDesc *deviceDesc, SiteView::ServiceDesc *serviceDesc, const NPT_List<NPT_String>& nameList, const NPT_List<NPT_String>& valueList)
{
}


void DLNADelegation::dmrOpen(const NPT_String& url, const NPT_String& mimeType, const NPT_String& metaData)
{
}
void DLNADelegation::dmrPlay()
{
}
void DLNADelegation::dmrPause()
{
}
void DLNADelegation::dmrStop()
{
}
void DLNADelegation::dmrSeekTo(NPT_Int64 timeInMillis)
{
}
void DLNADelegation::dmrSetMute(bool mute)
{
}
void DLNADelegation::dmrSetVolume(int volume)
{
}

// DLNA browse callback 
void DLNADelegation::onDLNACoreOpFinished(SiteView::DLNACoreOp *op)
{
	printf("browse fineshed\n");
}

void DLNADelegation::onDLNAProgressiveBrowseOpResult(SiteView::DLNAProgressiveBrowseOp *op, NPT_UInt32 startingIndex, NPT_UInt32 numberReturned, NPT_UInt32 totalMatches, const SiteView::DLNAObjectList& ls)
{
	printf("browse fileNumber:%d\n", numberReturned);
	if (wait(m_BroOp))
        {
            if (m_BroOp->succeeded()) 
            {
                printf("borwse op  successed\n");
            }
            else
            {
                printf("borwse op un successed\n");
            }
        }
		else
        {
            printf("wait wrong-->click cancle btn\n");
        }
		m_BroOp->release();
}


bool DLNADelegation::wait(SiteView::DLNACoreOp* op)
{
    if (NPT_SUCCEEDED(op->wait(500)))
    {
        return true;
    }
    //WaitPopup  waitPopup;
    //if (op->checkFinishedIfNotSetCallback(&waitPopup))
    //{
    //    return true;
    //}
    //else
    //{
    //    waitPopup.showWaitDialog();
    //    UnLockMode mode = lockUI();
    //    op->resetCallback();
    //    if (UnLockModeManual == mode)
    //    {
    //        op->abort();
    //        return false;
    //    }
    //    return true;
    //}
	return true;
}

void DLNADelegation::importPhotos()
{    
	if (m_bPhotosImpor)
    {
        return;
    }
    SiteView::DLNACoreOp* op;
    if (NPT_SUCCEEDED(m_core->importPhotos("", &op)))
    {
        waitForImportPhotos();
        op->checkFinishedIfNotSetCallback(this);
        op->release();
        m_bPhotosImpor = true;
    }
}

void DLNADelegation::waitForImportPhotos()
{

}

void DLNADelegation::finishImportPhotos()
{

}

SiteView::DLNACore* DLNADelegation::getCore()  const
{
    return m_core;
}

SiteView::DeviceDescList DLNADelegation::getMediaRenderList() const
{
    return m_core->snapshotMediaRendererList();
}

SiteView::DeviceDescList DLNADelegation::getMediaServerList() const
{
    return m_core->snapshotMediaServerList();
}

void DLNADelegation::setCurrentServer(SiteView::DeviceDesc* mds)
{
    if (m_currentServer)
    {
        m_currentServer->release();
        m_currentServer = NULL;
    }
    m_currentServer = mds;
    m_currentServer->addRef();
    m_dmsCurrentUUID = mds->uuid();
}

void DLNADelegation::setCurrentRender(SiteView::DeviceDesc* mdr)
{
    if (m_currentRender)
    {
        m_currentRender->release();
        m_currentRender = NULL;
        m_dmrCurrentUUID = SiteView::UUID::null();
    }
    if (!mdr)
    {
        return;
    }
    m_currentRender = mdr;
    m_currentRender->addRef();
    m_dmrCurrentUUID = mdr->uuid();
    m_defaultRenderUUID = m_dmrCurrentUUID.toString();
}

SiteView::UUID DLNADelegation::getCurrentServerUUID() const
{
    return m_dmsCurrentUUID;
}

SiteView::UUID DLNADelegation::getCurrentRenderUUID() const
{
    return m_dmrCurrentUUID;
}

SiteView::DeviceDesc* DLNADelegation::getCurrentServer() const
{
    return m_currentServer;
}

SiteView::DeviceDesc* DLNADelegation::getCurrentRender() const
{
    return m_currentRender;
}

void DLNADelegation::traverseRenderList()
{
    const SiteView::DeviceDescList& dmrList = getMediaRenderList();
    SiteView::UUID defaultRenderUUID = SiteView::UUID::fromString(m_defaultRenderUUID);
    SiteView::DeviceDesc * defaultRender = dmrList.find(defaultRenderUUID);
    //setActiveRender(*defaultRender);
}

const SiteView::UUID& DLNADelegation::getServerUuidOfSelf()
{
    return m_core->getMediaServerUuid();
}
const SiteView::UUID& DLNADelegation::getRenderUuidOfSelf()
{
    return m_core->getMediaRendererUuid();
}

bool DLNADelegation::currentRenderIsSelf()
{
    if(m_defaultRenderUUID == getRenderUuidOfSelf().toString())
    {
        return true;
    }
    else
    {
        return false;
    }
}

void DLNADelegation::getMediaObjType(const SiteView::DLNAObject* media, DLNAMediaObjType & eType)
{
    if (media)
    {
        const NPT_String contentType = media->upnpClass().Left(21);
        if (contentType == "object.item.imageItem" ) 
        {
            eType = DLNAMediaObjTypeImage;
        } 
        else if (contentType == "object.item.videoItem") 
        {
            eType = DLNAMediaObjTypeVideo;
        } 
        else if (contentType == "object.item.audioItem")
        {
            eType = DLNAMediaObjTypeAudio;
        }
    }
}

/////////////////////
void DLNADelegation::restartUPNP()
{
        m_core->start();
}

void DLNADelegation::refreshMediaSource()
{
    m_core->clearMediaServerContent();
    m_core->mediaStore()->importIOSPhotos("");
}

void DLNADelegation::startUPNPRender()
{
    m_core->enableFunction(DLNACore::Function_MediaRenderer, true);
}

void DLNADelegation::startUPNPServer()
{
    m_core->enableFunction(DLNACore::Function_MediaServer, true);
}

void DLNADelegation::startUPNPControlPoint()
{
    m_core->enableFunction(DLNACore::Function_ControlPoint, true);
}

void DLNADelegation::stopUPNPRender()
{
    m_core->enableFunction(DLNACore::Function_MediaRenderer, false);
}

void DLNADelegation::stopUPNPServer()
{
    m_core->enableFunction(DLNACore::Function_MediaServer, false);
}

void DLNADelegation::stopUPNPControlpoint()
{
    m_core->enableFunction(DLNACore::Function_ControlPoint, false);
}

void DLNADelegation::setProperty(const NPT_String& name, const NPT_String& value)
{
    m_core->setProperty(name,value);
}

void DLNADelegation::importFileSystemToMediaServer(const NPT_List<NPT_String>& dirs, const NPT_List<NPT_String>& names, SiteView::ShareType eType)
{
	m_core->clearMediaServerContent();
	m_core->importFileSystemToMediaServer(dirs, names, false, eType);
}

SiteView::ServiceDesc* DLNADelegation::findServiceByType(const NPT_String& serviceType) const
{
    return m_currentRender->findServiceByType(serviceType);
}

SiteView::ServiceDesc* DLNADelegation::findServiceById(const NPT_String& serviceType) const
{
    return m_currentRender->findServiceById(serviceType);
}

NPT_Result DLNADelegation::queryStateVariables(const NPT_String& serviceId, const NPT_List<NPT_String>& nameList, NPT_List<NPT_String>& valueList)
{
    return m_core->queryStateVariables(m_dmrCurrentUUID, serviceId, nameList, valueList);
}

///////////////////UPNP
void DLNADelegation::browse(const int nUUID, const NPT_String& serverOrDirTitle, const NPT_String& containerId)
{
	//DLNAProgressiveBrowseOp * op;
	DeviceDescList devlist = m_core->snapshotMediaServerList();
	UUID uid = devlist.itemAt(nUUID)->uuid();
	m_core->browseMediaServerEx(uid, containerId, 100, this, &m_BroOp);
	m_BroOp->checkFinishedIfNotSetCallback(this);
	printf("browse....\n");

	//SiteView::DLNABrowseOp *op;
	//if (NPT_SUCCEEDED(m_core->browseMediaServer(m_dmsCurrentUUID, containerId, false, &op))) 
/*	if (NPT_SUCCEEDED(m_core->browseMediaServerEx(uid, containerId, 100, this, &op)))
    {
        if (wait(op))
        {
            if (op->succeeded()) 
            {
                printf("borwse op  successed\n");
            }
            else
            {
                printf("borwse op un successed\n");
            }
        }
		else
        {
            printf("wait wrong-->click cancle btn\n");
        }
		op->release();
	}	*/
}

void DLNADelegation::refreshDevices(DLNACore::FlushMode mode)
{
//	m_core->flushDeviceList(flushMode);
	m_core->searchDevices(15);
}


void DLNADelegation::setThumbImg(NPT_String& url, DLNAMediaObjType mediaType)//2012.2.9
{

}

void DLNADelegation::openMediaObj(const SiteView::DLNAItem* mediaItem)
{
	DeviceDescList devlist = m_core->snapshotMediaServerList();
	UUID renderUuid = devlist.itemAt(0)->uuid();

	DLNAObjectList objList;
	DLNAObject * obj = objList.itemAt(0);
	DLNAItem * item = obj->asItem();

   // SiteView::UUID renderUuid = getCurrentRenderUUID();
	if (!renderUuid.isNull()) 
    {
        DLNAMediaObjType type;
		getMediaObjType(mediaItem, type);
		
        NPT_String iconUrl;
        if (((SiteView::DLNAObject*)mediaItem)->findThumbnailURL(200, 200, NULL, iconUrl))
        {
            //setThumbImg(iconUrl, type);
        }
        
		SiteView::DLNACoreOp *op;
		if (NPT_SUCCEEDED(m_core->playMedia(renderUuid, mediaItem, &op))) 
        {
            if (wait(op))
            {
                if (op->succeeded()) 
                {
                    //[m_delegate showControlPointPageWithMediaType:type];
                }
                else
                {
                    printf("play op un successed");
                    printf("\n");
                }
            }
			else
            {
                printf("wait wrong-->click cancle btn");
                printf("\n");
            }
			op->release();
		}
        else
        {
            //NSLog(@"create Op error");
        }
	}
    else
    {
        //[m_delegate shouldSelectRenderForMedia:mediaItem];
    }
}

void DLNADelegation::openMediaObj(const SiteView::DLNAItem* mediaItem, unsigned int timeout)
{
    SiteView::UUID renderUuid = getCurrentRenderUUID();
	if (!renderUuid.isNull()) 
    {
        DLNAMediaObjType type;
		getMediaObjType(mediaItem, type);
        NPT_String iconUrl;
        if (((SiteView::DLNAObject*)mediaItem)->findThumbnailURL(200, 200, NULL, iconUrl))
        {
            setThumbImg(iconUrl, type);
        }
        
		SiteView::DLNACoreOp *op;
		if (NPT_SUCCEEDED(m_core->playMedia(renderUuid, mediaItem, &op))) 
        {
            /*if (NPT_SUCCEEDED(op->wait(timeout*1000)) && op->succeeded())
             {
             [m_delegate showControlPointPageWithMediaType:type];
             
             }
             /*/
            if (wait(op))
            {
                if (op->succeeded())
                {
                    //[m_delegate showControlPointPageWithMediaType:type];
                }
            }
            //*/
			else
            {
                //[m_delegate autoPlaybackTimeOut];
            }
			op->release();
		}
        else
        {
            
        }
	}
    else
    {
        //[m_delegate shouldSelectRenderForMedia:mediaItem];
    }
}

void DLNADelegation::setActiveServer(SiteView::DeviceDesc& device)
{
    setCurrentServer(&device);
}

void DLNADelegation::setActiveRender(SiteView::DeviceDesc& device)
{
    setCurrentRender(&device);
    //[m_delegate reloadMeidaRenderList:getMediaRenderList() withCurrentRenderID:m_defaultRenderUUID];
}



void DLNADelegation::queryTrackInfo()
{
/*	if (m_trackCount>TRACK_TIMEOUT_COUNT)
    {
        m_trackOp->release();
		m_trackOp = NULL;
    }

    if (!m_trackOp)
    {
		if (NPT_SUCCEEDED(m_core->queryMediaPositionInfo(m_dmrCurrentUUID, &m_trackOp))) 
        {
			m_trackOp->checkFinishedIfNotSetCallback(this);
		}
	}
    else
    {
        printf("-------------track no return\n");
        m_trackCount++;
    }	*/
}

int DLNADelegation::onQueryMediaPositionFinished()
{
    int track = 0;
	if (m_trackOp) 
    {
		if (m_trackOp->succeeded()) 
        {
			track = m_trackOp->trackTime();
		}

        printf("-------------%d\n",track);
		m_trackOp->release();
		m_trackOp = NULL;
	}
    m_trackCount = 0;//if track successed  reset  count
    return track;
}

void DLNADelegation::stop()
{
    if (m_dmrCurrentUUID.isNull())
    {
        //[delegate() notAviableRender];
        return;
    }
    
	SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->stopMedia(m_dmrCurrentUUID, &op)))
    {
        op->release();
    }
}

void DLNADelegation::pause()
{
    if (m_dmrCurrentUUID.isNull())
    {
        //[delegate() notAviableRender];
        return;
    }
    
	SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->pauseMedia(m_dmrCurrentUUID, &op)))
    {
        op->release();
    }
}

void DLNADelegation::play()
{
    if (m_dmrCurrentUUID.isNull())
    {
        //[delegate() notAviableRender];
        return;
    }
    
	SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->playMedia(m_dmrCurrentUUID, NULL, &op)))
    {
        op->release();
    }
}


void DLNADelegation::setVolume(int volume)
{
    if (m_dmrCurrentUUID.isNull()) 
    {
        //[delegate() notAviableRender];
		return;
	}
	
	SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->changeMediaVolume(m_dmrCurrentUUID, volume, &op)))
    {
        op->release();
    }
}

void DLNADelegation::setProgress(int progress)
{
    if (m_dmrCurrentUUID.isNull())
    {
        //[delegate() notAviableRender];
        return;
    }
    
	SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->seekMedia(m_dmrCurrentUUID, progress, &op)))
    {
        op->release();
    }
}


void DLNADelegation::setMute(bool mute)
{
    if (m_dmrCurrentUUID.isNull())
    {
        //[delegate() notAviableRender];
        return;
    }

    SiteView::DLNACoreOp *op;
    if (NPT_SUCCEEDED(m_core->muteMedia(m_dmrCurrentUUID, mute, &op)))
    {
        op->release();
    }
}

void DLNADelegation::renderReportPlayBackState(DLNAMediaPlayBackState state)
{
    if (m_core)
    {
		switch (state) 
        {
            case DLNAMediaPlayBackStatePlaying:
                m_core->dmrReportState(SiteView::DLNACore::DMRState_Playing);
                break;
            case DLNAMediaPlayBackStatePause:
                m_core->dmrReportState(SiteView::DLNACore::DMRState_Paused);
                break;
            case DLNAMediaPlayBackStateStop:
                m_core->dmrReportState(SiteView::DLNACore::DMRState_Stopped);
                break;
            case DLNAMediaPlayBackStateErr:
                m_core->dmrReportErrorStatus(true);
                break;
		}
	}
}

void DLNADelegation::renderReportProgress(long long playbackSeconds, long long durationSeconds)
{
    if (m_core) 
    {
		m_core->dmrReportProgress(playbackSeconds*1000, durationSeconds*1000);
	}
}