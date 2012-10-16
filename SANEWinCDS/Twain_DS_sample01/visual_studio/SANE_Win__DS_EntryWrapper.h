#pragma once

#include <vcclr.h>
#include <string>
#include "SANE_WIN__IDS_EntryWrapper.h"

//using namespace SANEWinDS::TWAIN_VB;

class DLLAPI DS_EntryWrapper : IDS_EntryWrapper
{
private:
    //gcroot<SANEWinDS::TWAIN_VB::DS_Entry_Pump ^>    _managedObject;
    gcroot<System::Object ^>    _managedObject;
    
public:
    DS_EntryWrapper() { }
    
    void		Log(std::string message);
	TW_INT16	Message_To_DS(pTW_IDENTITY _pOrigin, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData);
	//void		Message_From_DS(pTW_IDENTITY _pOrigin, pTW_IDENTITY _pDest, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData){}
};