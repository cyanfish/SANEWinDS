/*
'
'   Copyright 2011, 2012 Alec Skelly
'
'   This file is part of SANEWinDS.
'
'   SANEWinDS is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   SANEWinDS is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with SANEWinDS.  If not, see <http://www.gnu.org/licenses/>.
'
*/
/***************************************************************************
* Copyright � 2007 TWAIN Working Group:  
*   Adobe Systems Incorporated, AnyDoc Software Inc., Eastman Kodak Company, 
*   Fujitsu Computer Products of America, JFL Peripheral Solutions Inc., 
*   Ricoh Corporation, and Xerox Corporation.
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the TWAIN Working Group nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY TWAIN Working Group ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL TWAIN Working Group BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
***************************************************************************/

/**
* @file CTWAINDS_Sample1.cpp
* Reveals the main entry point for the DSM 
* @author TWAIN Working Group
* @date April 2007
*/

#include "CTWAINDS_Sample1.h"
#include <list>
//#import "C:\VBProjects\TWAIN_Test\TWAIN_Test\bin\Debug\TWAIN_Test.tlb" raw_interfaces_only

//#pragma managed

//#pragma once

#pragma unmanaged

#include "SANE_Win__IDS_EntryWrapper.h"
//#include "c:\vbprojects\TWAIN_Test_Tester\TWAIN_Test_Tester\IMessageBoxWrapper.h"


//////////////////////////////////////////////////////////////////////////////
// Globals
/**
* gloabal pointer of the Data Source, for access to the main DS_Entry.
*/
typedef struct _DS_inst
{
  TW_IDENTITY AppId;
  CTWAINDS_Base *pDS; 
}DS_inst;

typedef list<DS_inst> lstDS;
lstDS g_lstDS;

typedef struct _DS_inst_VB
{
	TW_IDENTITY AppId;
	IDS_EntryWrapper *pDS;
} DS_inst_VB;

typedef list<DS_inst_VB> lstDS_VB;
lstDS_VB g_lstDS_VB;

#ifdef TWH_CMP_MSC
  /** 
  * gloadbal Windows Instance handle for the DSM DLL... 
  */
  HINSTANCE   g_hinstance     = 0;
#endif

//////////////////////////////////////////////////////////////////////////////
// This is the main entry point. This function is dlsym'd by the DSM.

