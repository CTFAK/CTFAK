#pragma once
#include <vector>

typedef void func(PEXCEPTION_POINTERS debug_context);

struct VEHUnit {
	bool exists = false;
	uintptr_t address;
	void* callback;
	byte oldByte;
	bool post_call = false;
	void enable()
	{
		*(byte*)address = 0xCC;
	}
	void disable()
	{
		*(byte*)address = oldByte;
	}
};
struct VEHStruct {
	std::vector<VEHUnit> list;
	VEHUnit Find(uintptr_t address) {

		for (int i = 0; i < list.size(); i++) {
			auto tpl = list[i];
			if (tpl.address == address || (tpl.address > address - 0x16 && tpl.address < address)) {
				return tpl;
			}
		}

		VEHUnit pl;
		return pl;
	};
	void Append(uintptr_t address, void* callback, bool postcall = false) {
		DWORD dwOld;
		VirtualProtect((void*)address, 1, PAGE_EXECUTE_READWRITE, &dwOld);

		VEHUnit h = {
			true,
			address,
			callback,
			*(byte*)address,
			postcall
		};
		list.push_back(h);

		h.enable();
	}
};
VEHStruct VEH;

LONG WINAPI CorruptionHandler(PEXCEPTION_POINTERS ExceptionInfo) {

	if (!ExceptionInfo || !ExceptionInfo->ExceptionRecord || !ExceptionInfo->ContextRecord)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}
	auto code = ExceptionInfo->ExceptionRecord->ExceptionCode;
	if (code != EXCEPTION_BREAKPOINT && code != EXCEPTION_SINGLE_STEP)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}

	auto h = VEH.Find((uintptr_t)ExceptionInfo->ContextRecord->Eip);
	if (h.exists) {
		if (code == EXCEPTION_BREAKPOINT) {
			h.disable();
			ExceptionInfo->ContextRecord->EFlags |= 0x100;
			if (!h.post_call) ((func*)h.callback)(ExceptionInfo);
			ExceptionInfo->ContextRecord->Eip = h.address;

			return EXCEPTION_CONTINUE_EXECUTION;
		}
		else {
			h.enable();
			ExceptionInfo->ContextRecord->EFlags &= ~(0x100);
			if (h.post_call) ((func*)h.callback)(ExceptionInfo);
			return EXCEPTION_CONTINUE_EXECUTION;
		}
	}
	else {
		return EXCEPTION_CONTINUE_EXECUTION;
	}
	return EXCEPTION_CONTINUE_EXECUTION;

}