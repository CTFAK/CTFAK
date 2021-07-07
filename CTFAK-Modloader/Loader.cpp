#include "Loader.h"
#include <iostream>
#include "detours.h"
#include <imgui.h>
#include <comdef.h> 
#include <string> 
#include "Settings.h"
#include "Memory.h"
#include "GUIHandler.h"




using namespace std;
int(__fastcall* SetCounter_15100)(LPRUNOBJECT a1, CValue* a2);
int(__cdecl* SetPosition_193F0)(LPRUNOBJECT a1);
int(__fastcall* EventLoop_58F80)(LPRH a1, int a2);
void(__thiscall* SwitchFrame_25800)(void* a1);
int (__fastcall* ReadChunks_3270)(void* a1, int a2, char* a3, int a4, int a5, int a6);
int gameDataCreated=0;

typedef int(__cdecl* updateObject)(void* object);
static updateObject UpdateObject;

CRunApp* GetGameBase()
{
	if (Loader::currentApp)
	{
		if (Loader::currentApp->m_miniHdr.gaType[4] == 'M')
		{
			return Loader::currentApp;
		}
		else
		{
			Loader::currentApp = 0;
			return GetGameBase();
		}
	}
	else
	{
		Loader::currentApp = (CRunApp*)*(void**)(Loader::GameBase + 0xAC9AC);
		if (Loader::currentApp->m_miniHdr.gaType[3] == 'M')
		{
			return Loader::currentApp;
		}
		else
		{
			Loader::currentApp = (CRunApp*)*(void**)(Loader::GameBase + 0xB60E4);
			if (Loader::currentApp->m_miniHdr.gaType[3] == 'M')
			{
				return Loader::currentApp;
			}
			else return NULL;
		}
		
		
	}
	
}

LPOI GetOIFromRunObj(LPRUNOBJECT obj)
{
	for (size_t i = 0; i < Loader::currentApp->m_oiMaxIndex; i++)
	{
		if ((Loader::currentApp->m_ois[i]->oiHdr.oiHandle) == (obj->roHo.hoOi))return Loader::currentApp->m_ois[i];
	}
	printf("OBJECT NOT FOUND\n");
	return NULL;
}

LPRUNOBJECT GetObjectFromHandle(int handle)
{
	auto obj = ((LPRUNOBJECT*)(Loader::currentApp->m_Frame->m_objectList))[handle*2];
	//auto list = Memory::FindDMAAddy(Loader::GameBase + 0xAC9AC, { 0x1D8 ,0x8D0 });
	//auto obj = *(int*)((*(int*)list) + handle * 8);
	return (LPRUNOBJECT)obj;
}

int cheatCounterValue = 0;
int selectedObjectHandle = 0;
LPRUNOBJECT selectedObject;
void Loader::DrawUI()
{
	//ImGui::ShowDemoWindow();
	if (!Loader::currentApp) Loader::currentApp = GetGameBase();
	wstring name;
	if (ImGui::Begin(_bstr_t(Loader::currentApp->m_name)))
	{
		if (ImGui::CollapsingHeader("Main"))
		{
			name = L"cock";//Loader::gameDataPtr->APPNAME;
			ImGui::Text((string("Name: ") + string(name.begin(), name.end())).c_str());
		}

		if (ImGui::CollapsingHeader("Object Manipulator"))
		{
			if (ImGui::TreeNode("Selection"))
			{
				//ImGui::InputInt("Object ID", &selectedObjectHandle);
				for (size_t i = 0; i < (Loader::currentApp->m_Frame->m_loMaxIndex); i++)
				{
					auto obj = ((LPRUNOBJECT*)Loader::currentApp->m_Frame->m_objectList)[i*2];
					if (obj != NULL)
					{
						auto objName = _bstr_t(GetOIFromRunObj(obj)->oiName);
						if (ImGui::Button(objName))
						{
							printf("Selected object %s (%X)\n", string(objName).c_str(), obj->roHo.hoAddress);
							selectedObject = obj;//GetObjectFromHandle(i);
						}

					}
					//if (ImGui::Button("Select"))selectedObject = GetObjectFromHandle(selectedObjectHandle);
				}
				
				ImGui::TreePop();
			}
			
			if (selectedObject)
			{
				if (ImGui::TreeNode("Basic"))
				{
					
					if (ImGui::Button("Hide"))
					{

						//UpdateObject(selectedObject);
						
					}
					ImGui::Button("Show");
					static bool autoPos;
					static int newX;
					static int newY;

					ImGui::Checkbox("Auto-update position", &autoPos);
					if (autoPos)
					{
						if(ImGui::InputInt("X", &newX)) callRunTimeFunction2(selectedObject->roHo.hoAddress, RFUNCTION_SETPOSITION, newX, selectedObject->roHo.hoY);
						if(ImGui::InputInt("Y", &newY))	callRunTimeFunction2(selectedObject->roHo.hoAddress,RFUNCTION_SETPOSITION,selectedObject->roHo.hoX,newY);
						
					}
					else
					{
						ImGui::InputInt("X", &newX);
						ImGui::InputInt("Y", &newY);
						if(ImGui::Button("Set Position")) callRunTimeFunction2(selectedObject->roHo.hoAddress, RFUNCTION_SETPOSITION, newX, newY);

					}
					ImGui::TreePop();
				}

				if (ImGui::TreeNode("Counter"))
				{
					ImGui::InputInt("Value", &cheatCounterValue);
					if (ImGui::Button("Set new value"))
					{
						CValue val;
						//val.m_double = cheatCounterValue;
						val.m_long = cheatCounterValue;
						val.m_type = 0;
					
						SetCounter_15100(selectedObject, &val);
					}
					bool isFreeze;
					ImGui::Checkbox("Freeze", &isFreeze);
					ImGui::TreePop();
				}
			}
			//auto objects = (ObjectList*)FindDMAAddy(Loader::GameBase + 0xAC9AC, { 0x190 });
			//ImGui::Text((string("Object Count: ") + to_string(objects->objectCount)).c_str());
			
			
		}
		if (ImGui::CollapsingHeader("Settings"))
		{
			ImGui::Checkbox("Disable Events", &(Settings::disableEvents));
		}
		
	
		ImGui::End();
	}


	
}





