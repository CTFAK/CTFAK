#include "Loader.h"
#include "Structs.h"
#include <iostream>
#include "detours.h"
#include <imgui.h>
#include <comdef.h> 
#include <string> 
#include "Settings.h"
#include "Memory.h"
#include "GUIHandler.h"


using namespace std;
int(__fastcall* SetCounter_15100)(RuntimeObject* a1, CounterValue* a2);
int(__cdecl* SetPosition_193F0)(RuntimeObject* a1);
int(__fastcall* EventLoop_58F80)(DWORD* a1, int a2);
void(__thiscall* SwitchFrame_25800)(void* a1);
int (__fastcall* ReadChunks_3270)(void* a1, int a2, char* a3, int a4, int a5, int a6);
int gameDataCreated=0;

RuntimeObject* GetObjectFromHandle(int handle)
{
	auto list = Memory::FindDMAAddy(Loader::GameBase + 0xAC9AC, { 0x1D8 ,0x8D0 });
	auto obj = *(int*)((*(int*)list) + handle * 8);
	return (RuntimeObject*)obj;
}

int cheatCounterValue = 0;
int selectedObjectHandle = 0;
RuntimeObject* selectedObject;
void Loader::DrawUI()
{
	//ImGui::ShowDemoWindow();
	if (!Loader::gameDataPtr) Loader::gameDataPtr = (DataStruct*)*(void**)(Loader::GameBase + 0xAC9AC);
	wstring name;
	if (ImGui::Begin(_bstr_t(Loader::gameDataPtr->APPNAME)))
	{
		if (ImGui::CollapsingHeader("Main"))
		{
			name = Loader::gameDataPtr->APPNAME;
			ImGui::Text((string("Name: ") + string(name.begin(), name.end())).c_str());
		}

		if (ImGui::CollapsingHeader("Object Manipulator"))
		{
			if (ImGui::TreeNode("Selection"))
			{
				ImGui::InputInt("Object ID", &selectedObjectHandle);
				if (ImGui::Button("Select"))selectedObject = GetObjectFromHandle(selectedObjectHandle);
				ImGui::TreePop();
			}
			
			if (selectedObject)
			{
				if (ImGui::TreeNode("Basic"))
				{
					ImGui::Button("Hide");
					ImGui::Button("Show");
					ImGui::TreePop();
				}
				if (ImGui::TreeNode("Counter"))
				{
					ImGui::InputInt("Value", &cheatCounterValue);
					if (ImGui::Button("Set new value"))
					{
						CounterValue val;
						val.a = 0;
						val.value = cheatCounterValue;
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





int __fastcall SetCounerHook(RuntimeObject* a1, CounterValue* a2)
{
	int result = SetCounter_15100(a1, a2);
	if (a1) printf("Counter %X changed to %u\n", a1, a2->value);
	
	return result;
}
int __cdecl SetPositionHook(RuntimeObject* a1)
{
	int result = SetPosition_193F0(a1);
	if (a1) printf("Position of %X changed to %u\n", a1);

	return result;
}
int __fastcall EventLoopHook(DWORD* a1, int a2)
{
	auto shit = GetProcAddress((HMODULE)Loader::GameBase, "StartApp");
	printf("%X", shit);
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
		SetCounter_15100 = (int(__fastcall*)(RuntimeObject* a1, CounterValue* a2))DetourFunction((PBYTE)(base+0x15100), (PBYTE)SetCounerHook);
		SetPosition_193F0 = (int(__cdecl*)(RuntimeObject* a1))DetourFunction((PBYTE)(base+0x193F0), (PBYTE)SetPositionHook);
		EventLoop_58F80 = (int(__fastcall*)(DWORD* a1,int a2))DetourFunction((PBYTE)(base+ 0x58F80), (PBYTE)EventLoopHook);
		SwitchFrame_25800 = (void(__thiscall*)(void* a1))DetourFunction((PBYTE)(base+ 0x25800), (PBYTE)SwitchFrameHook);
		ReadChunks_3270 = (int(__fastcall*)(void* a1, int a2, char* a3, int a4, int a5, int a6))DetourFunction((PBYTE)(base+ 0x3270), (PBYTE)ReadChunksHook);
	}
}
void Loader::InitMono()
{
	Mono::Initialize();

	Mono::mono_set_config_dir("C:\Program Files (x86)\Mono");
	const char* managedPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Secret Neighbor\\MelonLoader\\Managed";
	//const char* managedPath = "C:\\Program Files (x86)\\Mono\\lib\\mono\\4.7.2-api";
	Mono::mono_set_assemblies_path(managedPath);
	Mono::mono_assembly_setrootdir(managedPath);

	Loader::Domain = Mono::mono_jit_init("CTFAK");

	auto assembly = Mono::mono_domain_assembly_open(Loader::Domain, "CTFAK.Modhandler.dll");
	MonoImage* image = Mono::mono_assembly_get_image(assembly);
	MonoClass* klass = Mono::mono_class_from_name(image, "CTFAK.Modhandler", "Loader");

	Loader::OnInjectedMethod = Mono::mono_class_get_method_from_name(klass, "OnInjected", 1);
	Loader::OnUpdateMethod = Mono::mono_class_get_method_from_name(klass, "OnUpdate", 0);
	Loader::OnFrameSwitchMethod = Mono::mono_class_get_method_from_name(klass, "OnFrameChanged", 1);

	if (!Loader::OnInjectedMethod)cout << "No OnInjected Method" << endl;
	if (!Loader::OnUpdateMethod)cout << "No OnUpdate Method" << endl;
	if (!Loader::OnFrameSwitchMethod)cout << "No OnFrameChanged Method" << endl;

	MonoObject* exceptionObject = NULL;
	void* args = { &Loader::gameDataPtr };
	Mono::mono_runtime_invoke(Loader::OnInjectedMethod, NULL, &args, &exceptionObject);

	
	
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
	Loader::gameDataPtr = (DataStruct*)*(void**)(Loader::GameBase + (0xAC9AC));
	Original_LoadLibraryA = (HMODULE(__stdcall*)(LPCSTR lpLibFileName))DetourFunction((PBYTE)(&LoadLibraryA), (PBYTE)Hooked_LoadLibraryA);
	Loader::DoHooks(Loader::GameBase, 0);
	

	

	
	
}
MonoDomain* Loader::Domain;
MonoMethod* Loader::OnUpdateMethod;
MonoMethod* Loader::OnFrameSwitchMethod;
MonoMethod* Loader::OnInjectedMethod;
DataStruct* Loader::gameDataPtr;
uintptr_t Loader::GameBase;
HMODULE Loader::CTFAK;