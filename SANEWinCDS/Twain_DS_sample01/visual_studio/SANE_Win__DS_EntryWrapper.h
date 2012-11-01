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