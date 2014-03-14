/*****************************************************************************
Copyright (c) 2001 - 2009, SiteView.
All rights reserved.

*****************************************************************************/

#ifndef _DLNADELEGATION_HEAD_
#define _DLNADELEGATION_HEAD_

#include "DLNACore.h"

using namespace SiteView;

class DLNADelegation : 
	public SiteView::DLNACoreDelegate,
	public SiteView::DLNAProgressiveBrowseOp::ResultCallback,
	public SiteView::DLNACoreOp::FinishCallback
{
public:
    static DLNADelegation *  GetInstance();

	enum DLNAMediaObjType
	{
		DLNAMediaObjTypeVideo = 0,
		DLNAMediaObjTypeAudio,
		DLNAMediaObjTypeImage
	};

	enum DLNAMediaPlayBackState
	{
		DLNAMediaPlayBackStateStop = 0,
		DLNAMediaPlayBackStatePlaying,
		DLNAMediaPlayBackStatePause,
		DLNAMediaPlayBackStateErr,
		DLNAMediaPlayBackStateNone
	};

public:
    void importPhotos();
    void waitForImportPhotos();
    void finishImportPhotos();
    
    SiteView::DLNACore * getCore() const;
    SiteView::DeviceDescList getMediaRenderList() const;
    SiteView::DeviceDescList getMediaServerList() const;
    SiteView::UUID getCurrentServerUUID() const;
    SiteView::UUID getCurrentRenderUUID() const;
    SiteView::DeviceDesc* getCurrentServer() const;
    SiteView::DeviceDesc* getCurrentRender() const;
    void traverseRenderList();
	void onDiscoveryListChaned(const NPT_String& ip, const NPT_String& devName, const int type){}
    
    const SiteView::UUID& getServerUuidOfSelf();
    const SiteView::UUID& getRenderUuidOfSelf();
    bool currentRenderIsSelf();
    
    void restartUPNP();
    void refreshMediaSource();
    void startUPNPServer();
    void startUPNPRender();
    void startUPNPControlPoint();
    void stopUPNPServer();
    void stopUPNPRender();
    void stopUPNPControlpoint();
    
    void setProperty(const NPT_String& name, const NPT_String& value);
	void importFileSystemToMediaServer(const NPT_List<NPT_String>& dirs, const NPT_List<NPT_String>& names, SiteView::ShareType eType);
    
    SiteView::ServiceDesc *findServiceByType(const NPT_String& serviceType) const;
    SiteView::ServiceDesc *findServiceById(const NPT_String& serviceType) const;
    NPT_Result queryStateVariables(const NPT_String& serviceId, const NPT_List<NPT_String>& nameList, NPT_List<NPT_String>& valueList);    
    
public:
    void getMediaObjType(const SiteView::DLNAObject* media, DLNAMediaObjType & eType);
    void browse(const int nUUID, const NPT_String& serverOrDirTitle, const NPT_String& containerId);
    void refreshDevices(DLNACore::FlushMode mode);
    void openMediaObj(const SiteView::DLNAItem* mediaItem);
    void openMediaObj(const SiteView::DLNAItem* mediaItem, unsigned int timeout);
    void setActiveServer(SiteView::DeviceDesc& device);
    void setActiveRender(SiteView::DeviceDesc& device);
    
    //control point
    void queryTrackInfo();
    int onQueryMediaPositionFinished();
    void stop();
    void pause();
    void play();
    void setVolume(int volume);
    void setProgress(int progress);
    void setMute(bool mute);
    
    //render
    void renderReportPlayBackState(DLNAMediaPlayBackState state);
    void renderReportProgress(long long playbackSeconds, long long durationSeconds);

private:
	static DLNADelegation * m_pThis;
	SiteView::DLNACore * m_core;

	SiteView::UUID            m_dmsCurrentUUID;
	SiteView::UUID            m_dmrCurrentUUID;
	SiteView::DeviceDesc      * m_currentServer;
	SiteView::DeviceDesc      * m_currentRender;
	NPT_String              m_defaultRenderUUID;

	DLNAProgressiveBrowseOp * m_BroOp;
	SiteView::DLNAQueryPositionInfoOp         *m_trackOp;
	unsigned int                            m_trackCount;
	bool m_bPhotosImpor;

private:
	DLNADelegation();
	~DLNADelegation();

	bool wait(SiteView::DLNACoreOp* op);
	void loadConfig();
	void writeConfig();
	void WriteDLNAUserConfig();
	void setCurrentServer(SiteView::DeviceDesc* dms);
	void setCurrentRender(SiteView::DeviceDesc* dmr);
	void setThumbImg(NPT_String& url, DLNAMediaObjType mediaType);//2012.2.9//播放界面的缩略图 

protected:
	// call back
    virtual void onMediaServerListChanged();
	virtual void onMediaRendererListChanged();
	virtual void onMediaServerStateVariablesChanged(SiteView::DeviceDesc *deviceDesc, SiteView::ServiceDesc *serviceDesc, const NPT_List<NPT_String>& nameList, const NPT_List<NPT_String>& valueList);
	virtual void onMediaRendererStateVariablesChanged(SiteView::DeviceDesc *deviceDesc, SiteView::ServiceDesc *serviceDesc, const NPT_List<NPT_String>& nameList, const NPT_List<NPT_String>& valueList);
    
    virtual void dmrOpen(const NPT_String& url, const NPT_String& mimeType, const NPT_String& metaData);
	virtual void dmrPlay();
	virtual void dmrPause();
	virtual void dmrStop();
	virtual void dmrSeekTo(NPT_Int64 timeInMillis);
	virtual void dmrSetMute(bool mute);
	virtual void dmrSetVolume(int volume);
    
	virtual void onDLNACoreOpFinished(SiteView::DLNACoreOp *op);
    virtual void onDLNAProgressiveBrowseOpResult(SiteView::DLNAProgressiveBrowseOp *op, NPT_UInt32 startingIndex, NPT_UInt32 numberReturned, NPT_UInt32 totalMatches, const SiteView::DLNAObjectList& ls);
};
#endif