#ifdef TWH_CMP_MSC
TW_UINT16 FAR PASCAL
#else
FAR PASCAL TW_UINT16 
#endif
DS_Entry( pTW_IDENTITY _pOrigin,
          TW_UINT32    _DG,
          TW_UINT16    _DAT,
          TW_UINT16    _MSG,
          TW_MEMREF    _pData)
{

  IDS_EntryWrapper* pTWAIN_VB = 0;
  if(_pOrigin)
  {
    lstDS_VB::iterator llIter=g_lstDS_VB.begin();
    for(;llIter!=g_lstDS_VB.end();llIter++)
    {
      if((*llIter).AppId.Id==_pOrigin->Id)
      {
        pTWAIN_VB=(*llIter).pDS;
      }
    }
  }
  

  //CTWAINDS_Base* pTWAINLayer = 0;

  //if(_pOrigin)
  //{
  //  lstDS::iterator llIter=g_lstDS.begin();
  //  for(;llIter!=g_lstDS.end();llIter++)
  //  {
  //    if((*llIter).AppId.Id==_pOrigin->Id)
  //    {
  //      pTWAINLayer=(*llIter).pDS;
  //    }
  //  }
  //}

  // Curently we are not open
  if( 0 == pTWAIN_VB )
  {

	if( DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_GET == _MSG )
    {
      // Copy the ID assigned by the DSM eventhough the spec states
      // that the id will not be assigned until MSG_OPENDS
      //CTWAINDS_Base::m_TheIdentity.Id = ((pTW_IDENTITY)_pData)->Id;
      //memcpy( _pData, &CTWAINDS_Base::m_TheIdentity, sizeof(CTWAINDS_Base::m_TheIdentity) );
	 

      //return TWRC_SUCCESS;
    }


     // Open the DS

	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	pTWAIN_VB = IDS_EntryWrapper::CreateInstance();
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    if( NULL == pTWAIN_VB )
    {
      // Failed to create the DS 
      //setConditionCode(TWCC_LOWMEMORY);
      return TWRC_FAILURE;
	} else {
			pTWAIN_VB->Log("Created new instance");
    }

	DS_inst_VB _DS_VB;
	_DS_VB.pDS = pTWAIN_VB;
	//_DS_VB.AppId = *_pOrigin; 'XXX without this we create a new object for each message
	//Below is a partial workaround.  This needs more thought.
	if (_pOrigin) {
		_DS_VB.AppId = *_pOrigin;
	}

	g_lstDS_VB.push_back(_DS_VB);
  }
  // If we were not open before, we are now, so continue with the TWAIN call
  TW_INT16 result = pTWAIN_VB->Message_To_DS(_pOrigin, _DG, _DAT, _MSG, _pData);

 // // Curently we are not open
 // if( 0 == pTWAINLayer )
 // {
 //   // Special case DSM can request to get identity information about 
 //   // DS before it is open.  In this special case, where the DS is not
 //   // open, we return this static Idenity.
 //   if( DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_GET == _MSG )
 //   {
 //     // Copy the ID assigned by the DSM eventhough the spec states
 //     // that the id will not be assigned until MSG_OPENDS
 //     //CTWAINDS_Base::m_TheIdentity.Id = ((pTW_IDENTITY)_pData)->Id;
 //     //memcpy( _pData, &CTWAINDS_Base::m_TheIdentity, sizeof(CTWAINDS_Base::m_TheIdentity) );

 //     //return TWRC_SUCCESS;
	//  return result;
 //   }

 //   // The DS is not open.  If we get a request to close DS do not open 
 //   // just to close, instead return that it is success closed.
 //   if( DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_CLOSEDS == _MSG )
 //   {
 //     //return TWRC_SUCCESS;
	//  return result;
 //   }

 //   // Open the DS

	////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	////pTWAIN_VB = IDS_EntryWrapper::CreateInstance();
	////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

	//pTWAINLayer = new CTWAINDS_FreeImage(*_pOrigin);
 //   if( NULL == pTWAINLayer 
 //    || TWRC_SUCCESS != pTWAINLayer->Initialize())
 //   {
 //     // Failed to create the DS 
 //     //setConditionCode(TWCC_LOWMEMORY);
 //     if(pTWAINLayer)
 //     {
 //       // Created but could not Initialize
 //       delete pTWAINLayer;
 //     }
 //     return TWRC_FAILURE;
 //   }
 //   DS_inst _DS;
 //   _DS.pDS = pTWAINLayer;
 //   _DS.AppId = *_pOrigin;
 //   g_lstDS.push_back(_DS);

	////DS_inst_VB _DS_VB;
	////_DS_VB.pDS = pTWAIN_VB;
	////_DS_VB.AppId = *_pOrigin;
	////g_lstDS_VB.push_back(_DS_VB);
	////

 // }

  // If we were not open before, we are now, so continue with the TWAIN call
  //TW_INT16 result = pTWAINLayer->DS_Entry(_pOrigin, _DG, _DAT, _MSG, _pData);
  //result = pTWAINLayer->DS_Entry(_pOrigin, _DG, _DAT, _MSG, _pData);

  /**
  * Special case - free memory if closing DS 
  * @todo keep track of what apps are connecting to the ds and only 
  * delete when count goes down to 0
  */
  //if( TWRC_SUCCESS == result && 
  //    DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_CLOSEDS == _MSG &&
  //    NULL != pTWAINLayer )
  //{
  //  lstDS::iterator llIter=g_lstDS.begin();
  //  for(;llIter!=g_lstDS.end();)
  //  {
  //    if((*llIter).AppId.Id==_pOrigin->Id)
  //    {
  //      delete  (*llIter).pDS;
  //      llIter = g_lstDS.erase(llIter);

		////IDS_EntryWrapper::Destroy(wrapper);

  //      continue;
  //    }
  //    llIter++;
  //  }
  //}

  if( TWRC_SUCCESS == result && 
      DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_CLOSEDS == _MSG &&
      NULL != pTWAIN_VB )
  {
    lstDS_VB::iterator llIter=g_lstDS_VB.begin();
    for(;llIter!=g_lstDS_VB.end();)
    {
      if((*llIter).AppId.Id==_pOrigin->Id)
      {
        delete  (*llIter).pDS;
        llIter = g_lstDS_VB.erase(llIter);

		//IDS_EntryWrapper::Destroy(wrapper);

        continue;
      }
      llIter++;
    }
  }


  return result;
}

//////////////////////////////////////////////////////////////////////////////


#ifdef TWH_CMP_MSC
/**
* DllMain is only needed for Windows, and it's only needed to collect
* our instance handle, which is also our module handle.  Don't ever
* put anything else in here, not even logging messages.  It just isn't
* safe...
*/
BOOL WINAPI DllMain(HINSTANCE _hmodule,
                    DWORD     _dwReasonCalled,
                    LPVOID)
{
  switch (_dwReasonCalled)
  {
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
      break;
    case DLL_PROCESS_ATTACH:
      g_hinstance = _hmodule;
      break;
    case DLL_PROCESS_DETACH:
      unLoadDSMLib();
      g_hinstance = 0;
      break;
  }
  return(TRUE);
}
#elif (TWNDS_CMP == TWNDS_CMP_GNUGPP)
    // Nothing for us to do...
#else
    #error Sorry, we do not recognize this system...
#endif

