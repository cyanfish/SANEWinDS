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
#include "SANE_WIN__IDS_EntryWrapper.h"
#include "SANE_WIN__DS_EntryWrapper.h"

using namespace System;
using namespace System::Reflection;
//delegate void Message_From_DS_EventHandler(System::IntPtr _pOrigin, System::IntPtr _pDest, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, System::IntPtr _pData);

public ref class VBDLL
{
	static Object^ _managedObject = nullptr;
	static Assembly^ assem = Assembly::LoadFrom(gcnew System::String(GetAssemblyDirectory()) + "\\SANEWinDS.dll");
public:
	static Type^ MyType = assem->GetType("SANEWinDS.TWAIN_VB.DS_Entry_Pump");
	static Object^ GetWrapper()
	{
		try
		{
			if (_managedObject == nullptr)
			{
				_managedObject = Activator::CreateInstance(MyType);
				EventInfo^ MTDSEvent = MyType->GetEvent("Message_From_DS");
				Type^ handlerType = MTDSEvent->EventHandlerType;
				if (handlerType != nullptr)
				{
					Type^ delegateType = Type::GetType("Message_From_DS_EventReceiver");
					if (delegateType != nullptr)
					{
						Delegate^ d = Delegate::CreateDelegate(handlerType, delegateType, "OnMessage_From_DS");
						MTDSEvent->AddEventHandler(_managedObject, d);
					}
				}
			}
			return _managedObject;
		}
		catch (exception& e)
		{
			//XXX
		}
	}

	static System::String^ GetAssemblyDirectory()
	{
        System::String^ codeBase = Assembly::GetExecutingAssembly()->CodeBase;
		System::String^ filePath = Uri(codeBase).LocalPath;
        return System::IO::Path::GetDirectoryName(filePath);
	}
};

public ref class Message_From_DS_EventReceiver
{
public:
	static void OnMessage_From_DS(System::IntPtr _pOrigin, System::IntPtr _pDest, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, System::IntPtr _pData)
	{
		TW_INT16 result = _DSM_Entry((pTW_IDENTITY) (__int32) _pOrigin, (pTW_IDENTITY) (__int32) _pDest, _DG, _DAT, _MSG, (TW_MEMREF) _pData);
	}
};

void DS_EntryWrapper::Log(std::string message)
{
	try 
	{
		VBDLL vb;
		_managedObject = vb.GetWrapper();

		MethodInfo^ LogMethod = vb.MyType->GetMethod("Log");
		array<String^>^ params = gcnew array<String^>(1);
		params[0] = gcnew System::String(message.c_str());
		LogMethod->Invoke(_managedObject, params);
	}
	catch (exception& e)
	{
		//XXX
	}
}

TW_INT16 DS_EntryWrapper::Message_To_DS(pTW_IDENTITY _pOrigin, unsigned __int32 _DG, unsigned __int32 _DAT, unsigned __int16 _MSG, TW_MEMREF _pData)
{
	try
	{
		VBDLL vb;
		_managedObject = vb.GetWrapper();

		MethodInfo^ MTDSMethod = vb.MyType->GetMethod("Message_To_DS");
		array<System::Object^>^params = gcnew array<System::Object^>(5);
		params[0] = (System::IntPtr) _pOrigin;
		params[1] = (System::UInt32) _DG;
		params[2] = (System::UInt32) _DAT;
		params[3] = (System::UInt16) _MSG;
		params[4] = (System::IntPtr) _pData;
		return (TW_INT16) MTDSMethod->Invoke(_managedObject, params);
	}
	catch (exception& e) 
	{
		//XXX
	}
}

IDS_EntryWrapper  *IDS_EntryWrapper::CreateInstance()
{
    return ((IDS_EntryWrapper *) new DS_EntryWrapper());
}

void IDS_EntryWrapper::Destroy(IDS_EntryWrapper *instance)
{
    delete instance;
}