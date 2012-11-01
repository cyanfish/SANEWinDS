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

#include DSMInterface.h
#define DLLAPI  __declspec(dllexport)

DLLAPI TW_UINT16 Message_To_DSM(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData)
	{
		return _DSM_Entry(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData)
};
