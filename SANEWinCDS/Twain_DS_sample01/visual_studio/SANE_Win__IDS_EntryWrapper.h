#pragma once
#include <string>
#include "..\src\CommonDS.h"

#define DLLAPI  __declspec(dllexport)

class DLLAPI IDS_EntryWrapper
{
public:
    virtual void		Log(std::string message) = 0;
	virtual TW_INT16	Message_To_DS(pTW_IDENTITY _pOrigin, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData) = 0;
	//virtual void		Message_From_DS(pTW_IDENTITY _pOrigin, pTW_IDENTITY _pDest, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData){}
    // Class factory
		
    static IDS_EntryWrapper		*CreateInstance();
    static void                 Destroy(IDS_EntryWrapper *instance);
};