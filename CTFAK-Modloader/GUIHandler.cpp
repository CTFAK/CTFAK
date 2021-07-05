#include <d3d9.h>
#pragma comment(lib, "d3d9.lib")
#include "imgui/imgui.h"
#include "imgui/imgui_impl_win32.h"
#include "imgui/imgui_impl_dx9.h"
#include "Loader.h"
#include <comdef.h> 
#include "GUIHandler.h"
#include "detours.h"
typedef LRESULT(CALLBACK* WNDPROC)(HWND, UINT, WPARAM, LPARAM);
extern LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
long(__stdcall* oEndScene)(LPDIRECT3DDEVICE9);
long(__stdcall* oReset)(LPDIRECT3DDEVICE9 pDevice, D3DPRESENT_PARAMETERS* pPresentationParameters);


WNDPROC oWndProc;
static HWND window = NULL;
LRESULT APIENTRY WndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {

	ImGui::GetIO().MouseDown[0] = ((GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0);
	//ImGui::GetIO().MouseWheel += GET_WHEEL_DELTA_WPARAM(wParam) > 0 ? +1.0f : -1.0f;
	//printf("%u\n", uMsg);
	if (true && ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
		return true;

	return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
}
long __stdcall hkReset(LPDIRECT3DDEVICE9 pDevice, D3DPRESENT_PARAMETERS* pPresentationParameters)
{

	
	//ImGui_ImplDX9_InvalidateDeviceObjects();
	long result = oReset(pDevice, pPresentationParameters);
	//ImGui_ImplDX9_CreateDeviceObjects();

	return result;
}

long __stdcall hkEndScene(LPDIRECT3DDEVICE9 pDevice)
{
	static bool init = true;
	if (init)
	{
		init = false;
		ImGuiIO& io = ImGui::GetIO();
		D3DDEVICE_CREATION_PARAMETERS params;
		pDevice->GetCreationParameters(&params);
		window = params.hFocusWindow;
		printf("Hooking WndProc\n");
		oWndProc = (WNDPROC)SetWindowLongPtr(window, GWLP_WNDPROC, (LONG_PTR)WndProc);
		printf("Hooked WndProc\n");
		ImGui::CreateContext();
		

		
		

		ImGui_ImplWin32_Init(window);
		ImGui_ImplDX9_Init(pDevice);
	}


	ImGui_ImplDX9_NewFrame();
	ImGui_ImplWin32_NewFrame();
	ImGui::NewFrame();
	//ImGui::ShowDemoWindow();
	Loader::DrawUI();


	ImGui::EndFrame();
	ImGui::Render();

	ImGui_ImplDX9_RenderDrawData(ImGui::GetDrawData());

	return oEndScene(pDevice); // Call original ensdcene so the game can draw

}
DWORD WINAPI HookDirect3D9(LPVOID lpParam)
{
	void* d3d9Device[119];

	IDirect3D9* pD3D = Direct3DCreate9(D3D_SDK_VERSION);
	if (!pD3D) return false;

	D3DPRESENT_PARAMETERS d3dpp = { 0 };
	d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
	d3dpp.hDeviceWindow = GetForegroundWindow();
	d3dpp.Windowed = ((GetWindowLong(d3dpp.hDeviceWindow, GWL_STYLE) & WS_POPUP) != 0) ? FALSE : TRUE;;

	IDirect3DDevice9* pDummyDevice = nullptr;
	HRESULT create_device_ret = pD3D->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, d3dpp.hDeviceWindow, D3DCREATE_SOFTWARE_VERTEXPROCESSING, &d3dpp, &pDummyDevice);
	if (!pDummyDevice || FAILED(create_device_ret))
	{
		pD3D->Release();

		return false;
	}

	memcpy(d3d9Device, *reinterpret_cast<void***>(pDummyDevice), sizeof(d3d9Device));

	pDummyDevice->Release();
	pD3D->Release();
	printf("Hooking EndScene function\n");
	//oReset = (long(__stdcall*)(LPDIRECT3DDEVICE9,D3DPRESENT_PARAMETERS*))DetourFunction((PBYTE)(d3d9Device[16]), (PBYTE)hkReset);
	oEndScene = (long(__stdcall*)(LPDIRECT3DDEVICE9))DetourFunction((PBYTE)(d3d9Device[42]), (PBYTE)hkEndScene);
	return TRUE;
}

