#include DSMInterface.h
#define DLLAPI  __declspec(dllexport)

DLLAPI TW_UINT16 Message_To_DSM(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData)
	{
		return _DSM_Entry(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData)
};