int __fastcall SetCounerHook(LPRUNOBJECT a1, CValue* a2)
{
	int result = SetCounter_15100(a1, a2);
	cout << "Counter " << a1 << " changed to " << a2->m_long << endl;
	
	return result;
}
int __cdecl SetPositionHook(LPRUNOBJECT a1)
{
	int result = SetPosition_193F0(a1);
	if (a1) printf("Position of %X changed to %u\n", a1);

	return result;
}
int __fastcall EventLoopHook(LPRH a1, int a2)
{
	
	//if((GetAsyncKeyState(VK_HOME) & 0x8000) != 0)printf("%X\n", a1);
	if ((GetAsyncKeyState(VK_HOME) & 0x8000) != 0)
	{
		//LPRH base = *(LPRH*)(Loader::GameBase + 0xAC9B4);
		//base->rhQuit = LOOPEXIT_GOTOLEVEL;
		//base->rhQuitParam = 5;
		//Loader::currentApp->m_pFree//key

	}

	//
	if (Settings::disableEvents)
	{
		return 0xFFFFFFF;
	}
	else return EventLoop_58F80(a1, a2);

}
void __fastcall SwitchFrameHook(void* a1)
{
	printf("Frame Switched. Args:  %X\n", a1);
	SwitchFrame_25800(a1);
}

int __fastcall ReadChunksHook(void* a1, int a2, char* a3, int a4, int a5, int a6)
{
	auto res = ReadChunks_3270(a1, a2, a3, a4, a5, a6);	
	
	return res;
}




void Loader::DoHooks(uintptr_t base, int gameType)
{
	//auto EventLoopPtr = Memory::aob(Loader::GameBase, 0x7FFFFFFF, "55 8B EC 83 EC 0C 53 56 57 8B 3D B4 C9 E8 00 8B D9 8B F2 C6 87 A6 01 00 00");
	if (gameType == 0)
	{
		SetCounter_15100 = (int(__fastcall*)(LPRUNOBJECT a1, CValue * a2))DetourFunction((PBYTE)(base+0x15100), (PBYTE)SetCounerHook);
		//SetPosition_193F0 = (int(__cdecl*)(LPRUNOBJECT a1))DetourFunction((PBYTE)(base+0x193F0), (PBYTE)SetPositionHook);
		EventLoop_58F80 = (int(__fastcall*)(LPRH a1,int a2))DetourFunction((PBYTE)(base+ 0x58F80), (PBYTE)EventLoopHook);
		SwitchFrame_25800 = (void(__thiscall*)(void* a1))DetourFunction((PBYTE)(base+ 0x25800), (PBYTE)SwitchFrameHook);
		ReadChunks_3270 = (int(__fastcall*)(void* a1, int a2, char* a3, int a4, int a5, int a6))DetourFunction((PBYTE)(base+ 0x3270), (PBYTE)ReadChunksHook);
	}
}


HMODULE(__stdcall* Original_LoadLibraryA)(LPCSTR lpLibFileName);
HMODULE __stdcall Hooked_LoadLibraryA(LPCSTR lpLibFileName)
{
	HMODULE lib = Original_LoadLibraryA(lpLibFileName);
	if (strcmp(lpLibFileName, "d3d9.dll") == 0)
	{
		CreateThread(nullptr, 0, HookDirect3D9, NULL, 0, nullptr);
	}
	return lib;
}

void Loader::Entry()
{
	FILE* pFile = nullptr;
	AllocConsole();
	freopen_s(&pFile, "CONOUT$", "w", stdout);
	Loader::GameBase = (uintptr_t)GetModuleHandle(NULL);
	Original_LoadLibraryA = (HMODULE(__stdcall*)(LPCSTR lpLibFileName))DetourFunction((PBYTE)(&LoadLibraryA), (PBYTE)Hooked_LoadLibraryA);
	Loader::DoHooks(Loader::GameBase, 0);
	//UpdateObject = (updateObject)(Loader::GameBase + 0x13A0);
	//UpdateObject = (updateObject)(Loader::GameBase + 0x193F0);

	

	
	
}
MonoDomain* Loader::Domain;
MonoMethod* Loader::OnUpdateMethod;
MonoMethod* Loader::OnFrameSwitchMethod;
MonoMethod* Loader::OnInjectedMethod;
CRunApp* Loader::currentApp;
uintptr_t Loader::GameBase;
HMODULE Loader::CTFAK;